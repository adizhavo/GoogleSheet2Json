using System;

namespace GoogleSheet2Json
{
    public class Builder : IBuilder
    {
        /// <summary>
        /// Will build the data structure for the parsed data by receiving commands
        /// from the parser
        /// </summary>
        
        public Data buildData { get; private set; }

        private FieldNode fieldNode;

        public Builder()
        {
            buildData = new Data();
        }

        public void StartBuild()
        {
            buildData.Clear();
        }

        public void SetRootName(string name)
        {
            buildData.root = name;
        }

        public void StartField(string fieldDefintion)
        {
            fieldNode = new FieldNode {definition = fieldDefintion};
        }

        public void EndField()
        {
            buildData.properties.Add(fieldNode);
        }

        public void EndBuild()
        {
            #if DEBUG
            Console.WriteLine($"End Build with data: {buildData}");
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

        public void AddCollectionElement(string element)
        {
            fieldNode.fieldValue = string.Empty;
            fieldNode.collectionValues.Add(element);
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
    }
}