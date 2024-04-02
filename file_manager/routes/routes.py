import os

import aiofiles as aof
from environs import Env
from fastapi import APIRouter, status, UploadFile

from schemas import *


router = APIRouter()
base_path = os.getcwd()
env: Env = Env()
env.read_env()


@router.post(
    "/users/upload_file",
    status_code=status.HTTP_201_CREATED,
    response_model=FileInfo,
)
async def upload_user_file(user_id: int, upload_file: UploadFile):
    file_name = f"users-{user_id}.{upload_file.filename.split(".")[-1]}"
    path = f"{base_path}/files/{file_name}"
    async with aof.open(path, "wb") as file:
        await file.write(await upload_file.read())
        url = f"{env("SERVER_IP")}/files/{file_name}"
    return {"name": file_name, "url": url}


@router.post(
    "/events/upload_file",
    status_code=status.HTTP_201_CREATED,
    response_model=FileInfo,
)
async def upload_event_file(event_id: int, upload_file: UploadFile):
    file_name = f"events-{event_id}.{upload_file.filename.split(".")[-1]}"
    path = f"{base_path}/files/{file_name}"
    async with aof.open(path, "wb") as file:
        await file.write(await upload_file.read())
        url = f"{env("SERVER_IP")}/files/{file_name}"
    return {"name": file_name, "url": url}



@router.post(
    "/admin/upload_file",
    status_code=status.HTTP_201_CREATED,
    response_model=FileInfo,
)
async def upload_admin_file(upload_file: UploadFile):
    file_name = upload_file.filename
    path = f"{base_path}/files/{file_name}"
    async with aof.open(path, "wb") as file:
        await file.write(await upload_file.read())
        url = f"{env("SERVER_IP")}/files/{file_name}"
    return {"name": file_name, "url": url}
