from teams.db.models import *
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select


class CRUD:
    async def get_teams(self, session: AsyncSession):
        sql_query = select(Teams).order_by(Teams.id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_teams_by_event(self, session: AsyncSession, event_id: int):
        sql_query = select(Teams).filter(Teams.event_id == event_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_possible_teams(self, session: AsyncSession, offset: int, event_id: int, role: str):
        sql_query = select(Teams).filter(Teams.event_id == event_id, Teams.need.contains([role])).offset(offset).limit(
            10).order_by(Teams.id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_team_tags(self, session: AsyncSession, team_id: int):
        sql_query = select(TeamsTags).filter(TeamsTags.team_id == team_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_team_members(self, session: AsyncSession, team_id: int):
        sql_query = select(TeamsMembers).filter(TeamsMembers.team_id == team_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_team(self, session: AsyncSession, team_id: int):
        sql_query = select(Teams).filter(Teams.id == team_id)
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def get_mirror_invite(self, session: AsyncSession, invite: TeamsInvites):
        sql_query = select(TeamsInvites).filter(TeamsInvites.user_id == invite.user_id,
                                                TeamsInvites.team_id == invite.team_id,
                                                TeamsInvites.from_team == (not invite.from_team))
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def create_team(self, session: AsyncSession, team: Teams):
        session.add(team)
        await session.commit()

    async def create_tag(self, session: AsyncSession, tag: TeamsTags):
        session.add(tag)
        await session.commit()

    async def update_team_name(self, session: AsyncSession, team_id: int, new_name: str):
        team = await self.get_team(session, team_id)
        team.name = new_name
        await session.commit()

    async def update_team_description(self, session: AsyncSession, team_id: int, new_description: str):
        team = await self.get_team(session, team_id)
        team.description = new_description
        await session.commit()

    async def update_team_size(self, session: AsyncSession, team_id: int, new_size: int):
        team = await self.get_team(session, team_id)
        team.size = new_size
        await session.commit()

    async def update_team_need(self, session: AsyncSession, team_id: int, new_need: List[str]):
        team = await self.get_team(session, team_id)
        team.need = new_need
        await session.commit()

    async def delete_team(self, session: AsyncSession, team_id: int):
        team = await  self.get_team(session, team_id)
        await session.delete(team)
        await session.commit()

    async def delete_team_tags(self, session: AsyncSession, team_id: int):
        team_tags = await self.get_team_tags(session, team_id)
        for tag in team_tags:
            await session.delete(tag)
            await session.commit()

    async def delete_team_members(self, session: AsyncSession, team_id: int):
        team_members = await self.get_team_members(session, team_id)
        for member in team_members:
            await session.delete(member)
            await session.commit()

    async def delete_tag(self, session: AsyncSession, team_id: int, tag: str):
        sql_query = select(TeamsTags).filter(TeamsTags.team_id == team_id, TeamsTags.tag.like(tag))
        result = await session.execute(sql_query)
        tag = result.scalars().first()
        await session.delete(tag)
        await session.commit()

    # async def join_team(self, session: AsyncSession, join: TeamsMembers):
    #     session.add(join)
    #     await session.commit()
