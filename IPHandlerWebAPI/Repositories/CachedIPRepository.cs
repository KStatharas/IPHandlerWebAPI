using IPHandlerWebAPI.Controllers;
using IPHandlerWebAPI.Data;
using IPHandlerWebAPI.Exceptions;
using IPHandlerWebAPI.Models;
using IPInfoProvider.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;

namespace IPHandlerWebAPI.Repositories
{
    /**
     * This is the Cache.
     * Cache overrides two methods from the IPRepository and creates entries
     * that can be later used to return IP information to the user.
     * 
     * Cache doesn't not intervene to the rest of the code and extra functionality
     * can be provided by only editing this file
     * 
     * When an IP or it's details are received, it will first check if they exist in cache. If they don't,
     * it will save them in a new entry and the user will try retrieve the info from the database.
     * If on the other hand they exist in cache, the user will just get them from it.
     */
    public class CachedIPRepository : IIPRepository
    {
        private readonly IPRepository _ipRepository;
        private readonly IMemoryCache _cache;

        public CachedIPRepository(IPRepository ipRepository, IMemoryCache cache)
        {
            _ipRepository = ipRepository;
            _cache = cache;
        }

        public async Task<IP> AddCachedIP(IP ipDetails)
        {
            string key = $"ip-{ipDetails.Ip}";

            return await _cache.GetOrCreate(
                key,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                    return await _ipRepository.AddCachedIP(ipDetails);
                })!;
        }

        public async Task<IP> GetCachedIP(string ip)
        {
            string key = $"ip-{ip}";
            var value = await _ipRepository.GetCachedIP(ip);
            if (value == null) return null;

            return await _cache.GetOrCreate(
                key,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                    return await _ipRepository.GetCachedIP(ip);
                })!;
        }

    }
}
