from .models import Ratings

from typing import Sequence, Optional, Any, List

from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select


class CRUD:
    async def get_ratings(self, session: AsyncSession) -> Sequence:
        sql_query = select(Ratings).order_by(Ratings.user_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_rating_by_user(self, session: AsyncSession, user_id: int):
        sql_query = select(Ratings).filter(Ratings.user_id == user_id)
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def create_ratings(self, session: AsyncSession, rating: Ratings) -> None:
        session.add(rating)
        await session.commit()

    async def update_rating(self, session: AsyncSession, user_id: int, delta: int) -> None:
        sql_query = select(Ratings).filter(Ratings.user_id == user_id)
        result = await session.execute(sql_query)
        rating = result.scalars().first()
        rating.rating += delta
        await session.commit()
