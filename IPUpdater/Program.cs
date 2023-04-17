using System.Configuration;
using IPUpdater.Services;
using IPUpdater.Models;

UserSettingsModel userSettings = new UserSettingsModel(ConfigurationManager.AppSettings.Get("apikey"),
                                                       ConfigurationManager.AppSettings.Get("secretapikey"),
                                                       ConfigurationManager.AppSettings.Get("DomainToUpdate"),
                                                       ConfigurationManager.AppSettings.Get("RecordTypeToUpdate"),
                                                       ConfigurationManager.AppSettings.Get("PorkbunPing"),
                                                       ConfigurationManager.AppSettings.Get("PorkbunDNSRequest"),
                                                       ConfigurationManager.AppSettings.Get("PorkbunDNSEdit"),
                                                       ConfigurationManager.AppSettings.Get("VerboseDebugging"));

IPUpdaterService apiService = new IPUpdaterService(userSettings);
if (userSettings.VerboseDebugging)
{
    Console.WriteLine("Getting current IP and authenticating with Porkbun API End points.");
}
var currentIP = apiService.PingAPI();

if (currentIP != null)
{
    if(currentIP.status.ToLower().Equals("success"))
    {
        if(userSettings.VerboseDebugging)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{currentIP.status}: Your current IP is {currentIP.yourip}.");
            Console.ResetColor();
            Console.WriteLine("Getting current DNS Records...");
        }
        //We have the IP and have tested authentication.
        var DNSRecords = apiService.GetDNSRecords();
        if (DNSRecords != null &&
            DNSRecords.records.Count > 0)
        {
            if (userSettings.VerboseDebugging)
            {
                foreach (var DNSRecord in DNSRecords.records)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DNSRecord.name.ToUpper()} - {DNSRecord.type}\n" +
                                      $"========================\n" +
                                      $"ID: {DNSRecord.id}\n" +
                                      $"Set IP: {DNSRecord.content}\n" +
                                      $"Server Current IP: {currentIP.yourip}\n" +
                                      $"========================\n");
                    Console.ResetColor();
                }
            }

            var updateResponses = apiService.UpdateDNSRecords(DNSRecords);

            if (updateResponses != null &&
                updateResponses.Count > 0)
            {
                if (userSettings.VerboseDebugging)
                {
                    foreach(var updateResponse in updateResponses)
                    {
                        if (updateResponse.status.ToLower().Equals("SUCCESS"))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{updateResponse.status}: {updateResponse.message}.\n");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{updateResponse.status}: {updateResponse.message}");
                            Console.ResetColor();
                        }
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No records were updated. This is usually because your IP has not changed.");
            }
        }
        else if (userSettings.VerboseDebugging)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DNSRecords.status}: {DNSRecords.message}");
            Console.ResetColor();
        }

    } 
    else if(userSettings.VerboseDebugging)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{currentIP.status}: {currentIP.message}");
        Console.ResetColor();
    }
}