using IPHandlerWebAPI.Data;
using IPHandlerWebAPI.Jobs;
using IPHandlerWebAPI.Models;
using IPHandlerWebAPI.Repositories;
using IPHandlerWebAPI.Utils;
using IPInfoProvider.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPHandlerWebAPI.Services
{
    /**
     * This is the API'S main service that handles user's input and provides it to the cache or database.
     * 
     */
    public class IPServiceImpl : IIPService
    {
        private readonly BackgroundJobs _backgroundJobs;
        private readonly IPHandlerWebApiDBContext dbContext;
        private readonly IIPRepository _retrievedIP;

        public IPServiceImpl(BackgroundJobs backgroundJobs, IPHandlerWebApiDBContext dbContext, IIPRepository retrievedIP)
        {
            _backgroundJobs = backgroundJobs;
            this.dbContext = dbContext;
            _retrievedIP = retrievedIP;
        }
        
        /**
         * GetIP calls GetCachedIP which checks if an IP exists in cache. If not it will try to get the information
         * by the database. If it doesn't exist there, IPStack (external API) will return all the details which will be saved
         * to the cache and database.
         */
        public async Task<OutputIP> GetIP(string ip)
        {
            var ipDetails = await _retrievedIP.GetCachedIP(ip);

            if (ipDetails == null)
            {
                IPInfoProviderService ipInfoProviderService = new IPInfoProviderService();
                ipDetails = ipInfoProviderService.GetDetails(ip);
                ipDetails.Id = Guid.NewGuid();

                await _retrievedIP.AddCachedIP(ipDetails);
            }

            OutputIP outputIp = new OutputIP
            {
                Ip = ipDetails.Ip,
                City = ipDetails.City,
                Country = ipDetails.Country,
                Continent = ipDetails.Continent,
                Latitude = ipDetails.Latitude,
                Longitude = ipDetails.Longitude
            };

            return outputIp;
        }

        /**
         * Returns all the IPs and their details that exist in the database.
         */
        public async Task<List<OutputIP>> GetIPs()
        {
            List<IP> ipList = await dbContext.IPs.ToListAsync();

            List<OutputIP> outList = ipList.Select(ip =>

            new OutputIP
            {
                Ip = ip.Ip,
                City = ip.City,
                Country = ip.Country,
                Continent = ip.Continent,
                Latitude = ip.Latitude,
                Longitude = ip.Longitude,
            }).ToList();


            return outList;
        }

 

        /**
         * UpdateIPs takes a list of IPs from the user and assigns it to the relative IPs that exist in the database.
         * The outcome of each operation is returned back.
         */
        public async Task<List<bool>> UpdateIPs(List<IP> ipObjects)
        {
            List<bool> results = new();

            foreach (IP ipObject in ipObjects)
            {
                try
                {
                    var ipDetails = await dbContext.IPs.FirstOrDefaultAsync(y => y.Ip == ipObject.Ip);

                    if (ipDetails == null) throw new IOException("One or more IPS are not valid");

                    ipDetails.Ip = ipObject.Ip;
                    ipDetails.Continent = ipObject.Continent;
                    ipDetails.Country = ipObject.Country;
                    ipDetails.City = ipObject.City;
                    ipDetails.Latitude = ipObject.Latitude;
                    ipDetails.Longitude = ipObject.Longitude;

                    results.Add(true);
                }
                catch (Exception e)
                {
                    results.Add(false);
                    throw e;
                }

            }
            await dbContext.SaveChangesAsync();

            return results;
        }

        /**
         * CreateJob creates a JobDTO and adds values to the singleton BackgroundJobs.cs
         * The job's id is assigned with a Guid which is returned to the user.
         */
        public async Task<JobDTO> CreateJob(string[] ips, UpdateIP[] updateIps)
        {

            List<IP> ipItems = APIUtils.AssignToIP(ips, updateIps);

            JobDTO job = new JobDTO
            {
                JobId = Guid.NewGuid(),
                QueuedIps = new(ipItems),
                Results = new(),
            };

            job.Progress[1] = job.QueuedIps.Count + job.Results.Count;

            _backgroundJobs.BackgroundTasks.Enqueue(job); 
            _backgroundJobs.TaskDictionary.TryAdd(job.JobId, job);

            return job;
        }

        /**
         * GetJobDetails receives a Guid from the user, searches the singleton's dictionary
         * and finally returns the job's status to the user.
         */ 

        public async Task<string> GetJobDetails(Guid id)
        {
            return _backgroundJobs.TaskDictionary[id].Message;
        }

    }
}
