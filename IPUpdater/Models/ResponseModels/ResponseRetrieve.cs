using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Models.ResponseModels
{
    public class ResponseRetrieve : ResponseModel
    {
        public List<DNSRecordModel> records { get; set; }
    }
}
