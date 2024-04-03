from os import getenv
from functools import lru_cache
from dotenv import load_dotenv

load_dotenv()


class Settings:
    POSTGRES_DRIVER: str = getenv("POSTGRES_DRIVER")
    POSTGRES_USER: str = getenv("POSTGRES_USER")
    POSTGRES_PASSWORD: str = getenv("POSTGRES_PASSWORD")
    POSTGRES_HOST: str = getenv("POSTGRES_HOST")
    POSTGRES_PORT: str = getenv("POSTGRES_PORT")
    POSTGRES_DB: str = getenv("POSTGRES_DB")


class APIUrls:
    USERS_API_URL: str = getenv("USERS_API_URL")
    EVENTS_API_URL: str = getenv("EVENTS_API_URL")


@lru_cache
def get_settings() -> Settings:
    return Settings()


@lru_cache
def get_api_urls() -> APIUrls:
    return APIUrls()
