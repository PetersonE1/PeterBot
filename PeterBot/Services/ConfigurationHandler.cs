using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Services
{
    public class ConfigurationHandler
    {
        public Configuration Configuration { get; private set; }

        public ConfigurationHandler(string filePath)
        {
            string data = File.ReadAllText(filePath);
            Configuration = JsonConvert.DeserializeObject<Configuration>(data) ?? new Configuration();
        }
    }

    public class Configuration
    {
        public string Token { get; set; }
        public string[] Commands { get; set; }
    }
}
