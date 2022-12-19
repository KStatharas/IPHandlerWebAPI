using IPInfoProvider.Services;
using System.Collections.Concurrent;

namespace IPHandlerWebAPI.Jobs
{
    public class BackgroundJobs
    {
        public ConcurrentQueue<JobDTO> BackgroundTasks { get; set; } = new ConcurrentQueue<JobDTO>();
        public ConcurrentDictionary<Guid,JobDTO> TaskDictionary { get; set; } = new ConcurrentDictionary<Guid,JobDTO>();
    }
}
