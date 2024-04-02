import os
from uuid import uuid4

import aiofiles as aof
import aiofiles.os as aos
from environs import Env
from fastapi import APIRouter, status, UploadFile

from schemas import *


router = APIRouter()
base_path = os.getcwd()
env: Env = Env()
env.read_env()


@router.post(
    "/upload_file",
    status_code=status.HTTP_201_CREATED,
    response_model=FileInfo,
)
async def upload_file(upload_file: UploadFile):
    file_name = uuid4()
    path = f"{base_path}/files/{file_name}.{upload_file.filename.split(".")[1]}"

    while await aos.path.exists(path):
        file_name = uuid4()
        path = f"{base_path}/files/{file_name}.{upload_file.filename.split(".")[1]}"

    async with aof.open(path, "wb") as file:
        await file.write(await upload_file.read())
        url = f"{env("SERVER_IP")}/files/{file_name}.{upload_file.filename.split(".")[1]}"

    return {"url": url}
