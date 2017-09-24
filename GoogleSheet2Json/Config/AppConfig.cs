using System;
using System.IO;
using Newtonsoft.Json;

namespace GoogleSheet2Json
{   
    public class AppConfig
    {
        public string applicationName;
        public string spreadSheetId;
        public string userName;
        public string clientSecret;
        public string outputDirectory;

        public override string ToString()
        {
            return $"[AppConfig] applicationName: {applicationName}, spreadSheetID: {spreadSheetId}, userName: {userName}, clientSecret: {clientSecret}, outputDirectory: {outputDirectory}";
        }
    }

    public class ConfigReader
    {
        public AppConfig appConfig { private set; get; }
        
        public void Initialise(ExportConfig exportConfig)
        {
            if (string.IsNullOrEmpty(exportConfig.configPath))
            {
                throw new ArgumentNullException("[ConfigReader] config path is empty");
            }
            
            StreamReader file =  new StreamReader(exportConfig.configPath);
            using (file)
            {
                appConfig = JsonConvert.DeserializeObject<AppConfig>(file.ReadToEnd());
                if (!string.IsNullOrEmpty(exportConfig.outputDir))
                {
                    appConfig.outputDirectory = exportConfig.outputDir;
                }

                Logger.DebugLogLine($"[ConfigReader] Start app with config {appConfig}");
            }
        }
    }
}