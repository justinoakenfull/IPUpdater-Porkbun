using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Models.ResponseModels
{
    //This contains all the fields of a single record that is
    //included in the retrieve by domain response.
    public class DNSRecordModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public string ttl { get; set; }
        public string prio { get; set; }
        public string notes { get; set; }
    }
}
