from pydantic import BaseModel


class FileInfo(BaseModel):
    name: str
    url: str
