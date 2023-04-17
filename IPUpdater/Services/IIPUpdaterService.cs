using IPUpdater.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Services
{
    public interface IIPUpdaterService
    {
        ResponsePing PingAPI();
        ResponseRetrieve GetDNSRecords();
        List<ResponseModel> UpdateDNSRecords(ResponseRetrieve allRecords);
    }
}
