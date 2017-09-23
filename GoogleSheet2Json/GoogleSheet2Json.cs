using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace GoogleSheet2Json
{
    /// <summary>
    /// reads and interpres arguments, interacts with the google API to extract the data from the sheet
    /// It loads credentials and creates the service for the comunication
    /// </summary>

    public class GoogleSheet2Json
    {
        public IList<IList<object>> dataKeys { private set; get; }
        public IList<IList<object>> dataValues { private set; get; }

        // TODO : read these values from a config file in Json and CLI arguments
        private static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private static string ApplicationName = "GoogleSheet2Json";
        private static string spreadSheetId = "1PnKZzMfM762HAWQBUMWxO0TNIEG3RZEGI_0rKm47x0U";
        private static string userName = "Adi Zhavo";
        private static string clientSecret = "client_secret.json";
        private static string keyRange = "A1:K";
        private static string valueRange = "A2:K";
        private static string tabName = "SAMPLE_1";

        private SheetsService service;

        public GoogleSheet2Json()
        {
            // Load credentials
            var credential = CreateUserCredentials(); 

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
        }

        public void ReadDataFromSheet()
        {
            // TODO : support key value pair data definition
            dataKeys = ExtractDataFromSheet(service, spreadSheetId, tabName, keyRange);
            dataValues = ExtractDataFromSheet(service, spreadSheetId, tabName, valueRange);

            if (dataKeys == null || dataValues == null)
                throw new Exception("[GoogleSheet2Json] keys or values are null, please check that the inputted data is correct");
        }

        private UserCredential CreateUserCredentials()
        {
            UserCredential credential;
            using (var stream = new FileStream(clientSecret, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, userName, CancellationToken.None, new FileDataStore(credPath, true)).Result;
                Console.WriteLine("[GoogleSheet2Json] Credential file saved to: " + credPath);
            }
            return credential;
        }

        private IList<IList<object>> ExtractDataFromSheet(SheetsService service, string spreadSheetId, string tabName, string dataRange)
        {
            // Define request parameters.
            var range = tabName + "!" + dataRange;
            var request = service.Spreadsheets.Values.Get(spreadSheetId, range);

            // read spread sheet
            var response = request.Execute();
            var values = response.Values;
            return response.Values;
        }
    }
}
