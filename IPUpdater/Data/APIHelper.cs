using System.Text.Json;
using System.Net;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Text;
using System.Collections.ObjectModel;

namespace IPUpdater.Data
{
    public class APIHelper
    {
        public string DomainToUpdate { get; set; } = "";
        public string RecordTypeToUpdate { get; set; } = "";
        public string PorkbunPing { get; set; } = "";
        public string PorkbunDNSRequest { get; set; } = "";
        public string PorkbunDNSEdit { get; set; } = "";
        public string apikey { get; set; } = "";
        public string secretapikey { get; set; } = "";

        public APIHelper()
        {
            
            InitialiseUserConfig();
            //Ping porkbun and get our current IP
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(PorkbunPing);
            //Returns a PingRequest class with the status and ip
            var resultPingRequest = GetPing(client);

            if (resultPingRequest != null)
            {
                if(resultPingRequest.status.ToLower().Equals("success"))
                {
                    Console.WriteLine("Success");
                    Console.WriteLine(resultPingRequest.yourIp.ToString());

                    var resultRecords = GetRecords(client);

                    List<Record> recordsToUpdate = new List<Record>();

                    foreach (var record in resultRecords.records)
                    {
                        if (record.type != null && record.type.ToUpper().Equals(RecordTypeToUpdate.ToUpper()))
                        {
                            if(!record.content.Equals(resultPingRequest.yourIp))
                            {
                                Console.WriteLine($"Record {record.name}'s IP: {record.content}\nCurrent IP: {resultPingRequest.yourIp}\nUpdating IP for {record.name}\n");
                                record.content = resultPingRequest.yourIp;
                                recordsToUpdate.Add(record);
                                
                            } else
                            {
                                Console.WriteLine($"Record {record.name}'s IP: {record.content}\nCurrent IP: {resultPingRequest.yourIp}\nNot Updating IP.\n");
                            }

                        }
                    }

                    UpdateRecord(recordsToUpdate, client);
                    GetRecords(client);
                    Console.WriteLine("Finished.");

                } else
                {
                    Console.WriteLine(resultPingRequest.status);
                    Console.WriteLine(resultPingRequest.message);
                }
            }
        }

        private void InitialiseUserConfig()
        {
            apikey = System.Configuration.ConfigurationManager.AppSettings.Get("apikey");
            secretapikey = System.Configuration.ConfigurationManager.AppSettings.Get("secretapikey");
            DomainToUpdate = System.Configuration.ConfigurationManager.AppSettings.Get("DomainToUpdate");
            RecordTypeToUpdate = System.Configuration.ConfigurationManager.AppSettings.Get("RecordTypeToUpdate");
            PorkbunPing = System.Configuration.ConfigurationManager.AppSettings.Get("PorkbunPing");
            PorkbunDNSRequest = System.Configuration.ConfigurationManager.AppSettings.Get("PorkbunDNSRequest");
            PorkbunDNSEdit = System.Configuration.ConfigurationManager.AppSettings.Get("PorkbunDNSEdit");
        }

        public PingRequest GetPing(HttpClient client)
        {
            var content = new StringContent(JsonConvert.SerializeObject(this), System.Text.UTF8Encoding.UTF8, "application/json");
            var response = client.PostAsync(client.BaseAddress, content);
            var stringResponse = response.Result.Content.ReadAsStringAsync().Result;
            var jsonString = JsonConvert.DeserializeObject<PingRequest>(stringResponse);
            jsonString ??= new PingRequest
                {
                    status = "ERROR: Response object was null after json deserialize.",
                    yourIp = "127.0.0.1"
                };

            return jsonString;
        }

        public AllRecords GetRecords(HttpClient client)
        {
            var content = new StringContent(JsonConvert.SerializeObject(this), UTF8Encoding.UTF8, "application/json");
            var response = client.PostAsync($"{PorkbunDNSRequest}{DomainToUpdate}", content);
            var stringResponse = response.Result.Content.ReadAsStringAsync().Result;
            var recordsResult = JsonConvert.DeserializeObject<AllRecords>(stringResponse);

            recordsResult ??= new AllRecords { status = "ERROR: Response object was null after json deserialize." };

            return recordsResult;
        }

        public void UpdateRecord(List<Record> records, HttpClient client)
        {
            //convert to updaterecords
            List<UpdateRecord> formattedRecords = new List<UpdateRecord>();
            foreach (var record in records)
            {
                formattedRecords.Add(new UpdateRecord
                {
                    id = record.id,
                    name = record.name.Split('.')[0] + "." + record.name.Split('.')[1] == DomainToUpdate ? "" : record.name.Split('.')[0],
                    type = record.type,
                    content = record.content,
                    ttl = record.ttl,
                    prio = record.prio,
                    notes = record.notes,
                    apikey = this.apikey,
                    secretapikey = this.secretapikey
                }); ;
            }

            //update each record
            foreach (var record in formattedRecords)
            {
                var content = new StringContent(JsonConvert.SerializeObject(record), UTF8Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{PorkbunDNSEdit}{DomainToUpdate}/{record.id}", content);
                var stringResponse = response.Result.Content.ReadAsStringAsync().Result;
                var responseStatus = JsonConvert.DeserializeObject<PingRequest>(stringResponse);
                Console.WriteLine(responseStatus.status);
            }
        }

    }
}
