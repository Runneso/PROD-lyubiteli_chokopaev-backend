from settings import Settings, get_settings

from typing import Any

from sqlalchemy.ext.asyncio import create_async_engine, AsyncEngine
from sqlalchemy.orm import declarative_base

settings: Settings = get_settings()
database_url: str = f"{settings.POSTGRES_DRIVER}://{settings.POSTGRES_USER}:{settings.POSTGRES_PASSWORD}@{settings.POSTGRES_HOST}:{settings.POSTGRES_PORT}/{settings.POSTGRES_DB}"
engine: AsyncEngine = create_async_engine(database_url)

Base: Any = declarative_base()
