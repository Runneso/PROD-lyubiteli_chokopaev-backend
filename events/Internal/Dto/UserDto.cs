namespace Events.Internal.Dto 
{
    public record class UserDto 
    {
        public int id { get; set; }
        public string role { get; set; }
        public List<string> langs { get; set; }
        public string email { get; set; }
    }
}