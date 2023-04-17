using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Models.ResponseModels
{
    //This is the base response model that includes the two
    //properties that is included in nearly all responses
    //from the porkbun API. https://porkbun.com/api/json/v3/documentation
    public class ResponseModel
    {
        public string status { get; set; } = "Unknown";
        public string message { get; set; } = "There may have been an error or a response did not contain a status update.";
    }
}
