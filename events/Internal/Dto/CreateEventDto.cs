namespace Events.Internal.Dto 
{
    public class CreateEventDto 
    {
        public int OrgId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public string? MinLenTeam { get; set; }

        public string? MaxLenTeam { get; set; }
        public string[]? Required { get; set; }
    }
}