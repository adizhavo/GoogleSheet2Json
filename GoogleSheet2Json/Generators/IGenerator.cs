namespace GoogleSheet2Json.Generators
{
    public interface IGenerator
    {
        string GeneratedFile { get; }
        void Generate(Data buildData, ExportConfig exportConfig);
    }
}