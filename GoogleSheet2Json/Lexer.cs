﻿using System;
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
        private IParser parser;

        private int positionInLine = 0;

        public Lexer(IParser parser)
        {
            this.parser = parser;
        }

        public void Lex(IList<object> keys, IList<IList<object>> dataValues)
        {
            if (keys != null && dataValues != null)
            {
                parser.Start();

                // define property container
                parser.Name("root");

                foreach (var values in dataValues)
                {
                    if (values.Count > 0)
                    {
                        parser.StartProperty();

                        for (int i = 0; i < keys.Count; i++)
                        {
                            if (i < values.Count)
                            {
                                var key = keys[i];
                                var value = values[i];

                                if (!string.Equals(key, StringConstants.COMMENT_ANNOTATION))
                                {
                                    parser.StartField();

                                    LexKey(key.ToString());
                                    LexValue(value.ToString());

                                    parser.EndField();

                                    #if DEBUG
                                    Console.WriteLine($"Lexed data with key: {key} and value: {value}\n");
                                    #endif
                                }
                            }
                        }

                        parser.EndProperty();
                    }
                }

                parser.End();
            }
        }

        private void LexKey(string key)
        {
            parser.Name(key);
        }

        private void LexValue(string value)
        {
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
                throw new ArgumentException($"[Lexer] value is not a valid token, character nr {positionInLine}, please read the documentation or tests for the supported charcters: {value}");
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
                var regex = Regex.Match(subString, StringConstants.EMPTY_SPACE_PATTERN);
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
                    case StringConstants.COMMA :                parser.Comma(); break;
                    case StringConstants.RANGE_CHAR :           parser.Range(); break;
                    case StringConstants.OPEN_BRACKET :         parser.OpenBrace(); break;
                    case StringConstants.CLOSE_BRACKET :        parser.CloseBrace(); break;
                    case StringConstants.OPEN_SQUARE_BRACKET :  parser.OpenSquareBrackets(); break;
                    case StringConstants.CLOSE_SQUARE_BRACKET : parser.CloseSquareBrackets(); break;
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
                var regex = Regex.Match(subString, StringConstants.WORD_PATTERN);
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

