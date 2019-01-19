namespace GoogleSheet2Json
{
    /// <summary>
    /// Will build the data structure for the parsed data by receiving commands
    /// from the parser
    /// </summary>
    
    // This interface is required to create mocks for tests
    
    public interface IBuilder
    {
        void StartBuildSingleObject();
        void StartBuildArrayOfObjects();
        void SetRootName(string name);
        void StartProperty();
        void EndProperty();
        void StartField(string fieldDefintion);
        void EndField();
        void EndBuild();
        void SetField(string name);
        void TryAddMinRange(string divider);
        void TryAddMaxRange(string value);
        void StartCollection();
        void AddFieldToCollection();
        void StartMap();
        void StartMapArray();
        void AddKey(string key);
        void AddValue(string value);
        void AppendToKey(string key);
        void AppendToValue(string value);
        void AppendToLastElementOfCollection(string append);
    }
}