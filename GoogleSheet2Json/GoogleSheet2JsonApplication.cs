namespace GoogleSheet2Json
{
    public class GoogleSheet2JsonApplication
    {
        private static void Main(string[] args)
        {
            var sheet2Json = new GoogleSheet2Json();
            sheet2Json.ReadDataFromSheet();
            
            var builder = new Builder();
            var parser = new Parser(builder);
            var lexer = new Lexer(parser);
            
            lexer.Lex(sheet2Json.dataKeys[0], sheet2Json.dataValues);
        }
    }
}