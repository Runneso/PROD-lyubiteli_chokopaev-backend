import uvicorn
from fastapi import FastAPI

from routes import router


app = FastAPI()


app.include_router(
    router,
    prefix="/api/v1/files",
    tags=["files"],
)


if __name__ == "__main__":
    uvicorn.run(app)
