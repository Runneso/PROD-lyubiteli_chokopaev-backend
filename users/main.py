import uvicorn
from fastapi import FastAPI

from routes import router


app = FastAPI()


app.include_router(
    router,
    prefix="/users",
    tags=["users"],
)


if __name__ == "__main__":
    uvicorn.run("main:app")
