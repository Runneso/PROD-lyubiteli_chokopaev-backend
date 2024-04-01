from sqlalchemy.orm import relationship
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import Column, VARCHAR, BigInteger, ForeignKey, Boolean


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
    is_admin = Column(Boolean)
    role = Column(VARCHAR)
    tag = relationship("Tag", back_populates="user")

    def __str__(self) -> str:
        return f"<User: {self.id}>"

    def columns_to_dict(self) -> dict:
        """
        Convert to dict to jsonify.
        """

        d = {key: getattr(self, key) for key in self.__mapper__.c.keys()}
        return d


class Tag(Base):
    __tablename__ = "tags"

    id = Column(BigInteger, unique=True, primary_key=True)
    user_id = Column(
        ForeignKey(User.id, ondelete="CASCADE"),
        nullable=False,
    )
    tag = Column(VARCHAR, nullable=False)
    user = relationship("User", back_populates="tag")

    def __str__(self) -> str:
        return f"<Tag: {self.tag}, User: {self.user_id}>"
