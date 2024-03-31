namespace Events.Internal.Dto 
{
    public class CreateEventDto 
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public long OrganizerId { get; set; }
    }
}