namespace GoogleSheet2Json.Generators
{
    public interface IGenerator
    {
        string GeneratedContent { get; }
        void Generate(Data buildData, ExportConfig exportConfig);
    }
}