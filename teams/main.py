from db import (CRUD,
                create_db,
                engine,
                Teams,
                TeamsMembers,
                TeamsTags,
                TeamsInvites)
from schemas import (Status,
                     Team,
                     DeleteTeam,
                     CreateTeam,
                     UpdateTeam,
                     UpdateTag,
                     InviteTeam,
                     PossibleTeam,
                     AnswerInvite,
                     GetInvites)
from settings import Settings, get_settings
from services import UsersAPI

from typing import List, Union, Optional
from contextlib import asynccontextmanager

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
            members=current_members, need=team.need, size=team.size
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
            members=current_members, need=team.need, size=team.size
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
            members=current_members, need=team.need, size=team.size
        )
        return current_team


@teams.post(prefix + "create", status_code=201)
async def create_team(create_data: CreateTeam, session=Depends(get_session)):
    """
    Handler for creating teams
    :param create_data:
    :param session:
    """
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
async def delete_team(remove_data: DeleteTeam, session=Depends(get_session)):
    """
    Handler for deleting team
    :param remove_data:
    :param session:
    """
    await db.delete_team(session, remove_data.team_id)
    await db.delete_team_tags(session, remove_data.team_id)
    await db.delete_team_members(session, remove_data.team_id)


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
async def delete_tag(tag_data: UpdateTag, session=Depends(get_session)):
    """
    Handler for deleting tag from team
    :param tag_data:
    :param session:
    """
    tags = set(tag.tag for tag in await db.get_team_tags(session, tag_data.team_id))

    if tag_data.tag not in tags:
        raise CustomException(status_code=409, reason="Данный тэг не найден.")

    await db.delete_tag(session, tag_data.team_id, tag_data.tag)


@teams.post(prefix + "createInvite", status_code=201)
async def send_invite(invite_data: InviteTeam, session=Depends(get_session)):
    """
    Hanlder for sending invites
    :param invite_data:
    :param session:
    """
    curr_team = await db.get_team(session, invite_data.team_id)
    teams_array = await db.get_teams_by_event(session, curr_team.event_id)

    for team in teams_array:
        if invite_data.user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            raise CustomException(status_code=409,
                                  reason="Данный участник уже участвует в данном событии в составе другой команды.")

    length_members = len(await db.get_team_members(session, curr_team.id))
    if length_members >= curr_team.size:
        raise CustomException(status_code=403, reason="На данный момент команда укомплектована.")

    invite = TeamsInvites(user_id=invite_data.user_id,
                          team_id=invite_data.team_id,
                          from_user=invite_data.from_team)
    mirror_invite = await db.get_mirror_invite(session, invite)

    if mirror_invite is None:
        await db.create_invite(session, invite)
    else:
        member = TeamsMembers(user_id=invite_data.user_id,
                              team_id=invite_data.team_id)
        await db.create_member(session, member)
        await db.delete_invite(session, invite)
        await db.delete_invite(session, mirror_invite)


@teams.get(prefix + "invites", status_code=200)
async def get_invites(invite_data: GetInvites, session=Depends(get_session)):
    """
    Handler for getting invites
    :param invite_data:
    :param session:
    :return:
    """
    teams_array = await db.get_teams_by_event(session, invite_data.event_id)
    inTeam = "solo"
    team_id = None
    for team in teams_array:
        if invite_data.user_id in set(
                team_members.user_id for team_members in
                await db.get_team_members(session, team.id)) and team.author_id == invite_data.user_id:
            team_id = team.id
            inTeam = "author"
            break
        elif invite_data.user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            inTeam = "member"
            break
    match inTeam:
        case "solo":
            return await db.get_solo_invites(session, invite_data.user_id, invite_data.event_id)
        case "authour":
            return await db.get_author_invites(session, team_id, invite_data.event_id)
        case "member":
            raise CustomException(status_code=404,
                                  reason="Вы состоите в команде, поэтому вы не можете просматривать приглашания.")


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
        length_members = len(list(await db.get_team_members(session, invite.team_id)))

        if length_members >= team.size:
            return CustomException(status_code=409, reason="На данный момент команда укомплектована.")

        member = TeamsMembers(user_id=invite.user_id, team_id=invite.team_id)
        await db.create_member(session, member)
    await db.delete_invite(session, invite)


@teams.get(prefix + "possibleTeams", status_code=200)
async def get_possible_teams(possible_data: PossibleTeam, session=Depends(get_session),
                             offset: Optional[int] = Query(0)):
    """
    Hanlder for getting possible teams by user and event
    :param possible_data:
    :param session:
    :param offset:
    :return:
    """
    response = list()
    teams_array = await db.get_teams_by_event(session, possible_data.event_id)
    for team in teams_array:
        if possible_data.user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            raise CustomException(status_code=409,
                                  reason="Данный участник уже участвует в данном событии в составе другой команды.")

    user_data = await UsersAPI.get_user(possible_data.user_id)
    teams_tags = {int(team.id): get_tags(list(await db.get_team_tags(session, team.id))) for team in teams_array}
    teams_array = sorted(await db.get_possible_teams(session, offset, possible_data.event_id, user_data["role"]),
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
                members=current_members, need=team.need, size=team.size
            )
            response.append(current_team)
    return response


@teams.exception_handler(CustomException)
async def custom_exception_handler(request: Request, exc: CustomException):
    """
    Exception hanlder for CustomException
    :param request:
    :param exc:
    :return:
    """
    return JSONResponse(status_code=exc.status_code,
                        content=jsonable_encoder(Status(status=exc.reason)))


if __name__ == "__main__":
    uvicorn.run(teams)
