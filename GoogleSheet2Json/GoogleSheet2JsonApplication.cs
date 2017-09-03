namespace GoogleSheet2Json
{
    public class GoogleSheet2JsonApplication
    {
        private static void Main(string[] args)
        {
            var builder = new Builder();
            var parser = new Parser(builder);
            var lexer = new Lexer(parser);
            var sheet2Json = new GoogleSheet2Json(args, lexer);
            sheet2Json.ExtractData();
            sheet2Json.WriteData();
        }
    }
}