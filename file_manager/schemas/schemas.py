from pydantic import BaseModel


class FileInfo(BaseModel):
    url: str
