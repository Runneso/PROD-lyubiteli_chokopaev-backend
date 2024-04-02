from typing import Annotated

from sqlalchemy.ext.asyncio import AsyncSession
from fastapi import APIRouter, status, Depends
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm

from database import CRUD, get_async_session
from schemas import *


router = APIRouter()
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/v1/users/sign_in")


@router.post(
    "/create_user",
    status_code=status.HTTP_201_CREATED,
    response_model=Token,
)
async def create_user(
    data: CreateUser,
    postgres: AsyncSession = Depends(get_async_session),
):
    token = await CRUD.create_user_db(data, postgres)
    return {"access_token": token, "token_type": "bearer"}


@router.post(
    "/sign_in",
    status_code=status.HTTP_200_OK,
    response_model=Token,
)
async def sign_in(
    data: Annotated[OAuth2PasswordRequestForm, Depends()],
    postgres: AsyncSession = Depends(get_async_session),
):
    token = await CRUD.sign_in_db(data, postgres)
    return {"access_token": token, "token_type": "bearer"}


@router.get(
    "/get_me",
    status_code=status.HTTP_200_OK,
    response_model=GetUser,
)
async def get_me(
    token: Annotated[str, Depends(oauth2_scheme)],
    postgres: AsyncSession = Depends(get_async_session),
):
    result = await CRUD.auth(token, postgres)
    return result


@router.get(
    "/get_user/{user_id}",
    status_code=status.HTTP_200_OK,
    response_model=GetUser,
)
async def get_user(
    user_id: int,
    postgres: AsyncSession = Depends(get_async_session),
):
    result = await CRUD.get_user_db(user_id, postgres)
    return result


@router.get(
    "/get_user_by_tg/{tg_username}",
    status_code=status.HTTP_200_OK,
    response_model=GetUser,
)
async def get_user_by_tg(
    tg_username: str,
    postgres: AsyncSession = Depends(get_async_session),
):
    result = await CRUD.get_user_by_tg_db(tg_username, postgres)
    return result


@router.patch(
    "/patch_me",
    status_code=status.HTTP_200_OK,
    response_model=GetUser,
)
async def patch_me(
    token: Annotated[str, Depends(oauth2_scheme)],
    data: UpdateUser,
    postgres: AsyncSession = Depends(get_async_session),
):
    result = await CRUD.patch_user_db(token, data, postgres)
    return result


@router.delete(
    "/delete_me",
    status_code=status.HTTP_204_NO_CONTENT,
)
async def delete_me(
    token: Annotated[str, Depends(oauth2_scheme)],
    postgres: AsyncSession = Depends(get_async_session),
):
    await CRUD.delete_user_db(token, postgres)
