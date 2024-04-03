from typing import Optional, List

from pydantic import BaseModel, ConfigDict


class Status(BaseModel):
    status: Optional[str] = "<Status>"

    model_config = ConfigDict(from_attributes=True)


class UpdateRating(BaseModel):
    place: int
    user_id: int


class UpdateData(BaseModel):
    event_id: int
    data: List[UpdateRating]
