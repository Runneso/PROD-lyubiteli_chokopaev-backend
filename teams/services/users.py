from settings import APIUrls, get_api_urls

from typing import Optional, Any

import aiohttp

api_urls: APIUrls = get_api_urls()
baseUrl = api_urls.USERS_API_URL


class UsersAPI:
    """
    Users API
    """

    @staticmethod
    async def get_user(user_id: int) -> Optional[dict[str, Any]]:
        url = f"{baseUrl}/get_user/{user_id}"
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                if response.status != 200:
                    return None
                json = await response.json()
                return json
