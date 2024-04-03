from db import CRUD, create_db, engine, Ratings
from schemas import Status, UpdateData
from settings import Settings, get_settings
from services import EventsAPI

from contextlib import asynccontextmanager

import uvicorn
from fastapi.encoders import jsonable_encoder
from fastapi import FastAPI, Depends, Request
from fastapi.responses import JSONResponse
from sqlalchemy.ext.asyncio import async_sessionmaker


@asynccontextmanager
async def lifespan(app: FastAPI):
    print("Startup")
    await create_db()
    yield
    print("Shutdown")


leaderboard = FastAPI(lifespan=lifespan)

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


@leaderboard.get(prefix + "ping", status_code=200, response_model=Status)
async def ping():
    """
    Ping handler
    :return:
    """
    return Status(status="OK")


@leaderboard.post(prefix + "init/{user_id}", status_code=201)
async def init_rating(user_id: int, session=Depends(get_session)):
    """
    Handler for init rating by user_id
    :param user_id:
    :param session:
    :return:
    """
    rating = Ratings(user_id=user_id, rating=1000)
    await db.create_ratings(session, rating)


@leaderboard.patch(prefix + "update", status_code=200)
async def updata_rating(update_data: UpdateData, session=Depends(get_session)):
    """
    Handler for updating users' ratings
    :param update_data:
    :param session:
    :return:
    """
    event = await EventsAPI.get_event(update_data.event_id)
    tiers = {1: 2000, 2: 1500, 3: 1200}
    tier = tiers[event["tier"]]

    last_place = -float("inf")
    for data in update_data.data:
        last_place = max(last_place, data.place)

    for data in update_data.data:
        rating = (await db.get_rating_by_user(session, data.user_id)).rating
        coefficient = 1 - (rating / tier) if 1 - (rating / tier) >= 0 else 0
        place = 0.5 - (data.place / last_place)
        await db.update_rating(session, data.user_id, place * coefficient * 100)


@leaderboard.get(prefix + "leaderboard/{event_id}", status_code=200)
async def get_leaderboard(event_id: int, session=Depends(get_session)):
    """
    Handler for getting event's leaderboard
    :param event_id:
    :param session:
    """
    response = list()

    users = (await EventsAPI.get_users_event(event_id))["users"]
    for user in users:
        rating = await db.get_rating_by_user(session, user)
        response.append(rating)
    return sorted(response, key=lambda rating_data: rating_data.rating, reverse=True)


@leaderboard.exception_handler(CustomException)
async def custom_exception_handler(request: Request, exc: CustomException):
    """
    Exception handler for CustomException
    :param request:
    :param exc:
    :return:
    """
    return JSONResponse(
        status_code=exc.status_code, content=jsonable_encoder(Status(status=exc.reason))
    )


if __name__ == "__main__":
    uvicorn.run(leaderboard, host="0.0.0.0", port=80)
