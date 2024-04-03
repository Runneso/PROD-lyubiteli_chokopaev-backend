namespace Gateway.Internal.Dto 
{
    public class TeamDto 
    {
        public int id { get; set; }
        public int author_id { get; set; }
        public int event_id { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string description { get; set; }
        public string[] need { get; set; }
        public string[] tags { get; set; }
        public List<ProfileDto> members { get; set; }
    }
}