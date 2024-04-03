from settings import APIUrls, get_api_urls

from typing import Optional, Any

import aiohttp

api_urls: APIUrls = get_api_urls()
baseUrl = api_urls.EVENTS_API_URL


class EventsAPI:
    """
    Events API
    """

    @staticmethod
    async def get_event(event_id: int) -> Optional[dict[str, Any]]:
        url = f"{baseUrl}/events/{event_id}"
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                if response.status != 200:
                    return None
                json = await response.json()
                return json

    @staticmethod
    async def get_users_event(event_id: int) -> Optional[dict[str, Any]]:
        url = f"{baseUrl}/get_users/{event_id}"
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                if response.status != 200:
                    return None
                json = await response.json()
                return json
