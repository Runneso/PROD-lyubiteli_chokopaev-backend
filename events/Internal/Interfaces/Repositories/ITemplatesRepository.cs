using Events.Internal.Storage.Entities;

namespace Events.Internal.Interafces 
{
    public interface ITemplatesRepository
    {
        public Task AddTemplate(Template template);
        public Task<Template> GetTemplate(int eventId);
        public Task RmTemplate(Template template);
    }
}