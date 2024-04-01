namespace Events.Internal.Dto 
{
    public class UploadMembersDto 
    {
        public IFormFile file { get; set; }
        public int OrganizerId { get; set; }
    }
}