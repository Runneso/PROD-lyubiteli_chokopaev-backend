from teams.db.db import Base, engine

from typing import List

from sqlalchemy.orm import Mapped, mapped_column
from sqlalchemy import Text, BigInteger, Boolean
from sqlalchemy.dialects.postgresql import ARRAY


class Teams(Base):
    __tablename__ = "teams"
    id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    author_id: Mapped[int] = mapped_column(BigInteger, nullable=False)
    event_id: Mapped[int] = mapped_column(BigInteger, nullable=False)
    size: Mapped[int] = mapped_column(BigInteger, nullable=False)
    name: Mapped[str] = mapped_column(Text, nullable=False, unique=True)
    description: Mapped[str] = mapped_column(Text, nullable=True)
    need: Mapped[List[str]] = mapped_column(ARRAY(Text), nullable=False)


class TeamsTags(Base):
    __tablename__ = "team_tags"
    id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    team_id: Mapped[int] = mapped_column(BigInteger, nullable=False)
    tag: Mapped[str] = mapped_column(Text, nullable=False)


class TeamsMembers(Base):
    __tablename__ = "team_members"
    id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    team_id: Mapped[int] = mapped_column(BigInteger, nullable=False)
    user_id: Mapped[int] = mapped_column(BigInteger, nullable=False)


class TeamsInvites(Base):
    __tablename__ = "team_invites"
    id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    user_id: Mapped[int] = mapped_column(BigInteger, nullable=False)
    team_id: Mapped[int] = mapped_column(BigInteger, nullable=False)
    from_team: Mapped[bool] = mapped_column(Boolean, nullable=False)


async def create_db() -> None:
    async with engine.begin() as connection:
        await connection.run_sync(Base.metadata.create_all)
    await engine.dispose()
