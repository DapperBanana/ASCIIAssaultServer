using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ASCIIAssault_Server
{
    public class Utility
    {
        private static IConfiguration? _config;
        public static IConfiguration Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                        .Build();
                }
                return _config;
            }
        }
    }
}
