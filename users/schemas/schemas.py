from typing import Optional

from pydantic import BaseModel, EmailStr, Field


class Token(BaseModel):
    access_token: str
    token_type: str


class CreateUser(BaseModel):
    name: str
    surname: str
    patronymic: Optional[str] = None
    email: EmailStr
    password: str = Field(pattern=r"[A-Za-z0-9@#$%^&+=]{8,}")
    photo: Optional[str] = None
    tg_username: str
    is_admin: bool = Field(default=False)
    role: Optional[str]
    tags: list[str] = []


class GetUser(BaseModel):
    id: int
    name: str
    surname: str
    patronymic: Optional[str] = None
    email: EmailStr
    photo: Optional[str] = None
    tg_username: str
    is_admin: bool
    role: Optional[str]
    tags: list[str] = []


class UpdateUser(BaseModel):
    name: Optional[str] = None
    surname: Optional[str] = None
    patronymic: Optional[str] = None
    photo: Optional[str] = None
    role: Optional[str] = None
    tg_username: Optional[str] = None
    tags: Optional[list[str]] = None
