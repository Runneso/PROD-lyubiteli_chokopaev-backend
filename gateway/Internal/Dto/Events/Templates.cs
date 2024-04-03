namespace Gateway.Internal.Dto 
{
    public class Templates 
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int MinLen { get; set; }
        public int MaxLen { get; set; }
        public string Required { get; set; }
    }
}