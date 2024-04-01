using Events.Internal.Storage.Entities;

namespace Events.Internal.Dto 
{
    public class EventDto 
    {
        public Event Event { get; set; }
        public Template? TeamsTemplate { get; set; }
    }
}