from datetime import timedelta, datetime as dt

from jwskate import Jwt
from cashews import Cache, cache
from fastapi import status, HTTPException
from sqlalchemy.exc import IntegrityError
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select, update, delete, func, and_
from fastapi.security import OAuth2PasswordRequestForm
from werkzeug.security import check_password_hash, generate_password_hash

from config import Config, load_config
from schemas import *
from .models import *


config: Config = load_config()
cache: Cache = config.app.cache


class CRUD:
    @classmethod
    async def auth(cls, token: str, postgres: AsyncSession) -> dict[str, str]:
        try:
            jwt = Jwt(token)

            if (
                jwt.verify_signature(config.app.public_key, alg="ES256")
                and not jwt.is_expired()
            ):
                cached = await cache.get(f"user-{jwt.id}")
                if cached:
                    return cached
                else:
                    user = await postgres.get(User, jwt.id)

                    if not user:
                        raise HTTPException(
                            status.HTTP_404_NOT_FOUND, detail="User not found"
                        )
                    return user.columns_to_dict()
            else:
                raise HTTPException(status.HTTP_401_UNAUTHORIZED, detail="Token is invalid")
        except:
            raise HTTPException(status.HTTP_401_UNAUTHORIZED, detail="Token is invalid")

    @classmethod
    async def create_user_db(cls, data: CreateUser, postgres: AsyncSession) -> str:
        tags = data.__dict__.pop("tags")
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
        await cache.delete(f"user-{created_user.id}")
        await cache.delete(f"user-{created_user.tg_username}-tg")

        for tag in tags:
            postgres.add(Tag(user_id=created_user.id, tag=tag))

        claims = {
            "id": created_user.id,
            "exp": dt.now() + timedelta(days=30),
        }
        jwt = str(Jwt.sign(claims, key=config.app.private_key, alg="ES256"))
        await postgres.commit()

        return jwt

    @classmethod
    async def sign_in_db(
        cls, data: OAuth2PasswordRequestForm, postgres: AsyncSession
    ) -> str:
        user = (
            await postgres.execute(select(User).filter(User.email == data.username))
        ).scalar_one_or_none()

        if not user:
            raise HTTPException(status.HTTP_404_NOT_FOUND, detail="User not found")

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
        cached = await cache.get(f"user-{user_id}")
        if cached:
            return cached
        else:
            user = await postgres.get(User, user_id)
            if not user:
                raise HTTPException(status.HTTP_404_NOT_FOUND, detail="User not found")
            await cache.set(f"user-{user_id}", user.columns_to_dict(), "90m")
            return user.columns_to_dict()

    @classmethod
    async def get_user_by_tg_db(cls, tg_username: str, postgres: AsyncSession) -> dict[str, str]:
        cached = await cache.get(f"user-{tg_username}-tg")
        if cached:
            return cached
        else:
            user = (
            await postgres.execute(select(User).filter(User.tg_username == tg_username))
        ).scalar_one_or_none()

        if not user:
            raise HTTPException(status.HTTP_404_NOT_FOUND, detail="User not found")
        await cache.set(f"user-{tg_username}-tg", user.columns_to_dict(), "90m")
        return user.columns_to_dict()

    @classmethod
    async def patch_user_db(cls, token: str, data: UpdateUser, postgres: AsyncSession):
        user = await cls.auth(token, postgres)
        data_dict = data.model_dump()
        user_id = user["id"]
        tg_user = user["tg_username"]
        tags = data_dict.get("tags")

        if tags:
            del data_dict["tags"]
            user_tags = [tag.tag for tag in (await postgres.execute(
                select(Tag).filter(Tag.user_id == user["id"])
            )).scalars().all()]

            for tag in tags:
                if tag not in user_tags:
                    postgres.add(Tag(user_id=user["id"], tag=tag))

            for tag in user_tags:
                if tag not in tags:
                    await postgres.execute(delete(Tag).filter(Tag.user_id == user["id"], Tag.tag == tag))

            await postgres.commit()

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
                raise HTTPException(status.HTTP_409_CONFLICT, detail="User with this credentials already exists")

            await cache.delete(f"user-{user_id}")
            await cache.delete(f"user-{tg_user}-tg")

        result = await postgres.get(User, user_id)
        tags = [tag.tag for tag in (await postgres.execute(select(Tag).filter(Tag.user_id == result.id))).scalars().all()]
        result.__dict__["tags"] = tags
        return result

    @classmethod
    async def delete_user_db(cls, token: str, postgres: AsyncSession):
        user = await cls.auth(token, postgres)
        await postgres.execute(delete(User).filter(User.id == user["id"]))
        await cache.delete(f"user-{user["id"]}")
        await cache.delete(f"user-{user["tg_username"]}-tg")
        await cache.delete(f"user-{user["tg_username"]}-exists")
        await postgres.commit()
