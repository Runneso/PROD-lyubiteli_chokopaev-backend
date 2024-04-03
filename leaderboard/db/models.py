from .database import Base, engine

from typing import List

from sqlalchemy.orm import Mapped, mapped_column
from sqlalchemy import Text, BigInteger, Boolean
from sqlalchemy.dialects.postgresql import ARRAY


class Ratings(Base):
    """
    Rating SQL table
    """
    __tablename__ = "rating"
    user_id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    rating: Mapped[int] = mapped_column(BigInteger, nullable=False, default=1000)


async def create_db():
    """
    Main migration
    :return:
    """
    async with engine.begin() as connection:
        await connection.run_sync(Base.metadata.create_all)
    await engine.dispose()
