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
        public List<FieldNode> properties = new List<FieldNode>();

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

    public class FieldNode
    {
        public string definition = string.Empty;
        
        public string fieldValue = string.Empty;
        
        // properties for building a key value data
        public bool isMap;
        public string key = string.Empty;
        public string value = string.Empty;
        
        // properties for building an array of key values
        public bool isArrayOfMaps;
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();
        
        // properties for building a collection
        public bool isCollection;
        public List<string> collectionValues = new List<string>();
        
        // properties for building a range
        public bool isRange;
        public string min = string.Empty;
        public string max = string.Empty;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"[PROP] {definition}, name: {fieldValue}, isMap {isMap}, key {key}, value {value}, isRange {isRange}, min {min}, max {max},");
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