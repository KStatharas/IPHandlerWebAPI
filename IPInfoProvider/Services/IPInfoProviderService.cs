using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPInfoProvider.Services
{
    /**
     * This is the library's main function.
     * It communicates with an external API named IPStack and receives a JSON
     * containing information about an IP. 
     * Check IIPDetails for what IP details are included.
     * JSONs are then converted to objects by using Newtonsoft.
     */ 
    public class IPInfoProviderService : IIPInfoProvider
    {
        public IP GetDetails(string ip)
        {
            using (WebClient wc = new WebClient())
            {
                //Using WebClient we manage to retrieve IPStack's JSON and pass it to a string
                string ipJson = wc.DownloadString($"http://api.ipstack.com/{ip}?access_key=5ecadc841773602eddfc4e119cb5e42a");
                IP ipDetails = JsonConvert.DeserializeObject<IP>(ipJson);

                //Dynamic object helps getting specific values from json
                dynamic json = JsonConvert.DeserializeObject(ipJson);
                ipDetails.Country = json.country_name;
                ipDetails.Continent = json.continent_name;

                return ipDetails;

            }

        }

 
    }
}
