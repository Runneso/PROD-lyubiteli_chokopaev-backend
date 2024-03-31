from teams.db import CRUD, create_db, engine
from teams.db.models import Teams, TeamsMembers, TeamsTags, TeamsInvites
from teams.schemas import Status, Team, DeleteTeam, CreateTeam, UpdateTeam, UpdateTag, InviteTeam, PossibleTeam
from teams.config import Settings, get_settings
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
    for router in []:
        app.include_router(router)
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
    print(fuzz.token_sort_ratio(user_tags, team_tags))
    return fuzz.token_sort_ratio(user_tags, team_tags)


def get_tags(tags: List[TeamsTags]):
    return [tag.tag for tag in tags]


@teams.get(prefix + "ping", status_code=200, response_model=Status)
async def ping():
    return Status(status="OK")


@teams.get(prefix + "teams", status_code=200, response_model=List[Team])
async def get_teams(session=Depends(get_session)):
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
    except Exception as error:
        raise CustomException(status_code=409, reason="Команда с таким названием уже создана.")


@teams.delete(prefix + "remove", status_code=204)
async def delete_team(remove_data: DeleteTeam, session=Depends(get_session)):
    await db.delete_team(session, remove_data.team_id)
    await db.delete_team_tags(session, remove_data.team_id)
    await db.delete_team_members(session, remove_data.team_id)


@teams.patch(prefix + "update", status_code=200)
async def update_team(update_data: UpdateTeam, session=Depends(get_session)):
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
    tags = set(tag.tag for tag in await db.get_team_tags(session, tag_data.team_id))

    if tag_data.tag in tags:
        raise CustomException(status_code=409, reason="Данный тэг уже есть.")

    tag = TeamsTags(team_id=tag_data.team_id, tag=tag_data.tag)
    await db.create_tag(session, tag)


@teams.delete(prefix + "deleteTag", status_code=204)
async def delete_tag(tag_data: UpdateTag, session=Depends(get_session)):
    tags = set(tag.tag for tag in await db.get_team_tags(session, tag_data.team_id))

    if tag_data.tag not in tags:
        raise CustomException(status_code=409, reason="Данный тэг не найден.")

    await db.delete_tag(session, tag_data.team_id, tag_data.tag)


@teams.post(prefix + "sendInvite", status_code=201)
async def send_invite(invite_data: InviteTeam, session=Depends(get_session)):
    team = await db.get_team(session, invite_data.team_id)
    teams_array = await db.get_teams_by_event(session, team.event_id)

    for team in teams_array:
        if invite_data.user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            raise CustomException(status_code=409,
                                  reason="Данный участник уже участвует в данном событии в составе другой команды.")
    invite = TeamsInvites(user_id=invite_data.user_id,
                          team_id=invite_data.team_id,
                          from_user=invite_data.from_team)
    mirror_invite = await db.get_mirror_invite(session, invite)

    if invite_data.from_team:
        if mirror_invite is None:
            # отправить инвайт человеку и уведомление о приглашении в команду
            pass
        else:
            # добавить в команду
    else:
        if mirror_invite is None:
            # отправить капитану команды инвайт и уведомить его
            pass
        else:
            # добавить в команду
            pass


# @teams.post(prefix + "join", status_code=201)
# async def join_team(join_data: JoinTeam, session=Depends(get_session)):
#     team = await db.get_team(session, join_data.team_id)
#     event_teams = await db.get_teams_by_event(session, team.event_id)
#     for team in event_teams:
#         if int(join_data.user_id) in set(
#                 team_members.user_id for team_members in await db.get_team_members(session, team.id)):
#             raise CustomException(status_code=409,
#                                   reason="Данный участник уже участвует в данном событии в составе другой команды.")
#     join = TeamsMembers(user_id=join_data.user_id,
#                         team_id=join_data.team_id)
#     await db.join_team(session, join)


@teams.get(prefix + "possibleTeams")
async def get_possible_teams(possible_data: PossibleTeam, session=Depends(get_session),
                             offset: Optional[int] = Query(0)):
    response = list()
    teams_array = await db.get_teams_by_event(session, possible_data.event_id)
    for team in teams_array:
        if possible_data.user_id in set(
                team_members.user_id for team_members in await db.get_team_members(session, team.id)):
            raise CustomException(status_code=409,
                                  reason="Данный участник уже участвует в данном событии в составе другой команды.")

    user_data = await UsersAPI.get_user(possible_data.user_id)
    teams_tags = {int(team.id): get_tags(await db.get_team_tags(session, team.id)) for team in teams_array}
    teams_array = sorted(await db.get_possible_teams(session, offset, possible_data.event_id, user_data["role"]),
                         key=lambda current_team: get_Levenshtein_distance(user_data["tags"],
                                                                           teams_tags[current_team.id]), reverse=True)
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


@teams.exception_handler(CustomException)
async def custom_exception_handler(request: Request, exc: CustomException):
    return JSONResponse(status_code=exc.status_code,
                        content=jsonable_encoder(Status(status=exc.reason)))


if __name__ == "__main__":
    uvicorn.run(teams, host=settings.SERVER_ADDRESS, port=settings.SERVER_PORT)
