using System;
using GoogleSheet2Json.Generators;

namespace GoogleSheet2Json
{
    public class GoogleSheet2JsonApplication
    {
        private static void Main(string[] args)
        {
            // read arguments
            var argumentReader = new ArgumentReader();
            argumentReader.Parse(args);
            
            // setup the app config
            var configReader = new ConfigReader();
            configReader.Initialise(argumentReader.exportConfig);
            
            // extract data from sheets
            var sheet2Json = new GoogleSheet2Json(configReader.appConfig);
            sheet2Json.ReadDataFromSheet(argumentReader.exportConfig);
           
            // setup compilation process
            var builder = new Builder();
            var parser = new Parser(builder);
            var lexer = new Lexer(parser);
            var generator = new JsonGenerator();
            
            lexer.Lex(sheet2Json.dataKeys[0], sheet2Json.dataValues, argumentReader.exportConfig);
            generator.Generate(builder.BuildData, argumentReader.exportConfig);
            
            Console.WriteLine(generator.GeneratedFile);
        }
    }
}