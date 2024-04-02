using System.ComponentModel.DataAnnotations.Schema;

namespace Events.Internal.Storage.Entities 
{
    public class Event 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public string? MembersListPath { get; set; }
        public string? ResultsPath { get; set; }
    }
}