using System;

namespace GoogleSheet2Json
{
    public class Builder : IBuilder
    {
        /// <summary>
        /// Will build the data structure for the parsed data by receiving commands
        /// from the parser
        /// </summary>
        
        public Data BuildData { get; private set; }

        private PropertyNode propertyNode;
        private FieldNode fieldNode;

        public Builder()
        {
            BuildData = new Data();
        }

        public void StartBuild()
        {
            BuildData.root = string.Empty;
            BuildData.properties.Clear();
        }

        public void SetRootName(string name)
        {
            BuildData.root = name;
        }

        public void StartProperty()
        {
            propertyNode = new PropertyNode();
        }

        public void EndProperty()
        {
            BuildData.properties.Add(propertyNode);

            #if DEBUG
            Console.WriteLine($"End property with fields: {propertyNode}");
            #endif
        }

        public void StartField(string fieldDefintion)
        {
            fieldNode = new FieldNode {definition = fieldDefintion};
        }

        public void EndField()
        {
            propertyNode.fields.Add(fieldNode);
            
            #if DEBUG
            Console.WriteLine($"End field with data: {fieldNode}");
            #endif
        }

        public void EndBuild()
        {
            #if DEBUG
            Console.WriteLine($"End Build with data: {BuildData}");
            #endif
        }

        public void SetField(string name)
        {
            fieldNode.fieldValue += name;
        }

        public void TryAddMinRange(string divider)
        {
            var fValue = fieldNode.fieldValue;

            short outShort = -1;
            int outInt = -1;
            long outLong = -1;
            float outFloat = -1f;
            
            var isInt = short.TryParse(fValue, out outShort) || int.TryParse(fValue, out outInt) || long.TryParse(fValue, out outLong);
            var isFloat = float.TryParse(fValue, out outFloat);

            if (isInt || isFloat)
            {
                fieldNode.isRange = true;
                fieldNode.min = fieldNode.fieldValue;
                fieldNode.fieldValue = string.Empty;
            }
            else
            {
                fieldNode.fieldValue += "-";
            }
        }

        public void TryAddMaxRange(string value)
        {
            if (fieldNode.isRange)
            {
                fieldNode.max = value;
                fieldNode.fieldValue = string.Empty;
            }
            else
            {
                fieldNode.fieldValue += value;
            }
        }

        public void StartCollection()
        {
            fieldNode.fieldValue = string.Empty;
            fieldNode.isCollection = true;
        }

        public void AddFieldToCollection()
        {
            fieldNode.collectionValues.Add(fieldNode.fieldValue.Trim());
            fieldNode.fieldValue = string.Empty;
        }

        public void StartMap()
        {
            fieldNode.fieldValue = string.Empty;
            
            if (fieldNode.isMap)
            {
                fieldNode.isMap = false;
                fieldNode.isArrayOfMaps = true;
                
                fieldNode.keys.Add(fieldNode.key);
                fieldNode.key = string.Empty;
                
                fieldNode.values.Add(fieldNode.value);
                fieldNode.value = string.Empty;
            }
            else
            {
                fieldNode.isMap = true;
            }
        }

        public void AddKey(string key)
        {
            fieldNode.fieldValue = string.Empty;
            
            if (fieldNode.isMap)
            {
                fieldNode.key = key;
            }
            else if (fieldNode.isArrayOfMaps)
            {
                fieldNode.keys.Add(key);
            }
        }

        public void AddValue(string value)
        {
            fieldNode.fieldValue = string.Empty;
            
            if (fieldNode.isMap)
            {
                fieldNode.value = value;
            }
            else if (fieldNode.isArrayOfMaps)
            {
                fieldNode.values.Add(value);
            }
        }

        public void AppendToKey(string key)
        {
            if (fieldNode.isMap)
            {
                fieldNode.key += key;
            }
            else if (fieldNode.isArrayOfMaps && fieldNode.keys.Count > 0)
            {
                fieldNode.keys[fieldNode.keys.Count - 1] += key;
            }
        }
        
        public void AppendToValue(string value)
        {
            if (fieldNode.isMap)
            {
                fieldNode.value += value;
            }
            else if (fieldNode.isArrayOfMaps && fieldNode.values.Count > 0)
            {
                fieldNode.values[fieldNode.values.Count - 1] += value;
            }
        }

        public void AppendToLastElementOfCollection(string append)
        {
            if (fieldNode.isCollection && fieldNode.collectionValues.Count > 0)
            {
                fieldNode.collectionValues[fieldNode.collectionValues.Count - 1] += append;
            }
        }
    }
}