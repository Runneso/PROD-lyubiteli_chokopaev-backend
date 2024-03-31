from typing import Optional, Any

import aiohttp

baseUrl = "https://2cllrl3g-8000.euw.devtunnels.ms/"


class UsersAPI:
    @staticmethod
    async def get_user(user_id: int) -> Optional[dict[str, Any]]:
        url = f"{baseUrl}/get_user/{user_id}"
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                if response.status != 200:
                    return None
                json = await response.json()
                return {
                    "role": json["role"],
                    "tags": json["tags"]
                }
