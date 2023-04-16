using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Data
{
    public class UpdateRecord : Record
    {
        public string secretapikey { get; set; }
        public string apikey { get; set; }
        public string status { get; set; }

    }
}
