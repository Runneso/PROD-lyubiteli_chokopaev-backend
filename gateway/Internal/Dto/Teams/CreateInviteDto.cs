namespace Gateway.Internal.Dto 
{
    public class CreateInviteDto 
    {
        public int team_id { get; set; }
        public int user_id { get; set; }
        public bool from_team { get; set; }
    }
}