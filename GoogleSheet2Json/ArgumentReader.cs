using System;
using System.Collections.Generic;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Will read/parse the Comamnd Line Arguments and setup export variables
    /// </summary>
    
    public class ArgumentReader
    {
        public ExportConfig exportConfig { get; }
        
        public ArgumentReader()
        {
            exportConfig = new ExportConfig();
        }
        
        public void Parse(string[] arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument[0] == '-')
                {
                    var trimmedArgument = argument.Substring(1);
                    var split = trimmedArgument.Split('=');
                    var command = split.Length > 0 ? split[0] : trimmedArgument;
                    var variables = split.Length > 1 ? split[1] : string.Empty;
                
                    CheckForArrayOrSingleObject(command);   
                    AddLiteratKeys(command, variables);
                    SetSheetTab(command, variables);
                    SetKeyValueRange(command, variables);
                    SetConfigPath(command, variables);
                    SetOutputPath(command, variables);
                    SetOutputFileName(command, variables);
                }
            }

            #if DEBUG
            Console.WriteLine($"[ArgumentReader] arguments setup: {exportConfig}");      
            #endif
        }

        private void CheckForArrayOrSingleObject(string command)
        {
            if (string.Equals(StringConstants.IS_SINGLE_OBJECT_COMMAND, command))
            {
                exportConfig.isSingleObject = true; 
                exportConfig.isArrayOfObjects = false;
            }
        }

        private void AddLiteratKeys(string command, string variables)
        {
            if (string.Equals(StringConstants.IS_LITERAL_KEY_COMMAND, command))
            {
                var keys = variables.Split(',');
                foreach (var key in keys)
                {
                    exportConfig.literalKeys.Add(key);
                }
            }
        }

        private void SetSheetTab(string command, string variable)
        {
            if (string.Equals(StringConstants.SHEET_TAB_COMMAND, command))
            {
                exportConfig.sheetTab = variable;
            }
        }

        private void SetKeyValueRange(string command, string variable)
        {
            if (string.Equals(StringConstants.KEY_RANGE_COMMAND, command))
            {
                exportConfig.keyRange = variable;
            }
            else if (string.Equals(StringConstants.VALUE_RANGE_COMMAND, command))
            {
                exportConfig.valueRange = variable;
            }
        }

        private void SetConfigPath(string command, string variable)
        {
            if (string.Equals(StringConstants.CONFIG_FILE_PATH_COMMAND, command))
            {
                exportConfig.configPath = variable;
            }
        }

        private void SetOutputPath(string command, string varible)
        {
            if (string.Equals(StringConstants.OVERRIDE_OUTPUT_DIR_COMMAND, command))
            {
                exportConfig.outputDir = varible;
            }
        }
        
        private void SetOutputFileName(string command, string varible)
        {
            if (string.Equals(StringConstants.OUTPUT_FILE_NAME_COMMAND, command))
            {
                exportConfig.outputFileName = varible;
            }
        }
    }

    public class ExportConfig
    {
        public List<string> literalKeys;
        public bool isArrayOfObjects;
        public bool isSingleObject;
        public string sheetTab;
        public string keyRange;
        public string valueRange;
        public string configPath;
        public string outputDir;
        public string outputFileName;
        
        public ExportConfig()
        {
            literalKeys = new List<string>();
            sheetTab = string.Empty;
            keyRange = string.Empty;
            valueRange = string.Empty;
            isArrayOfObjects = true;
            configPath = string.Empty;
            outputDir = string.Empty;
            outputFileName = "FILE_NAME_NOT_SET";
        }

        public override string ToString()
        {
            return $"literalKeys: {literalKeys.Count}, isArray: {isArrayOfObjects}, isSingle: {isSingleObject}, sheetTab: {sheetTab}, keyRange: {keyRange}, valueRange: {valueRange}, configPath: {configPath}, outputDir: {outputDir}, outputFileName: {outputFileName}";
        }
    }
}