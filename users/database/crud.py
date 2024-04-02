from datetime import timedelta, datetime as dt

from jwskate import Jwt
from fastapi import status, HTTPException
from fastapi.security import OAuth2PasswordRequestForm
from sqlalchemy.exc import IntegrityError
from sqlalchemy import select, update, delete
from sqlalchemy.ext.asyncio import AsyncSession
from werkzeug.security import check_password_hash, generate_password_hash

from config import Config, load_config
from schemas import *
from .models import *


config: Config = load_config()


class CRUD:
    @classmethod
    async def on_startup(cls, postgres: AsyncSession) -> None:
        admin = CreateUser(
            name=config.app.admin_name,
            surname=config.app.admin_surname,
            patronymic=config.app.admin_patronymic,
            email=config.app.admin_email,
            password=config.app.admin_password,
            tg_username=config.app.admin_tg_username,
            is_admin=True,
        )
        try:
            await cls.create_user_db(admin, postgres)
        except:
            pass

    @classmethod
    async def auth(cls, token: str, postgres: AsyncSession) -> dict[str, str]:
        try:
            jwt = Jwt(token)

            if (
                jwt.verify_signature(config.app.public_key, alg="ES256")
                and not jwt.is_expired()
            ):
                user = await postgres.get(User, jwt.id)

                if not user:
                    raise HTTPException(
                        status.HTTP_401_UNAUTHORIZED, detail="User not found"
                    )
                return user.columns_to_dict()
            else:
                raise HTTPException(
                    status.HTTP_401_UNAUTHORIZED, detail="Token is invalid"
                )
        except:
            raise HTTPException(status.HTTP_401_UNAUTHORIZED, detail="Token is invalid")

    @classmethod
    async def create_user_db(cls, data: CreateUser, postgres: AsyncSession) -> str:
        data.__dict__["hashed_password"] = generate_password_hash(data.password)
        del data.password

        try:
            new_user = User(**data.__dict__)
            postgres.add(new_user)
            await postgres.commit()
        except IntegrityError:
            raise HTTPException(status.HTTP_409_CONFLICT, detail="User already exists")

        created_user = (
            await postgres.execute(select(User).filter(User.email == data.email))
        ).scalar_one_or_none()
        claims = {
            "id": created_user.id,
            "exp": dt.now() + timedelta(days=30),
        }
        jwt = str(Jwt.sign(claims, key=config.app.private_key, alg="ES256"))
        return jwt

    @classmethod
    async def sign_in_db(
        cls, data: OAuth2PasswordRequestForm, postgres: AsyncSession
    ) -> str:
        user = (
            await postgres.execute(select(User).filter(User.email == data.username))
        ).scalar_one_or_none()

        if not user:
            raise HTTPException(status.HTTP_401_UNAUTHORIZED, detail="User not found")

        if check_password_hash(user.hashed_password, data.password):
            claims = {
                "id": user.id,
                "exp": dt.now() + timedelta(days=30),
            }
            jwt = str(Jwt.sign(claims, key=config.app.private_key, alg="ES256"))
        else:
            raise HTTPException(
                status.HTTP_401_UNAUTHORIZED, detail="Invalid credentials"
            )
        return jwt

    @classmethod
    async def get_user_db(cls, user_id: int, postgres: AsyncSession) -> dict[str, str]:
        user = await postgres.get(User, user_id)
        if not user:
            raise HTTPException(status.HTTP_404_NOT_FOUND, detail="User not found")
        return user.columns_to_dict()

    @classmethod
    async def get_user_by_tg_db(
        cls, tg_username: str, postgres: AsyncSession
    ) -> dict[str, str]:
        user = (
            await postgres.execute(select(User).filter(User.tg_username == tg_username))
        ).scalar_one_or_none()

        if not user:
            raise HTTPException(status.HTTP_404_NOT_FOUND, detail="User not found")
        return user.columns_to_dict()

    @classmethod
    async def patch_user_db(cls, token: str, data: UpdateUser, postgres: AsyncSession):
        user = await cls.auth(token, postgres)
        data_dict = data.model_dump()
        user_id = user["id"]

        updated_items = {
            key: value for key, value in data_dict.items() if value is not None
        }

        if updated_items:
            try:
                await postgres.execute(
                    update(User).filter(User.id == user_id).values(**updated_items)
                )
                await postgres.commit()
            except IntegrityError:
                raise HTTPException(
                    status.HTTP_409_CONFLICT,
                    detail="User with this credentials already exists",
                )

        result = await postgres.get(User, user_id)
        return result

    @classmethod
    async def delete_user_db(cls, token: str, postgres: AsyncSession):
        user = await cls.auth(token, postgres)
        await postgres.execute(delete(User).filter(User.id == user["id"]))
        await postgres.commit()
