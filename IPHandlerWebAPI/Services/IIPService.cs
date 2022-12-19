using IPHandlerWebAPI.Jobs;
using IPHandlerWebAPI.Models;
using IPInfoProvider.Services;
using Microsoft.AspNetCore.Mvc;

namespace IPHandlerWebAPI.Services
{
    public interface IIPService
    {
        Task<List<OutputIP>> GetIPs();
        Task<OutputIP> GetIP(string ip);
        Task<List<bool>> UpdateIPs(List<IP> ipObjects);
        Task<JobDTO> CreateJob(string[] ips, UpdateIP[] updateIps);
        Task<string> GetJobDetails(Guid id);
    }
}
