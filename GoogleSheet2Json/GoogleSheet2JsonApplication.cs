using System;
using GoogleSheet2Json.Generators;

namespace GoogleSheet2Json
{
    public class GoogleSheet2JsonApplication
    {
        private static void Main(string[] args)
        {
            var argumentReader = new ArgumentReader();
            argumentReader.Parse(args);
            
            var sheet2Json = new GoogleSheet2Json();
            sheet2Json.ReadDataFromSheet();
           
            var builder = new Builder();
            var parser = new Parser(builder);
            var lexer = new Lexer(parser);
            var generator = new JsonGenerator();
            
            lexer.Lex(sheet2Json.dataKeys[0], sheet2Json.dataValues);
            generator.Generate(builder.BuildData);
            
            Console.WriteLine(generator.GeneratedFile);
        }
    }
}