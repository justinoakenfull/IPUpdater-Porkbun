using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Data
{
    public class Record
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public string ttl { get; set; }
        public string prio { get; set; }
        public string notes { get; set; }
    }
}
