"""initial

Revision ID: 6f78e0b8646b
Revises: 
Create Date: 2024-04-02 13:01:54.253865

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = '6f78e0b8646b'
down_revision: Union[str, None] = None
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    # ### commands auto generated by Alembic - please adjust! ###
    op.create_table('users',
    sa.Column('id', sa.BigInteger(), nullable=False),
    sa.Column('name', sa.VARCHAR(), nullable=False),
    sa.Column('surname', sa.VARCHAR(), nullable=False),
    sa.Column('patronymic', sa.VARCHAR(), nullable=True),
    sa.Column('email', sa.VARCHAR(), nullable=False),
    sa.Column('hashed_password', sa.VARCHAR(), nullable=False),
    sa.Column('photo', sa.VARCHAR(), nullable=True),
    sa.Column('tg_username', sa.VARCHAR(), nullable=False),
    sa.Column('is_admin', sa.Boolean(), nullable=True),
    sa.Column('role', sa.VARCHAR(), nullable=True),
    sa.Column('langs', sa.ARRAY(sa.VARCHAR()), nullable=True),
    sa.Column('tags', sa.ARRAY(sa.VARCHAR()), nullable=True),
    sa.PrimaryKeyConstraint('id'),
    sa.UniqueConstraint('email'),
    sa.UniqueConstraint('id'),
    sa.UniqueConstraint('tg_username')
    )
    # ### end Alembic commands ###


def downgrade() -> None:
    # ### commands auto generated by Alembic - please adjust! ###
    op.drop_table('users')
    # ### end Alembic commands ###