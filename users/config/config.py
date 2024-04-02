from dataclasses import dataclass

from jwskate import Jwk
from environs import Env
from cashews import Cache, cache


__all__ = ["Config", "load_config"]


@dataclass
class AppConfig:
    admin_name: str
    admin_surname: str
    admin_patronymic: str
    admin_email: str
    admin_password: str
    admin_tg_username: str

    cache: Cache
    public_key: Jwk
    private_key: Jwk


@dataclass
class PostgresConfig:
    driver: str
    user: str
    password: str
    host: str
    port: int
    database: str


@dataclass
class Config:
    _instance = None

    def __new__(cls, *args, **kwargs):
        if not isinstance(cls._instance, cls):
            cls._instance = object.__new__(cls)
        return cls._instance

    app: AppConfig
    postgres: PostgresConfig


def load_config() -> Config:
    """
    Create the app config class.
    """

    env: Env = Env()
    env.read_env()
    redis_driver = env("REDIS_DRIVER")
    redis_host = env("REDIS_HOST")
    redis_port = env("REDIS_PORT")

    cache.setup(f"{redis_driver}://{redis_host}:{redis_port}/1", client_side=True)

    return Config(
        app=AppConfig(
            admin_name=env("ADMIN_NAME"),
            admin_surname=env("ADMIN_SURNAME"),
            admin_patronymic=env("ADMIN_PATRONYMIC"),
            admin_email=env("ADMIN_EMAIL"),
            admin_password=env("ADMIN_PASSWORD"),
            admin_tg_username=env("ADMIN_TG_USERNAME"),
            cache=cache,
            public_key=Jwk.from_json(env("PUBLIC_KEY")),
            private_key=Jwk.from_json(env("PRIVATE_KEY")),
        ),
        postgres=PostgresConfig(
            driver=env("POSTGRES_DRIVER"),
            user=env("POSTGRES_USER"),
            password=env("POSTGRES_PASSWORD"),
            host=env("POSTGRES_HOST"),
            port=int(env("POSTGRES_PORT")),
            database=env("POSTGRES_DB"),
        ),
    )
