using System.ComponentModel.DataAnnotations.Schema;

namespace Events.Internal.Storage.Entities 
{
    public class Organizer 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EventId { get; set; }
        public int OrgId { get; set; }
    }
}