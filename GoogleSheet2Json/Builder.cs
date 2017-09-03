using System;

namespace GoogleSheet2Json
{
    public class Builder : IBuilder
    {
        /// <summary>
        /// Will build the data structure for the parsed data by receiving commands
        /// from the parser
        /// </summary>

        private Data buildData;
        private PropertyNode buildPropertyNode;

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

        public void StartProperty(string propDef)
        {
            buildPropertyNode = new PropertyNode {propDefintion = propDef};
        }

        public void EndProperty()
        {
            buildData.properties.Add(buildPropertyNode);
        }

        public void EndBuild()
        {
            // call the generator to build the structure
            
            #if DEBUG
            Console.WriteLine($"build data: {buildData}");
            #endif
        }

        public void AddField(string name)
        {
            buildPropertyNode.fieldValue += name;
        }

        public void TryAddMinRange(string divider)
        {
            var fValue = buildPropertyNode.fieldValue;

            short outShort = -1;
            int outInt = -1;
            long outLong = -1;
            float outFloat = -1f;
            
            var isInt = short.TryParse(fValue, out outShort) || int.TryParse(fValue, out outInt) || long.TryParse(fValue, out outLong);
            var isFloat = float.TryParse(fValue, out outFloat);

            if (isInt || isFloat)
            {
                buildPropertyNode.isRange = true;
                buildPropertyNode.min = buildPropertyNode.fieldValue;
                buildPropertyNode.fieldValue = string.Empty;
            }
            else
            {
                buildPropertyNode.fieldValue += "-";
            }
        }

        public void TryAddMaxRange(string value)
        {
            if (buildPropertyNode.isRange)
            {
                buildPropertyNode.max = value;
            }
            else
            {
                buildPropertyNode.fieldValue += value;
            }
        }

        public void StartCollection()
        {
            buildPropertyNode.isCollection = true;
        }

        public void AddCollectionElement(string element)
        {
            buildPropertyNode.collectionValues.Add(element);
        }

        public void StartMap()
        {
            if (buildPropertyNode.isMap)
            {
                buildPropertyNode.isMap = false;
                buildPropertyNode.isArrayOfMaps = true;
                
                buildPropertyNode.keys.Add(buildPropertyNode.key);
                buildPropertyNode.key = string.Empty;
                
                buildPropertyNode.values.Add(buildPropertyNode.value);
                buildPropertyNode.value = string.Empty;
            }
            else
            {
                buildPropertyNode.isMap = true;
            }
        }

        public void AddKey(string key)
        {
            if (buildPropertyNode.isMap)
            {
                buildPropertyNode.key = key;
            }
            else if (buildPropertyNode.isArrayOfMaps)
            {
                buildPropertyNode.keys.Add(key);
            }
        }

        public void AddValue(string value)
        {
            if (buildPropertyNode.isMap)
            {
                buildPropertyNode.value = value;
            }
            else if (buildPropertyNode.isArrayOfMaps)
            {
                buildPropertyNode.values.Add(value);
            }
        }
    }
}