using System.Configuration;
using System.Collections.Specialized;
using IPUpdater.Data;
using IPUpdater.Services;
using IPUpdater.Models;

UserSettingsModel userSettings = new UserSettingsModel(ConfigurationManager.AppSettings.Get("apikey"),
                                                       ConfigurationManager.AppSettings.Get("secretapikey"),
                                                       ConfigurationManager.AppSettings.Get("DomainToUpdate"),
                                                       ConfigurationManager.AppSettings.Get("RecordTypeToUpdate"),
                                                       ConfigurationManager.AppSettings.Get("PorkbunPing"),
                                                       ConfigurationManager.AppSettings.Get("PorkbunDNSRequest"),
                                                       ConfigurationManager.AppSettings.Get("PorkbunDNSEdit"));

IPUpdaterService apiService = new IPUpdaterService(userSettings);

var currentIP = apiService.PingAPI();
var DNSRecords = apiService.GetDNSRecords();
var filteredRecords = apiService.FilterDNSRecordsFromResponse(DNSRecords, ConfigurationManager.AppSettings.Get("RecordTypeToUpdate"));
