using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Will lex the stream of data and deliver commands to the parser
    /// </summary>

    public class Lexer
    {
        public const string NAME_REGEX = @"(\w+\s*\b.*)+";
        public const string EMPTY_SPACE_REGEX = @"^\s";

        private Parser parser;

        private int positionInLine = 0;

        public Lexer(Parser parser)
        {
            this.parser = parser;
        }

        public void Lex(IList<object> keys, IList<object> values)
        {
            parser.Start();

            for(int i = 0; i < keys.Count; i ++)
            {
                var key = keys[i];
                var value = values[i];

                LexKey(key.ToString());
                LexValue(value.ToString());
            }

             parser.End();
        }

        private void LexKey(string key)
        {
            parser.Name(key);
        }

        private void LexValue(string value)
        {
            parser.StartValue();

            positionInLine = 0;

            while(positionInLine < value.Length)
            {
                LexToken(value);
            }
        }

        private void LexToken(string value)
        {
            if (!TokenFound(value))
            {
                throw new System.ArgumentException("[Lexer] value is not a valid token, please read the documentation: " + value);
            }
        }

        private bool TokenFound(string value)
        {
            return IsEmptySpace(value) || IsSingleCharToken(value) || IsName(value);
        }

        private bool IsEmptySpace(string value)
        {
            if (positionInLine < value.Length)
            {
                var subString = value.Substring(positionInLine);
                var regex = Regex.Match(subString, EMPTY_SPACE_REGEX);
                if (regex.Success)
                {
                    positionInLine += regex.Value.Length;
                }

                return regex.Success;
            }
            else return false;
        }

        private bool IsSingleCharToken(string value)
        {
            if (positionInLine < value.Length)
            {
                var found = true;
                var character = value.Substring(positionInLine, 1);
                switch (character)
                {
                    case "," : parser.Comma(); break;
                    case "-" : parser.Dash(); break;
                    case "(" : parser.OpenBrace(); break;
                    case ")" : parser.CloseBrace(); break;
                    case "[" : parser.OpenSquareBrackets(); break;
                    case "]" : parser.CloseSquareBrackets(); break;
                    default  : found = false; break;
                }

                if (found)
                {
                    positionInLine ++;
                }

                return found;
            }
            else return false;
        }

        private bool IsName(string value)
        {
            if (positionInLine < value.Length)
            {
                var subString = value.Substring(positionInLine);
                var regex = Regex.Match(subString, NAME_REGEX);
                if (regex.Success)
                {
                    parser.Name(regex.Value);
                    positionInLine += regex.Value.Length;
                    return true;
                }
            }

            return false;
        }
    }
}

