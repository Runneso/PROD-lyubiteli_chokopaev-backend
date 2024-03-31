from typing import AsyncGenerator

from sqlalchemy.engine import URL
from sqlalchemy.ext.asyncio import AsyncSession, async_sessionmaker, create_async_engine

from config import Config, load_config


def get_postgres_sessionmaker() -> async_sessionmaker[AsyncSession]:
    """
    Configure the postgres connection.
    """

    config: Config = load_config()

    postgres_url = URL.create(
        drivername=config.postgres.driver,
        username=config.postgres.user,
        password=config.postgres.password,
        host=config.postgres.host,
        port=config.postgres.port,
        database=config.postgres.database,
    )

    async_engine = create_async_engine(url=postgres_url)
    session_maker = async_sessionmaker(bind=async_engine, class_=AsyncSession)
    return session_maker


async def get_async_session() -> AsyncGenerator[AsyncSession, None]:
    async with get_postgres_sessionmaker()() as session:
        yield session
