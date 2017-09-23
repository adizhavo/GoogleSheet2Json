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
                }
            }
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
    }

    public class ExportConfig
    {
        public List<string> literalKeys;
        public bool isArrayOfObjects;
        public bool isSingleObject;
        public string sheetTab;
        public string keyRange;
        public string valueRange;
        
        public ExportConfig()
        {
            literalKeys = new List<string>();
            sheetTab = string.Empty;
            keyRange = string.Empty;
            valueRange = string.Empty;
            isArrayOfObjects = true;
        }
    }
}