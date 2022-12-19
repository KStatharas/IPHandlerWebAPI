using IPInfoProvider.Services;
using System.Collections.Concurrent;

namespace IPHandlerWebAPI.Jobs
{
    public class JobDTO
    {
        public Guid JobId { get; set; }
        public String Status
        {
            get
            {
                if (QueuedIps.Count == 0 && Results.Count == 0) return $"Initialized";
                else if (QueuedIps.Count == 0) return $"Finished";
                else return $"Processing";
            }
        }
        public int[] Progress { get; set; } = new int[4];
        public Queue<IP> QueuedIps { get; set; }
        public Dictionary<Guid, bool> Results { get; set; }
        public String Message
        {
            get
            {
                if (Results.Count == 0)
                    return $"Job with ID '{JobId}' has just been initialized";

                else if (QueuedIps.Count == 0)
                    return $"Job with ID '{JobId}' has finished. Items inserted: {Progress[0]}/{Progress[1]} | Succeeded: {Progress[2]} | Failed: {Progress[3]}";

                else
                    return $"Job with ID '{JobId}' is in progress. Items inserted: {Progress[0]}/{Progress[1]} | Succeeded: {Progress[2]} | Failed: {Progress[3]}";
            }
        }


    }
}
