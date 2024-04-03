from db import (CRUD,
                create_db,
                engine,
                Teams,
                TeamsMembers,
                TeamsTags,
                TeamsInvites)
from schemas import (Status,
                     Team,
                     CreateTeam,
                     UpdateTeam,
                     UpdateTag,
                     InviteTeam,
                     AnswerInvite,
                     InviteTeamFull,
                     GetMyTeam,
                     WithoutTeam,
                     WithoutUsers,
                     AutogenerateTeam)
from settings import Settings, get_settings
from services import UsersAPI, EventsAPI

from typing import List, Union, Optional
from contextlib import asynccontextmanager
from collections import Counter

import uvicorn
from fastapi.encoders import jsonable_encoder
from fastapi import FastAPI, Depends, Request, Query
from fastapi.responses import JSONResponse
from sqlalchemy.ext.asyncio import async_sessionmaker
from rapidfuzz import fuzz


@asynccontextmanager
async def lifespan(app: FastAPI):
    print("Startup")
    await create_db()
    yield
    print("Shutdown")


teams = FastAPI(lifespan=lifespan)

settings: Settings = get_settings()

db = CRUD()
session_maker = async_sessionmaker(bind=engine)

version = "v1"
prefix = f"/api/{version}/"


class CustomException(Exception):
    def __init__(self, status_code: int, reason: str):
        self.status_code = status_code
        self.reason = reason


async def get_session():
    async with session_maker() as session:
        yield session


def get_Levenshtein_distance(user_tags: List[str], team_tags: List[str]) -> float:
    user_tags = " ".join(user_tags).lower()
    team_tags = " ".join(team_tags).lower()
    return fuzz.token_sort_ratio(user_tags, team_tags)


def get_tags(tags: List[TeamsTags]):
    return [tag.tag for tag in tags]


@teams.get(prefix + "ping", status_code=200, response_model=Status)
async def ping():
    """
    Ping handler
    :return:
    """
    return Status(status="OK")


@teams.get(prefix + "teams", status_code=200, response_model=List[Team])
async def get_teams(session=Depends(get_session)):
    """
    Handler for getting all teams
    :param session:
    :return:
    """
    response = list()
    teams_array = await db.get_teams(session)

    for team in teams_array:
        current_tags = list(team_tag.tag for team_tag in await db.get_team_tags(session, team.id))
        current_members = list(team_members.user_id for team_members in await db.get_team_members(session, team.id))
        current_team = Team(
            id=team.id, name=team.name,
            author_id=team.author_id, event_id=team.event_id,
            description=team.description, tags=current_tags,
            members=list(set(current_members + [team.author_id])), need=team.need, size=team.size
        )
        response.append(current_team)
    return response


@teams.get(prefix + "teams/{event_id}", status_code=200, response_model=List[Team])
async def get_teams_by_event(event_id: int, session=Depends(get_session)):
    """
    Handler for getting teams by event
    :param event_id:
    :param session:
    :return:
    """
    response = list()
    teams_array = await db.get_teams_by_event(session, event_id)

    for team in teams_array:
        current_tags = list(team_tag.tag for team_tag in await db.get_team_tags(session, team.id))
        current_members = list(team_members.user_id for team_members in await db.get_team_members(session, team.id))
        current_team = Team(
            id=team.id, name=team.name,
            author_id=team.author_id, event_id=team.event_id,
            description=team.description, tags=current_tags,
            members=list(set(current_members + [team.author_id])), need=team.need, size=team.size
        )
        response.append(current_team)
    return response


@teams.get(prefix + "team/{team_id}", status_code=200, response_model=Union[Team, Status])
async def get_team(team_id: str, session=Depends(get_session)):
    """
    Handler for getting teams by id
    :param team_id:
    :param session:
    :return:
    """
    team = await db.get_team(session, int(team_id))
    if team is None:
        raise CustomException(status_code=404, reason="Данная команда не найдена.")
    else:
        current_tags = list(team_tag.tag for team_tag in await db.get_team_tags(session, team.id))
        current_members = list(team_members.user_id for team_members in await db.get_team_members(session, team.id))
        current_team = Team(
            id=team.id, name=team.name,
            author_id=team.author_id, event_id=team.event_id,
            description=team.description, tags=current_tags,
            members=list(set(current_members + [team.author_id])), need=team.need, size=team.size
        )
        return current_team


@teams.post(prefix + "create", status_code=201)
async def create_team(create_data: CreateTeam, session=Depends(get_session)):
    """
    Handler for creating teams
    :param create_data:
    :param session:
    """
    event = await EventsAPI.get_event(create_data.event_id)
    if create_data.size > event["teamsTemplate"]["maxLen"] or create_data.size < event["teamsTemplate"]["minLen"]:
        raise CustomException(status_code=415, reason="Количество участников команды не соотвествует паттерну.")

    required = event["teamsTemplate"]["required"].split(";")
    if any(stack not in create_data.need for stack in required):
        raise CustomException(status_code=415, reason="Неверный стэк команды.")

    try:

        team = Teams(
            name=create_data.name,
            event_id=create_data.event_id,
            author_id=create_data.author_id,
            description=create_data.description,
            need=create_data.need,
            size=create_data.size
        )
        await db.create_team(session, team)
        team = await db.get_team_by_name(session, create_data.name)
        member = TeamsMembers(
            user_id=create_data.author_id,
            team_id=team.id
        )
        await db.create_member(session, member)
    except Exception as error:
        raise CustomException(status_code=409, reason="Команда с таким названием уже создана.")


@teams.delete(prefix + "remove", status_code=204)
async def delete_team(team_id: int, session=Depends(get_session)):
    """
    Handler for deleting team
    :param team_id:
    :param session:
    """
    team = await db.get_team(session, team_id)
    if team is None:
        raise CustomException(status_code=404, reason="Данная команда не найдена.")

    await db.delete_team_members(session, team_id)
    await db.delete_team_tags(session, team_id)
    await db.delete_team(session, team_id)


@teams.patch(prefix + "update", status_code=200)
async def update_team(update_data: UpdateTeam, session=Depends(get_session)):
    """
    Handler for updating team
    :param update_data:
    :param session:
    """
    if update_data.name is not None:
        await db.update_team_name(session, update_data.team_id, update_data.name)
    if update_data.description is not None:
        await db.update_team_description(session, update_data.team_id, update_data.description)
    if update_data.size is not None:
        await db.update_team_size(session, update_data.team_id, update_data.size)
    if update_data.need is not None:
        await db.update_team_need(session, update_data.team_id, update_data.need)


@teams.post(prefix + "createTag", status_code=201)
async def add_tag(tag_data: UpdateTag, session=Depends(get_session)):
    """
    Hanlder for adding tag to team
    :param tag_data:
    :param session:
    """
    tags = set(tag.tag for tag in await db.get_team_tags(session, tag_data.team_id))

    if tag_data.tag in tags:
        raise CustomException(status_code=409, reason="Данный тэг уже есть.")

    tag = TeamsTags(team_id=tag_data.team_id, tag=tag_data.tag)
    await db.create_tag(session, tag)


@teams.delete(prefix + "deleteTag", status_code=204)
async def delete_tag(team_id: int, tag: str, session=Depends(get_session)):
    """
    Handler for deleting tag from team
    :param tag:
    :param team_id:
    :param session:
    """
    tags = set(tag.tag for tag in await db.get_team_tags(session, team_id))

    if tag not in tags:
        raise CustomException(status_code=409, reason="Данный тэг не найден.")

    await db.delete_tag(session, team_id, tag)


@teams.post(prefix + "createInvite", status_code=201)
async def send_invite(invite_data: InviteTeam, session=Depends(get_session)):
    """
    Hanlder for sending invites
    :param invite_data:
    :param session:
    """
    curr_team = await db.get_team(session, invite_data.team_id)
    teams_array = await db.get_teams_by_event(session, curr_team.event_id)
    members = list(await db.get_team_members(session, curr_team.id))

    for team in teams_array:
        if invite_data.user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            raise CustomException(status_code=409,
                                  reason="Данный участник уже участвует в данном событии в составе другой команды.")

    if len(members) >= curr_team.size:
        raise CustomException(status_code=403, reason="На данный момент команда укомплектована.")

    invite = TeamsInvites(user_id=invite_data.user_id,
                          team_id=invite_data.team_id,
                          from_team=invite_data.from_team)
    mirror_invite = await db.get_mirror_invite(session, invite)

    if mirror_invite is None:
        await db.create_invite(session, invite)
    else:
        members_stack = [(await UsersAPI.get_user(member.user_id))["role"] for member in members]
        counter_stack = Counter(members_stack)
        nulls_keys = {key for key in counter_stack.keys() if counter_stack[key] == 0}
        delta = curr_team.size - sum(counter_stack.values())

        user = await UsersAPI.get_user(invite.user_id)

        if delta == len(nulls_keys) and user["role"] not in nulls_keys:
            raise CustomException(status_code=415, reason="Ваш стэк не подходит под данную команду.")

        member = TeamsMembers(user_id=invite_data.user_id,
                              team_id=invite_data.team_id)
        await db.create_member(session, member)
        await db.delete_invite(session, mirror_invite)


@teams.get(prefix + "invites", status_code=200, response_model=List[InviteTeamFull])
async def get_invites(user_id: int, event_id: int, session=Depends(get_session)):
    """
    Handler for getting invites
    :param event_id:
    :param user_id:
    :param session:
    :return:
    """
    teams_array = await db.get_teams_by_event(session, event_id)
    inTeam = "solo"
    team_id = None
    for team in teams_array:
        if user_id in set(
                team_members.user_id for team_members in
                await db.get_team_members(session, team.id)) and team.author_id == user_id:
            team_id = team.id
            inTeam = "author"
            break
        elif user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            inTeam = "member"
            break
    match inTeam:
        case "solo":
            result = await db.get_solo_invites(session, user_id, event_id)
        case "author":
            result = ((await db.get_author_invites(session, team_id, event_id)) + (
                await db.get_solo_invites(session, user_id,
                                          event_id)))
        case "member":
            result = await db.get_solo_invites(session, user_id, event_id)
        case _:
            result = await db.get_solo_invites(session, user_id, event_id)
    result = [InviteTeamFull(id=invite.id,
                             user_id=invite.user_id,
                             team_id=invite.team_id,
                             from_team=invite.from_team) for invite in result]
    return result


@teams.post(prefix + "answerInvite", status_code=201)
async def answer_invite(answer_data: AnswerInvite, session=Depends(get_session)):
    """
    Handler for answering invites
    :param answer_data:
    :param session:
    :return:
    """
    invite = await db.get_invite(session, answer_data.invite_id)
    if answer_data.isAccepted:
        team = await db.get_team(session, invite.team_id)

        members = list(await db.get_team_members(session, invite.team_id))
        if len(members) >= team.size:
            raise CustomException(status_code=415, reason="На данный момент команда укомплектована.")

        members_stack = [(await UsersAPI.get_user(member.user_id))["role"] for member in members]
        counter_stack = Counter(members_stack)
        nulls_keys = {key for key in counter_stack.keys() if counter_stack[key] == 0}
        delta = team.size - sum(counter_stack.values())

        user = await UsersAPI.get_user(invite.user_id)

        if delta == len(nulls_keys) and user["role"] not in nulls_keys:
            raise CustomException(status_code=415, reason="Ваш стэк не подходит под данную команду.")

        member = TeamsMembers(user_id=invite.user_id, team_id=invite.team_id)
        await db.create_member(session, member)
    await db.delete_invite(session, invite)


@teams.get(prefix + "possibleMembers", status_code=200)
async def get_possible_members(team_id: int, event_id: int, session=Depends(get_session),
                               offset: Optional[int] = Query(0)):
    """
    Handler for getting possible members by team and event
    :param team_id:
    :param event_id:
    :param session:
    :param offset:
    """
    response = list()

    team_needs = (await db.get_team(session, team_id)).need
    team_members = list(await db.get_team_members(session, team_id))
    team_tags = get_tags(list(await db.get_team_tags(session, team_id)))
    users_array = [user for user in await EventsAPI.get_users_event(event_id, offset) if user not in team_members]
    users_role = {user: (await UsersAPI.get_user(user))["role"] for user in users_array}
    users_tags = {user: (await UsersAPI.get_user(user))["tags"] for user in users_array}
    users_array = [user for user in users_array if users_role[user] in team_needs]
    users_array = sorted(users_array, key=lambda current_user: get_Levenshtein_distance(team_tags,
                                                                                        users_tags[current_user]),
                         reverse=True)

    for user in users_array:
        response.append(await UsersAPI.get_user(user))
    return response


@teams.get(prefix + "possibleTeams", status_code=200, response_model=List[Team])
async def get_possible_teams(user_id: int, event_id: int, session=Depends(get_session),
                             offset: Optional[int] = Query(0)):
    """
    Hanlder for getting possible teams by user and event
    :param event_id:
    :param user_id:
    :param session:
    :param offset:
    :return:
    """
    response = list()
    teams_array = await db.get_teams_by_event(session, event_id)

    user_data = await UsersAPI.get_user(user_id)
    if user_data is None:
        return response
    teams_tags = {int(team.id): get_tags(list(await db.get_team_tags(session, team.id))) for team in teams_array}
    if not (await db.get_possible_teams(session, offset, event_id, user_data["role"])):
        return response

    teams_array = sorted(await db.get_possible_teams(session, offset, event_id, user_data["role"]),
                         key=lambda current_team: get_Levenshtein_distance(user_data["tags"],
                                                                           teams_tags[current_team.id]), reverse=True)
    for team in teams_array:
        current_tags = list(team_tag.tag for team_tag in await db.get_team_tags(session, team.id))
        current_members = list(team_members.user_id for team_members in await db.get_team_members(session, team.id))
        if len(current_members) < team.size:
            current_team = Team(
                id=team.id, name=team.name,
                author_id=team.author_id, event_id=team.event_id,
                description=team.description, tags=current_tags,
                members=list(set(current_members + [team.author_id])), need=team.need, size=team.size
            )
            response.append(current_team)

    return response


@teams.post(prefix + "myTeam", status_code=200, response_model=Team)
async def get_my_team(user_data: GetMyTeam, session=Depends(get_session)):
    """
    Handler for getting user's team by user_id and event_id
    :param user_data:
    :param session:
    :return:
    """
    event_teams = await db.get_teams_by_event(session, user_data.event_id)
    for team in event_teams:
        current_tags = list(team_tag.tag for team_tag in await db.get_team_tags(session, team.id))
        current_members = list(team_members.user_id for team_members in await db.get_team_members(session, team.id))
        if user_data.user_id in current_members + [team.author_id]:
            current_team = Team(
                id=team.id, name=team.name,
                author_id=team.author_id, event_id=team.event_id,
                description=team.description, tags=current_tags,
                members=list(set(current_members + [team.author_id])), need=team.need, size=team.size
            )
            return current_team
    raise CustomException(status_code=404, reason="У пользователя нет команды в данной олимпиаде.")


@teams.delete(prefix + "deleteMember", status_code=204)
async def delete_member(team_id: int, user_id: int, session=Depends(get_session)):
    """
    Handler for deleting member by team_id and user_id
    :param team_id:
    :param user_id:
    :param session:
    """
    await db.delete_member(session, user_id, team_id)


@teams.post(prefix + "withoutTeam", status_code=200, response_model=WithoutUsers)
async def get_users_without_team(without_data: WithoutTeam, session=Depends(get_session)):
    """
    Handler for getting users without team
    :param without_data:
    :param session:
    :return:
    """
    event_users = set(without_data.users)
    event_teams = await db.get_teams_by_event(session, without_data.event_id)
    for team in event_teams:
        members = await db.get_team_members(session, team.id)
        for member in members:
            event_users.discard(member.user_id)
        event_users.discard(team.author_id)
    return WithoutUsers(users=list(event_users))


@teams.post(prefix + "autogenerateTeam", status_code=201)
async def autogenerate_team(team_data: AutogenerateTeam, session=Depends(get_session)):
    """
    Handler for getting autogeneration teams
    :param team_data:
    :param session:
    """
    team = Teams(
        author_id=team_data.author_id,
        name=team_data.name,
        event_id=team_data.event_id,
        size=len(team_data.members) + 1,
        description="Это команда сгенерирована автоматически.",
        need=list()
    )
    await db.create_team(session, team)
    new_team = await db.get_team_by_name(session, team_data.name)
    new_team_id = new_team.id
    for member in team_data.members:
        member = TeamsMembers(
            user_id=member,
            team_id=new_team_id
        )
        await db.create_member(session, member)


@teams.exception_handler(CustomException)
async def custom_exception_handler(request: Request, exc: CustomException):
    """
    Exception handler for CustomException
    :param request:
    :param exc:
    :return:
    """
    return JSONResponse(status_code=exc.status_code,
                        content=jsonable_encoder(Status(status=exc.reason)))


if __name__ == "__main__":
    uvicorn.run(teams, host="0.0.0.0", port=80)
