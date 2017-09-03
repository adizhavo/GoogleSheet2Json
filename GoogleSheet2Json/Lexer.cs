using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// ReSharper disable EmptyForStatement

namespace GoogleSheet2Json
{
    /// <summary>
    /// Will lex the stream of data and deliver commands to the parser
    /// </summary>

    public class Lexer
    {
        public const string WORD_PATTERN = @"(\w+\s*)*[^( \[* | \]* | \(* | \)* | \-* | \-* | \,* | *)]";
        public const string EMPTY_SPACE_PATTERN = @"^\s";

        private IParser parser;

        private int positionInLine = 0;

        public Lexer(IParser parser)
        {
            this.parser = parser;
        }

        public void Lex(IList<object> keys, IList<IList<object>> dataValues)
        {
            parser.Start();

            // define property container
            parser.Name("root");

            foreach(var values in dataValues)
            {
                for(int i = 0; i < keys.Count; i ++)
                {
                    var key = keys[i];
                    var value = values[i];

                    LexKey(key.ToString());
                    LexValue(value.ToString());

                    #if DEBUG
                    Console.WriteLine($"Lexed data with key: {key} and value: {value}\n");
                    #endif
                }
            }

             parser.End();
        }

        private void LexKey(string key)
        {
            parser.Name(key);
        }

        private void LexValue(string value)
        {
            parser.StartProperty();

            positionInLine = 0;

            while(positionInLine < value.Length)
            {
                LexToken(value);
            }

            parser.EndProperty();
        }

        private void LexToken(string value)
        {
            if (!TokenFound(value))
            {
                throw new ArgumentException($"[Lexer] value is not a valid token, please read the documentation or tests for the supported charcters: {value}");
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
                var regex = Regex.Match(subString, EMPTY_SPACE_PATTERN);
                if (regex.Success)
                {
                    positionInLine += regex.Value.Length;

                    for (int i = 0; i < regex.Value.Length; i++)
                    {
                        parser.WhiteSpace();
                    }
                }

                return regex.Success;
            }
            
            return false;
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
            
            return false;
        }

        private bool IsName(string value)
        {
            if (positionInLine < value.Length)
            {
                var subString = value.Substring(positionInLine);
                var regex = Regex.Match(subString, WORD_PATTERN);
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

