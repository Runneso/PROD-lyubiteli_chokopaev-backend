namespace Gateway.Internal.Dto 
{
    public class UpdateDto
    {
        public string? name { get; set; }
        public string? surname { get; set; }
        public string? patronymic { get; set; }
        public string? photo { get; set; }
        public string? tg_username { get; set; }
        public string? role { get; set; }
        public string[]? langs { get; set; }
        public string[]? tags { get; set; }
    }
}