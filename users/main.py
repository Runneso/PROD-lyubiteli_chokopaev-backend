from contextlib import asynccontextmanager

import uvicorn
from fastapi import FastAPI

from routes import router
from database import CRUD, get_postgres_sessionmaker


@asynccontextmanager
async def lifespan(app: FastAPI):
    async with get_postgres_sessionmaker()() as postgres:
        await CRUD.on_startup(postgres)
    yield


app = FastAPI(lifespan=lifespan)


app.include_router(
    router,
    prefix="/api/v1/users",
    tags=["users"],
)


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=80)
