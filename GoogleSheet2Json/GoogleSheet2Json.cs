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
        
        private static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private SheetsService service;
        private AppConfig appConfig;
        
        public GoogleSheet2Json(AppConfig appConfig)
        {
            this.appConfig = appConfig;
            
            // Load credentials
            var credential = CreateUserCredentials(); 

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = appConfig.applicationName,
                });
        }

        public void ReadDataFromSheet(ExportConfig exportConfig)
        {
            dataKeys = ExtractDataFromSheet(service, appConfig.spreadSheetId, exportConfig.sheetTab, exportConfig.keyRange);
            dataValues = ExtractDataFromSheet(service, appConfig.spreadSheetId, exportConfig.sheetTab, exportConfig.valueRange);

            if (dataKeys == null || dataValues == null)
                throw new Exception("[GoogleSheet2Json] keys or values are null, please check that the inputted data is correct");
        }

        private UserCredential CreateUserCredentials()
        {
            UserCredential credential;
            using (var stream = new FileStream(appConfig.clientSecret, FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, appConfig.userName, CancellationToken.None, new FileDataStore(credPath, true)).Result;
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
