using Gateway.Internal.Dto;

namespace Gateway.Internal.Interfaces 
{
    public interface IFilesService 
    {
        Task<FilesResult> UploadUserFile(IFormFile file);
    }
}