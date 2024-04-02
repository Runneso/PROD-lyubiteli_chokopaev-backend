import uvicorn
from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles

from routes import router


app = FastAPI()
app.mount("/files", StaticFiles(directory="files"), name="files")


app.include_router(
    router,
    prefix="/api/v1/files",
    tags=["files"],
)


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=80)
