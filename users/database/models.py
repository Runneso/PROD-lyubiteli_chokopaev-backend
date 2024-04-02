from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import Column, VARCHAR, BigInteger, ARRAY, Boolean


Base = declarative_base()


class User(Base):
    __tablename__ = "users"

    id = Column(BigInteger, unique=True, primary_key=True)
    name = Column(VARCHAR, nullable=False)
    surname = Column(VARCHAR, nullable=False)
    patronymic = Column(VARCHAR)
    email = Column(VARCHAR, nullable=False, unique=True)
    hashed_password = Column(VARCHAR, nullable=False)
    photo = Column(VARCHAR)
    tg_username = Column(VARCHAR, nullable=False, unique=True)
    is_admin = Column(Boolean, default=False)
    role = Column(VARCHAR)
    langs = Column(ARRAY(VARCHAR), default=[])
    tags = Column(ARRAY(VARCHAR), default=[])

    def __str__(self) -> str:
        return f"<User: {self.id}>"

    def columns_to_dict(self) -> dict:
        """
        Convert to dict to jsonify.
        """

        d = {key: getattr(self, key) for key in self.__mapper__.c.keys()}
        return d
