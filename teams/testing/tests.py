from unittest import IsolatedAsyncioTestCase

import aiohttp

baseUrl = "http://127.0.0.1:8000/api/v1"


class Test(IsolatedAsyncioTestCase):
    async def testPing(self):
        url = f"{baseUrl}/ping"
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as response:
                self.assertEquals(response.status, 200)

    async def testCreationTeam(self):
        url = f"{baseUrl}/create"
        dataset = [
            {
                "author_id": 101,
                "event_id": 1,
                "name": "Python developers",
                "size": 5,
                "description": "Love Python",
                "need": ["backend"]
            },
            {
                "author_id": 102,
                "event_id": 1,
                "name": "Flutter developers",
                "size": 3,
                "description": "Love Flutter",
                "need": ["backend", "mobile"]
            },
            {
                "author_id": 103,
                "event_id": 1,
                "name": "JS developers",
                "size": 4,
                "description": "Love JS",
                "need": ["backend", "frontend"]
            }
        ]
        for data in dataset:
            async with aiohttp.ClientSession() as session:
                async with session.post(url, json=data) as response:
                    self.assertEquals(response.status, 201)
