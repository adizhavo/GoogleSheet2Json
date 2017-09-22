namespace GoogleSheet2Json
{
    public class StringConstants
    {
        public const string WORD_PATTERN = @"(\w+\s*)*[^( \[* | \]* | \(* | \)* | \>* | \-* | \,* | *)]";
        public const string EMPTY_SPACE_PATTERN = @"^\s";
        public const string COMMA = ",";
        public const string RANGE_CHAR = ">";
        public const string OPEN_BRACKET = "(";
        public const string CLOSE_BRACKET = ")";
        public const string OPEN_SQUARE_BRACKET = "[";
        public const string CLOSE_SQUARE_BRACKET = "]";
        public const string COLON = ":";
        public const string EMPTY_SPACE = " ";
        public const string TRUE_CHAR = "TRUE";
        public const string FALSE_CHAR = "FALSE";
        public const string COMMENT_ANNOTATION = "#comment";
    }
}