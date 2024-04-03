namespace Gateway.Internal.Dto 
{
    public class UpdateTeamDto 
    {
        public int team_id { get; set; }
        public string? name { get; set; }
        public int? size { get; set; }
        public string? description { get; set; }
        public string[]? need { get; set; }
    }
}