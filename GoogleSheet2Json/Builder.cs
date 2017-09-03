using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleSheet2Json
{
    public class Builder
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


    /// <summary>
    /// Represent the data structure for the data that will be passed to the generators
    /// to build the json file, generate code and binaries
    /// The builder is reposible for building this
    /// </summary>

    public class Data
    {
        public string root;
        public List<PropertyNode> properties = new List<PropertyNode>();

        public void Clear()
        {
            root = string.Empty;
            properties.Clear();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"[DATA] {root}\n");
            
            foreach(var prop in properties) 
                stringBuilder.Append(prop + "\n");

            return stringBuilder.ToString();
        }
    }

    public class PropertyNode
    {
        public string propDefintion;
        
        public string fieldValue;
        
        // properties for building a key value data
        public bool isMap;
        public string key;
        public string value;
        
        // properties for building an array of key values
        public bool isArrayOfMaps;
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();
        
        // properties for building a collection
        public bool isCollection;
        public List<string> collectionValues = new List<string>();
        
        // properties for building a range
        public bool isRange;
        public string min;
        public string max;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"[PROP] {propDefintion}, name: {fieldValue}, isMap {isMap}, key {key}, value {value}, isRange {isRange}, min {min}, max {max},");
            stringBuilder.Append($"isArrayOfMaps {isArrayOfMaps}, keys: ");
            
            foreach(var k in keys) 
                stringBuilder.Append(k + " ");

            stringBuilder.Append(", values: ");
            
            foreach(var v in values) 
                stringBuilder.Append(v + " ");
            
            stringBuilder.Append($"isCollection {isCollection}, elements: ");
            
            foreach(var c in collectionValues) 
                stringBuilder.Append(c + " ");

            return stringBuilder.ToString();
        }
    }
}