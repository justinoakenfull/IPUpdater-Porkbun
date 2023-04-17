using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPUpdater.Models
{
    public class UserSettingsModel
    {
        public string apiKey { get; set; }
        public string secretapikey { get; set; }
        public string DomainToUpdate { get; set; }
        public string RecordTypeToUpdate { get; set; } = "A";
        public string APIPingURI { get; set; }
        public string APIDNSRequestURI { get; set; }
        public string APIDNSEditURI { get; set; }
        public bool VerboseDebugging { get; set; }

        public UserSettingsModel(string storedApiKey, string storedSecretApiKey,
                                 string storedDomainToUpdate, string storedRecordTypeToUpdate,
                                 string storedAPIPingURI, string storedAPIDNSRequestURI,
                                 string storedAPIDNSEditURI, string storedVerboseDebugging) 
        {
            apiKey = storedApiKey;
            secretapikey = storedSecretApiKey;
            DomainToUpdate = storedDomainToUpdate;
            RecordTypeToUpdate = storedRecordTypeToUpdate;
            APIPingURI = storedAPIPingURI;
            APIDNSRequestURI = storedAPIDNSRequestURI;
            APIDNSEditURI = storedAPIDNSEditURI;
            VerboseDebugging = storedVerboseDebugging == "True" ? 
                                                         true : 
                               storedVerboseDebugging == "true" ? 
                                                         true :
                               storedVerboseDebugging == "TRUE" ? 
                                                         true : 
                                                         false;
        }
    }
}

