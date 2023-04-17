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

        public List<PostEditModel> FilterDNSRecordsFromResponse(ResponseRetrieve responseRetrieve, string dnsType)
        {
            List<PostEditModel> filteredRecords = new List<PostEditModel>();

            foreach (var record in responseRetrieve.records)
            {
                if (record != null && 
                    record.type.ToLower().Equals(dnsType.ToLower()) && 
                    !record.content.Equals(currentIP))
                {
                    filteredRecords.Add(new PostEditModel
                    {
                        //Api keys
                        apikey = _userSettings.apiKey,
                        secretapikey = _userSettings.secretapikey,

                        //dns records

                        //For some reason keeping the domain name in the update json adds it in addition to what is existing. Not sure if I'm doing something wrong or
                        //just a weird quirk with their api. This ternary checks if whats its updating is a subdomain or not and either leaves is blank if its
                        //just the domain or keeps just the subdomain if it is one. 
                        name = record.name.Split('.')[0] + "." + record.name.Split('.')[1] == _userSettings.DomainToUpdate ? "" : record.name.Split('.')[0],
                        id = record.id,
                        type = _userSettings.RecordTypeToUpdate,
                        content = currentIP,
                        ttl = record.ttl,
                    });
                }
            }

            return filteredRecords;
        }

        public List<ResponseModel> UpdateDNSRecords(ResponseRetrieve allRecords)
        {
            var recordsToUpdate = FilterDNSRecordsFromResponse(allRecords, _userSettings.RecordTypeToUpdate);

            if (recordsToUpdate == null)
            {
                //Return a single response with a warning.
                return new List<ResponseModel> { new ResponseModel { status = "WARNING", message = "Records to update was null."} };
            }

            //Make sure we have the most updated IP before editting the DNS Records.
            UpdateIP();
            //List of servers responses.
            List<ResponseModel> responsesFromAPI = new List<ResponseModel>();
            foreach (var record in recordsToUpdate)
            {

                var _httpClient = CreateHttpClient($"{_userSettings.APIDNSEditURI}{_userSettings.DomainToUpdate}/{record.id}");
                var content = new StringContent(JsonConvert.SerializeObject(record), UTF8Encoding.UTF8, "application/json");
                var apiResponse = _httpClient.PostAsync(_httpClient.BaseAddress, content);
                var readContentResponse = apiResponse.Result.Content.ReadAsStringAsync().Result;
                var responseModel = JsonConvert.DeserializeObject<ResponseModel>(readContentResponse);
                if (responseModel != null)
                {
                    responseModel.message = responseModel.status == "SUCCESS" ? 
                                            $"DNS Record was successfully updated to {currentIP}." : 
                                            $"There was a problem updating {record.name}: {record.id}.";
                    responsesFromAPI.Add(responseModel);
                }
            }
            return responsesFromAPI;
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

        private void UpdateIP() { PingAPI(); }
    }
}
