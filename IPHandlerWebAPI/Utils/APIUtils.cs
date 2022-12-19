using IPHandlerWebAPI.Jobs;
using IPHandlerWebAPI.Models;
using IPInfoProvider.Services;
using System.Text.RegularExpressions;

namespace IPHandlerWebAPI.Utils
{
    /**
     * APIUtils contains a number of useful methods that are used accross the API.
     * One of those is IpIsValid() which ensures that an IP inserted by the user is
     * in the correct form.
     */ 
    public class APIUtils
    {
        public static bool IpIsValid(string ip)
        {
            string regExpress = @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(.(?!$)|$)){4}$";
            Regex ipRegx = new Regex(regExpress);

            return ipRegx.IsMatch(ip);
        }

        public static List<IP> AssignToIP(string[] ips, UpdateIP[] updateIps)
        {
            List<String> ipStrings = new List<String>(ips);
            Queue<UpdateIP> queueIps = new Queue<UpdateIP>(updateIps);
            
            List<IP> ipList = new List<IP>();

            ipStrings.ForEach(ip =>
            {
                var updateIp = queueIps.Dequeue();
                ipList.Add(new IP
                {
                    Ip = ip,
                    City = updateIp.City,
                    Continent = updateIp.Continent,
                    Country = updateIp.Country,
                    Latitude = updateIp.Latitude,
                    Longitude = updateIp.Longitude,

                });
            });

                return ipList;
        }

        public static OutputIP ConvertToOutputIP(IP ipObject)
        {

            OutputIP outputIP = new OutputIP
            {
                Ip = ipObject.Ip,
                City = ipObject.City,
                Country = ipObject.Country,
                Continent = ipObject.Continent,
                Latitude = ipObject.Latitude,
                Longitude = ipObject.Longitude

            };

            return outputIP;
        }

    }
}
