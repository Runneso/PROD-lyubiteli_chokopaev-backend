using System.ComponentModel.DataAnnotations.Schema;

namespace Events.Internal.Storage.Entities 
{
    public class Template 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EventId { get; set; }
        public int MinLen { get; set; }
        public int MaxLen { get; set; }
        public string Required { get; set; }
    }
}