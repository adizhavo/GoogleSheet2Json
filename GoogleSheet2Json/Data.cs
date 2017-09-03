using System.Text;
using System.Collections.Generic;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Data representation of the collected tokes, will go to the generator
    /// to build the json file, generate code and binaries
    /// The builder sets this up
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