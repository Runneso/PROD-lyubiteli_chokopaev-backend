from .models import Teams, TeamsMembers, TeamsTags, TeamsInvites

from typing import Sequence, Optional, Any, List

from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select


class CRUD:
    async def get_teams(self, session: AsyncSession) -> Sequence:
        """
        Get all teams
        :param session:
        :return:
        """
        sql_query = select(Teams).order_by(Teams.id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_teams_by_event(self, session: AsyncSession, event_id: int) -> Sequence:
        """
        Get teams by event
        :param session:
        :param event_id:
        :return:
        """
        sql_query = select(Teams).filter(Teams.event_id == event_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_possible_teams(self, session: AsyncSession, offset: int, event_id: int, role: str) -> Sequence:
        """
        Get possible teams by user and event
        :param session:
        :param offset:
        :param event_id:
        :param role:
        :return:
        """
        sql_query = select(Teams).filter(Teams.event_id == event_id, Teams.need.contains([role])).offset(offset).limit(
            10).order_by(Teams.id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_team_tags(self, session: AsyncSession, team_id: int) -> Sequence:
        """
        Get team's tags by team
        :param session:
        :param team_id:
        :return:
        """
        sql_query = select(TeamsTags).filter(TeamsTags.team_id == team_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_team_members(self, session: AsyncSession, team_id: int) -> Sequence:
        """
        Get team's members by team
        :param session:
        :param team_id:
        :return:
        """
        sql_query = select(TeamsMembers).filter(TeamsMembers.team_id == team_id)
        result = await session.execute(sql_query)

        return result.scalars().all()

    async def get_team(self, session: AsyncSession, team_id: int) -> Optional[Teams]:
        """
        Get team by id
        :param session:
        :param team_id:
        :return:
        """
        sql_query = select(Teams).filter(Teams.id == team_id)
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def get_team_by_name(self, session: AsyncSession, name: str) -> Optional[Teams]:
        """
        Get team by name
        :param session:
        :param name:
        :return:
        """
        sql_query = select(Teams).filter(Teams.name == name)
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def get_invite(self, session: AsyncSession, invite_id: int) -> Optional[TeamsInvites]:
        """
        Get invite by id
        :param session:
        :param invite_id:
        :return:
        """
        sql_query = select(TeamsInvites).filter(TeamsInvites.id == invite_id)
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def get_mirror_invite(self, session: AsyncSession, invite: TeamsInvites) -> Optional[TeamsInvites]:
        """
        Check mirror invite
        :param session:
        :param invite:
        :return:
        """
        sql_query = select(TeamsInvites).filter(TeamsInvites.user_id == invite.user_id,
                                                TeamsInvites.team_id == invite.team_id,
                                                TeamsInvites.from_team == (not invite.from_team))
        result = await session.execute(sql_query)

        return result.scalars().first()

    async def get_solo_invites(self, session: AsyncSession, user_id: int, event_id: int) -> List:
        """
        Get solo invites by user and event
        :param session:
        :param user_id:
        :param event_id:
        :return:
        """
        sql_query = select(TeamsInvites).filter(TeamsInvites.user_id == user_id, TeamsInvites.from_team == True)
        result = await session.execute(sql_query)
        result = result.scalars().all()
        result = [invite for invite in result if (await self.get_team(session, invite.team_id)).event_id == event_id]
        return result

    async def get_author_invites(self, session: AsyncSession, team_id: int, event_id: int) -> List:
        """
        Get author invites by user and event
        :param session:
        :param team_id:
        :param event_id:
        :return:
        """
        sql_query = select(TeamsInvites).filter(TeamsInvites.team_id == team_id, TeamsInvites.from_team == False)
        result = await session.execute(sql_query)
        result = result.scalars().all()
        result = [invite for invite in result if (await self.get_team(session, invite.team_id)).event_id == event_id]
        return result

    async def get_invite_by_data(self, session: AsyncSession, user_id: int, team_id: int, from_team: bool):
        sql_query = select(TeamsInvites).filter(TeamsInvites.team_id == team_id,
                                                TeamsInvites.user_id == user_id,
                                                TeamsInvites.from_team == from_team)
        result = await session.execute(sql_query)
        result = result.scalars().all()
        return result

    async def create_invite(self, session: AsyncSession, invite: TeamsInvites) -> None:
        """
        Invite creation
        :param session:
        :param invite:
        """
        session.add(invite)
        await session.commit()

    async def create_team(self, session: AsyncSession, team: Teams) -> None:
        """
        Team creation
        :param session:
        :param team:
        """
        session.add(team)
        await session.commit()

    async def create_tag(self, session: AsyncSession, tag: TeamsTags) -> None:
        """
        Tag creation
        :param session:
        :param tag:
        """
        session.add(tag)
        await session.commit()

    async def create_member(self, session: AsyncSession, member: TeamsMembers) -> None:
        """
        Member creation
        :param session:
        :param member:
        """
        session.add(member)
        await session.commit()

    async def update_team_name(self, session: AsyncSession, team_id: int, new_name: str) -> None:
        """
        Update team's name
        :param session:
        :param team_id:
        :param new_name:
        """
        team = await self.get_team(session, team_id)
        team.name = new_name
        await session.commit()

    async def update_team_description(self, session: AsyncSession, team_id: int, new_description: str) -> None:
        """
        Update team's description
        :param session:
        :param team_id:
        :param new_description:
        """
        team = await self.get_team(session, team_id)
        team.description = new_description
        await session.commit()

    async def update_team_size(self, session: AsyncSession, team_id: int, new_size: int) -> None:
        """
        Update team's size
        :param session:
        :param team_id:
        :param new_size:
        """
        team = await self.get_team(session, team_id)
        team.size = new_size
        await session.commit()

    async def update_team_need(self, session: AsyncSession, team_id: int, new_need: List[str]) -> None:
        """
        Update team's need
        :param session:
        :param team_id:
        :param new_need:
        """
        team = await self.get_team(session, team_id)
        team.need = new_need
        await session.commit()

    async def delete_team(self, session: AsyncSession, team_id: int) -> None:
        """
        Team deletion
        :param session:
        :param team_id:
        """
        team = await self.get_team(session, team_id)
        await session.delete(team)
        await session.commit()

    async def delete_member(self, session: AsyncSession, user_id: id, team_id: int):
        sql_query = select(TeamsMembers).filter(TeamsMembers.user_id == user_id, TeamsMembers.team_id == team_id)
        result = await session.execute(sql_query)
        member = result.scalars().first()
        await session.delete(member)
        await session.commit()

    async def delete_team_tags(self, session: AsyncSession, team_id: int) -> None:
        """
        Team's tag deletion
        :param session:
        :param team_id:
        """
        team_tags = await self.get_team_tags(session, team_id)
        for tag in team_tags:
            await session.delete(tag)
            await session.commit()

    async def delete_team_members(self, session: AsyncSession, team_id: int) -> None:
        """
        Team's member clearing
        :param session:
        :param team_id:
        """
        team = (await self.get_team(session, team_id))

        team_members = list(await self.get_team_members(session, team_id))
        try:
            members = team_members + [team.author_id]
        except Exception as error:
            members = team_members
        for member in members:
            try:
                await session.delete(member)
            except Exception as error:
                continue
        await session.commit()

    async def delete_tag(self, session: AsyncSession, team_id: int, tag: str) -> None:
        """
        Team's tag deletion
        :param session:
        :param team_id:
        :param tag:
        """
        sql_query = select(TeamsTags).filter(TeamsTags.team_id == team_id, TeamsTags.tag.like(tag))
        result = await session.execute(sql_query)
        tag = result.scalars().first()
        await session.delete(tag)
        await session.commit()

    async def delete_invite(self, session: AsyncSession, invite_data: TeamsInvites) -> None:
        """
        Invite deletion
        :param session:
        :param invite_data:
        """
        await session.delete(invite_data)
        await session.commit()
