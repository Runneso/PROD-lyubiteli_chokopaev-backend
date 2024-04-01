using System.ComponentModel.DataAnnotations.Schema;

namespace Events.Internal.Storage.Entities 
{
    public class EventsUsers 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EventId { get; set; }
        public long? UserId { get; set; }
        public bool IsJoin { get; set; }
        public string? Tg { get; set; }
    }
}