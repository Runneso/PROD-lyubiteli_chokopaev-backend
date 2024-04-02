namespace Gateway.Internal.Dto 
{
    public class CreateUserDto 
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string? patronymic { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public IFormFile? photoFile { get; set; }
        public string tg_username { get; set; }
        public bool is_admin { get; set; }
        public string? role { get; set; }
        public string[]? langs { get; set; }
        public string[]? tags { get; set; }
    }
}