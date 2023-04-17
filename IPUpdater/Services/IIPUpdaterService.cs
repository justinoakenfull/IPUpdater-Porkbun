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
        void GetDNSRecords();
        void UpdateDNSRecords();
    }
}
