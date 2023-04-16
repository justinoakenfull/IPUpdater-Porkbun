using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Data
{
    public class PingRequest
    {
        public string status { get; set; } = "";
        public string yourIp { get; set; } = "";
        public string message { get; set; } = "";
    }
}
