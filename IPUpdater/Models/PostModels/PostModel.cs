using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Models.PostModels
{
    //Base model used to post all requests to the porkbun API.
    //All post's made to the API require at a minimum:
    //API Key - Username(essentially)
    //Secret API Key - Password(essentially)
    //Please note: Not the actual account username and password.
    //Use the API key and secret generated: https://porkbun.com/account/api
    public class PostModel
    {
        public string apiKey { get; set; } = string.Empty;
        public string apiSecret { get; set; } = string.Empty;
    }
}
