namespace GoogleSheet2Json
{
    /// <summary>
    /// Will hold all string constant of the application
    /// </summary>
    
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
        
        // Command line commands
        public const string IS_SINGLE_OBJECT_COMMAND = "isSingleObject";
        public const string IS_LITERAL_KEY_COMMAND = "isLiteral";
        public const string SHEET_TAB_COMMAND = "sheetTab";
        public const string KEY_RANGE_COMMAND = "keyRange";
        public const string VALUE_RANGE_COMMAND = "valueRange";
    }
}