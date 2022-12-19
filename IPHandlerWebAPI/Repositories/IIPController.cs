using IPHandlerWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace IPHandlerWebAPI.Repositories
{
    public interface IIPController
    {
        public IActionResult GetIPs();
        public Task<IActionResult> GetIP(string ip);

        public Task<IActionResult> UpdateIPs(string[] ips, UpdateIP[] updateIPs);
    }
}
