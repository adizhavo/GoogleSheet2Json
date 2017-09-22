namespace GoogleSheet2Json
{
    /// <summary>
    /// Will build the data structure for the parsed data by receiving commands
    /// from the parser
    /// </summary>
    
    // This interface is required to create mocks for tests
    
    public interface IBuilder
    {
        void StartBuild();
        void SetRootName(string name);
        void StartField(string fieldDefintion);
        void EndField();
        void EndBuild();
        void SetField(string name);
        void TryAddMinRange(string divider);
        void TryAddMaxRange(string value);
        void StartCollection();
        void AddCollectionElement(string element);
        void StartMap();
        void AddKey(string key);
        void AddValue(string value);
    }
}