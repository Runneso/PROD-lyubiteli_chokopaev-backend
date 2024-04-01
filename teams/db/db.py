from teams.config import Settings, get_settings

from sqlalchemy.ext.asyncio import create_async_engine, AsyncEngine
from sqlalchemy.orm import DeclarativeBase

settings: Settings = get_settings()
database_url: str = f"{settings.POSTGRES_DRIVER}://{settings.POSTGRES_USER}:{settings.POSTGRES_PASSWORD}@{settings.POSTGRES_HOST}:{settings.POSTGRES_PORT}/{settings.POSTGRES_DB}"
engine: AsyncEngine = create_async_engine(database_url)


class Base(DeclarativeBase):
    pass
