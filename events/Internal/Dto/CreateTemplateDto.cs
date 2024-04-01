namespace Events.Internal.Dto 
{
    public class CreateTemplateDto
    {
        public int OrgId { get; set; }
        public int MinLen { get; set; }
        public int MaxLen { get; set; }
        public string[] Required { get; set; }
    }
}