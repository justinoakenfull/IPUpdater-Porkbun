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
        private UserSettingsModel _userSettings;
        private string currentIP = "";
        private PostModel _postModel = new PostModel();
        
        public ResponsePing PingAPI()
        {
            //Create and get back the httpclient passing in the API endpoint we want to hit.
            var _httpClient = CreateHttpClient(_userSettings.APIPingURI);
            //Convert the post model object to json
            var content = new StringContent(JsonConvert.SerializeObject(_postModel), UTF8Encoding.UTF8, "application/json");
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
            CloseHttpClient(_httpClient);
            return pingResponseModel;
        }

        public ResponseRetrieve GetDNSRecords()
        {

            var _httpClient = CreateHttpClient($"{_userSettings.APIDNSRequestURI}{_userSettings.DomainToUpdate}");

            var content = new StringContent(JsonConvert.SerializeObject(_postModel), UTF8Encoding.UTF8, "application/json");
            var apiResponse = _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var readContentResponse = apiResponse.Result.Content.ReadAsStringAsync().Result;
            var dnsRetrieveResponseModel = JsonConvert.DeserializeObject<ResponseRetrieve>(readContentResponse);

            dnsRetrieveResponseModel ??= new ResponseRetrieve
            {
                status = "ERROR",
                message = "This error is not server produced. Error pinging API or JSON received was empty."
            };
            
            CloseHttpClient(_httpClient);
            return dnsRetrieveResponseModel;
        }

        public List<DNSRecordModel> FilterDNSRecordsFromResponse(ResponseRetrieve responseRetrieve, string dnsType)
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

        private HttpClient CreateHttpClient(string BaseAddress)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseAddress);

            return _httpClient;
        }

        private void CloseHttpClient(HttpClient client) { client.Dispose(); }
    }
}
