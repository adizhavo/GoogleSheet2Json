using System;
using System.Collections.Generic;

namespace GoogleSheet2Json.Generators
{
    public class JsonGenerator : IGenerator
    {
        public string GeneratedContent { get; private set; }

        public const string QUOTE = "\"";
        public const string COLON = ":";
        public const string COMMA = ",";
        public const string OPEN_SQUARE_BRAC = "[";
        public const string CLOSE_SQUARE_BRAC = "]";
        public const string OPEN_BRAC = "{";
        public const string CLOSE_BRAC = "}";
        private ExportConfig exportConfig;
        
        public void Generate(Data buildData, ExportConfig exportConfig)
        {
            this.exportConfig = exportConfig;
            
            if (exportConfig.isArrayOfObjects)
            {
                GenerateArrayOfObjects(buildData);                
            }
            else if (exportConfig.isSingleObject)
            {
                GenerateSingleObject(buildData);
            }
            
            Logger.LogLine($"Generated content: \n{GeneratedContent}");
        }

        private void GenerateSingleObject(Data buildData)
        {
            GenerateFields(buildData.fields);
        }
        
        private void GenerateArrayOfObjects(Data buildData)
        {
            GeneratedContent = OPEN_BRAC;
            GeneratedContent += QUOTE;
            GeneratedContent += buildData.root;
            GeneratedContent += QUOTE + COLON + OPEN_SQUARE_BRAC;

            for (int i = 0; i < buildData.properties.Count; i++)
            {
                GenerateFields(buildData.properties[i].fields);

                if (i < buildData.properties.Count - 1)
                    GeneratedContent += COMMA;
            }

            GeneratedContent += CLOSE_SQUARE_BRAC + CLOSE_BRAC;
        }

        private void GenerateFields(List<FieldNode> fields)
        {
            GeneratedContent += OPEN_BRAC;
            
            for (int i = 0; i < fields.Count; i ++)
            {
                var property = fields[i];
                GeneratedContent += QUOTE + property.definition + QUOTE + COLON;

                if (property.isRange)
                {
                    BuildRange(property.min, property.max);
                }
                else if (property.isCollection)
                {
                    BuildCollection(property.collectionValues);
                }
                else if (property.isMap)
                {
                    BuildMap(property.key, property.value);
                }
                else if (property.isArrayOfMaps)
                {
                    BuildArrayOfMAps(property.keys, property.values);
                }
                else
                {
                    BuildField(property.definition, property.fieldValue);
                }
                
                if (i < fields.Count - 1)
                    GeneratedContent += COMMA;
            }
            
            GeneratedContent += CLOSE_BRAC;
        }

        private void BuildField(string definition, string element)
        {
            var isString = IsStringValue(element);
            
            if (isString && !exportConfig.literalKeys.Contains(definition)) GeneratedContent += QUOTE;

            GeneratedContent += TryIfElementIsBoolean(element);

            if (isString && !exportConfig.literalKeys.Contains(definition)) GeneratedContent += QUOTE;
        }

        private void BuildArrayOfMAps(List<string> keys, List<string> values)
        {
            GeneratedContent += OPEN_SQUARE_BRAC;

            for (int j = 0; j < keys.Count; j++)
            {
                var key = keys[j];
                var value = values[j];

                BuildMap(key, value);

                if (j < keys.Count - 1)
                    GeneratedContent += COMMA;
            }

            GeneratedContent += CLOSE_SQUARE_BRAC;
        }

        // Build range as { "min" : x, "max" : y }
        private void BuildRange(string min, string max)
        {
            GeneratedContent += OPEN_BRAC + QUOTE + "min" + QUOTE + COLON + min + COMMA + QUOTE + "max" + QUOTE + COLON +
                             max + CLOSE_BRAC;
        }

        // Build collection as [ x, y, z ]
        private void BuildCollection(List<string> collectionValues)
        {
            GeneratedContent += OPEN_SQUARE_BRAC;
            for (int j = 0; j < collectionValues.Count; j++)
            {
                var element = collectionValues[j];
                var isString = IsStringValue(element);

                if (isString) GeneratedContent += QUOTE;

                GeneratedContent += TryIfElementIsBoolean(element);

                if (isString) GeneratedContent += QUOTE;

                if (j < collectionValues.Count - 1)
                    GeneratedContent += COMMA;
            }
            GeneratedContent += CLOSE_SQUARE_BRAC;
        }

        // Build map as { "key" : x, "value" : y }
        private void BuildMap(string key, string value)
        {
            GeneratedContent += OPEN_BRAC + QUOTE + "key" + QUOTE + COLON;

            var isString = IsStringValue(key);

            if (isString) GeneratedContent += QUOTE;

            GeneratedContent += key;

            if (isString) GeneratedContent += QUOTE;

            GeneratedContent += COMMA + QUOTE + "value" + QUOTE + COLON;

            isString = IsStringValue(value);

            if (isString) GeneratedContent += QUOTE;

            GeneratedContent += TryIfElementIsBoolean(value);

            if (isString) GeneratedContent += QUOTE;

            GeneratedContent += CLOSE_BRAC;
        }

        private bool IsStringValue(string value)
        {
            short outShort = -1;
            int outInt = -1;
            long outLong = -1;
            float outFloat = -1f;
            
            var isInt = short.TryParse(value, out outShort) || int.TryParse(value, out outInt) || long.TryParse(value, out outLong);
            var isFloat = float.TryParse(value, out outFloat);

            return !isInt && !isFloat;
        }

        private string TryIfElementIsBoolean(string element)
        {
            if (element == StringConstants.TRUE_CHAR)
            {
                element = "true";
            }
            else if (element == StringConstants.FALSE_CHAR)
            {
                element = "false";   
            }

            return element;
        }
    }
}