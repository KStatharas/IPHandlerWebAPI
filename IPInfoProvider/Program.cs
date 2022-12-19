using IPInfoProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPInfoProvider
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPInfoProviderService iPInfoProviderinno = new IPInfoProviderService();
            IP myip = iPInfoProviderinno.GetDetails("94.131.128.214");
        }
    }
}
