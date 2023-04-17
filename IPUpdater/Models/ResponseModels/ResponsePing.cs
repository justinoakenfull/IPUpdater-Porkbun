using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Models.ResponseModels
{
    public class ResponsePing : ResponseModel
    {
        //Authentication with the porkbun API conveniently includes our IP
        //that we can use to check if we need to update the DNS records or not.
        public ResponsePing() { }

        //Defaults to localhost just for safety. Rather set the ip to localhost than
        //a random IP I probably don't have anymore.
        public string yourip { get; set; } = "127.0.0.1";

    }
}
