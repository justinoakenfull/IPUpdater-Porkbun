using IPUpdater.Models;
using IPUpdater.Models.PostModels;
using IPUpdater.Models.ResponseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Services
{
    internal class IPUpdaterService : IIPUpdaterService
    {
        private HttpClient _httpClient = new HttpClient();
        private UserSettingsModel _userSettings;
        private string currentIP = "";
        private PostModel _postModel = new PostModel();
        
        public ResponsePing PingAPI()
        {
            //Set the URI we are posting to
            _httpClient.BaseAddress = new Uri(_userSettings.APIPingURI);
            //Create a post model that will be used as the json content
            PostModel postModel = new PostModel{
                apikey = _userSettings.apiKey,
                secretapikey = _userSettings.secretapikey
            };
            //Convert the post model object to json
            var content = new StringContent(JsonConvert.SerializeObject(postModel), UTF8Encoding.UTF8, "application/json");
            //Post the json to the api
            var apiResponse = _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var readContentResponse = apiResponse.Result.Content.ReadAsStringAsync().Result;
            var pingResponseModel = JsonConvert.DeserializeObject<ResponsePing>(readContentResponse);
            //Now we have a response object, if something went wrong with the response, this is were we error handle.
            pingResponseModel ??= new ResponsePing
            {
                status = "ERROR",
                message = "This error is not server produced. Error pinging API or JSON received was empty."
            };
            if (pingResponseModel.yourip != null)
            {
                currentIP = pingResponseModel.yourip;
            }
            return pingResponseModel;
        }

        public ResponseRetrieve GetDNSRecords()
        {
            _httpClient.BaseAddress = new Uri(_userSettings.APIDNSRequestURI);

            PostModel postRequest = new PostModel();

            var content = new StringContent(JsonConvert.SerializeObject(postRequest), UTF8Encoding.UTF8, "application/json");
            var apiResponse = _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var readContentResponse = apiResponse.Result.Content.ReadAsStringAsync().Result;
            var dnsRetrieveResponseModel = JsonConvert.DeserializeObject<ResponseRetrieve>(readContentResponse);

            dnsRetrieveResponseModel ??= new ResponseRetrieve
            {
                status = "ERROR",
                message = "This error is not server produced. Error pinging API or JSON received was empty."
            };

            return dnsRetrieveResponseModel;
        }

        private List<DNSRecordModel> FilterDNSRecordsFromResponse(ResponseRetrieve responseRetrieve, string dnsType)
        {
            List<DNSRecordModel> filteredRecords = new List<DNSRecordModel>();

            foreach (var record in responseRetrieve.records)
            {
                if (record != null && 
                    record.type.ToLower().Equals(dnsType.ToLower()) && 
                    !record.content.Equals(currentIP))
                {
                    filteredRecords.Add(record);
                }
            }

            return filteredRecords;
        }

        public void UpdateDNSRecords()
        {
            throw new NotImplementedException();
        }

        public IPUpdaterService(UserSettingsModel userSettings)
        {
            _userSettings = userSettings;
            _postModel.apikey = _userSettings.apiKey;
            _postModel.secretapikey = _userSettings.secretapikey;
        }
    }
}
