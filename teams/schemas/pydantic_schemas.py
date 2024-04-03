from typing import Optional, List

from pydantic import BaseModel, ConfigDict


class Status(BaseModel):
    status: Optional[str] = "<Status>"

    model_config = ConfigDict(from_attributes=True)


class Team(BaseModel):
    id: int
    author_id: int
    event_id: int
    name: str
    size: int
    description: Optional[str]
    need: List[str]
    tags: List[str]
    members: List[int]


class CreateTeam(BaseModel):
    author_id: int
    event_id: int
    name: str
    size: int
    description: Optional[str]
    need: List[str]


class DeleteTeam(BaseModel):
    team_id: int


class UpdateTeam(BaseModel):
    team_id: int
    name: Optional[str] = None
    description: Optional[str] = None
    size: Optional[int] = None
    need: Optional[List[str]] = None


class UpdateTag(BaseModel):
    team_id: int
    tag: str


class InviteTeam(BaseModel):
    team_id: int
    user_id: int
    from_team: bool


class InviteTeamFull(BaseModel):
    id: int
    team_id: int
    user_id: int
    from_team: bool


class GetInvites(BaseModel):
    user_id: int
    event_id: int


class WithoutTeam(BaseModel):
    users: List[int]
    event_id: int


class WithoutUsers(BaseModel):
    users: List[int]


class GetMyTeam(BaseModel):
    user_id: int
    event_id: int


class AnswerInvite(BaseModel):
    invite_id: int
    isAccepted: bool


class AutogenerateTeam(BaseModel):
    author_id: int
    event_id: int
    name: str
    members: List[int]
