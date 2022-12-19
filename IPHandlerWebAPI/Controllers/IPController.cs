using IPHandlerWebAPI.Data;
using IPHandlerWebAPI.Jobs;
using IPHandlerWebAPI.Models;
using IPHandlerWebAPI.Repositories;
using IPHandlerWebAPI.Services;
using IPHandlerWebAPI.Utils;
using IPInfoProvider.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace IPHandlerWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IPController : Controller

    {
        /**
         *  This the API'S main service dependency injection.
         *  The service is used for all of the entry points' main functionality.
         */
        private readonly IPServiceImpl _service;

        public IPController(IPServiceImpl service)
        {
            _service = service;
        }

        /**
         *  This entry point returns all the IPs that exist in the database.
        */
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetIPs()
        {
            return Ok(await _service.GetIPs());
        }

        /**
         *  This entry point returns the details of an IP that is given by the user in the url.
         *  In order to get them, the API'S Service will first check the Cache (CachedIPRepository.cs) and try to get the details and if they don't exist
         *  there, it will then search the Database. If there's not such registered IP in the Database, an external API (IPStack) will be called to
         *  provide all the essential info and then insert it in the Database and Cache. This function is provided by IPInfoProvider library.
         *  In the end, the IP details will be returned to the user.
        */
        [HttpGet]
        [Route("{ip}")]
        [EnableQuery]
        public async Task<IActionResult> GetIP([FromRoute] string ip)
        {
            if (!APIUtils.IpIsValid(ip)) return BadRequest("Invalid IP");

            return Ok(await _service.GetIP(ip));
        }

        /**
         *   This entry point allows the user to provide an array of IPS that should be updated along with their details in JSON format.
         *   Then a job or task will be created and an Guid will be returned to the user.
         * 
         */

        [HttpPut]
        public async Task<IActionResult> UpdateIPs([FromQuery] string[] ips, UpdateIP[] updateIPs)
        {
            if (ips == null || updateIPs == null || ips.Length != updateIPs.Length || ips.Length == 0)
                return BadRequest("One or more input fields are invalid.");

            foreach (string ip in ips)
            {
                if (!APIUtils.IpIsValid(ip)) return BadRequest("An invalid IP was inserted.");
            }

            JobDTO newJob = await _service.CreateJob(ips, updateIPs);

            return Ok(newJob.JobId);
        }

        /**
         *   The user provides a job's Guid in order to get information about the status of that job.
         *   A job's status can be either "Initialized","Processing" or "Finished".
         *   Check the JobDTO.cs for more details about a job's properties.
         * 
         */

        [HttpGet]
        [Route("job/{id}")]
        public async Task<IActionResult> GetJob([FromRoute] Guid id)
        {
            var jobDetails = _service.GetJobDetails(id);
            if (jobDetails == null) return BadRequest("This job doesn't exist.");
            else return Ok(jobDetails.Result);
        }

    }
}
