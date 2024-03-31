from teams.config import Settings, get_settings

from sqlalchemy.ext.asyncio import create_async_engine, AsyncEngine
from sqlalchemy.orm import DeclarativeBase

settings: Settings = get_settings()
database_url: str = f"postgresql+asyncpg://{settings.POSTGRES_USERNAME}:{settings.POSTGRES_PASSWORD}@{settings.POSTGRES_HOST}:{settings.POSTGRES_PORT}/{settings.POSTGRES_DATABASE}"
engine: AsyncEngine = create_async_engine(database_url)


class Base(DeclarativeBase):
    pass
