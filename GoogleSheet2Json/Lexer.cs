using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net.Config;

// ReSharper disable EmptyForStatement

namespace GoogleSheet2Json
{
    /// <summary>
    /// Will lex the stream of data and deliver commands to the parser
    /// </summary>

    public class Lexer
    {
        private readonly IParser parser;

        private int positionInLine = 0;

        public Lexer(IParser parser)
        {
            this.parser = parser;
        }

        public void Lex(IList<object> keys, IList<IList<object>> dataValues, ExportConfig exportConfig)
        {
            if (keys != null && dataValues != null)
            {
                if (exportConfig.isArrayOfObjects)
                {
                    LexArrayOfValues(keys, dataValues, exportConfig);     
                }
                else if (exportConfig.isSingleObject)
                {
                    LexSingleObjectValues(dataValues, exportConfig);
                }
            }
        }

        private void LexSingleObjectValues(IList<IList<object>> dataValues, ExportConfig exportConfig)
        {
            parser.StartSingleObject();

            IList<object> propDefinition = new List<object>();
            IList<object> propValues = new List<object>();

            foreach (var values in dataValues)
            {
                // object is only a key value pair between property definition and its value
                if (values.Count >= 2)
                {
                    propDefinition.Add(values[0]);
                    propValues.Add(values[1]);
                }
            }

            for (int i = 0; i < propDefinition.Count; i++)
            {
                var key = propDefinition[i].ToString().Trim();
                var value = propValues[i];

                if (!string.Equals(key, StringConstants.COMMENT_ANNOTATION))
                {
                    Logger.DebugLogLine($"Lexing data with key: {key} and value: {value}");
                    
                    parser.StartField();

                    LexKey(key);

                    if (exportConfig.literalKeys.Contains(key))
                    {
                        parser.Name(value.ToString());
                    }
                    else
                    {
                        LexValue(value.ToString());
                    }
                    
                    parser.EndField();
                    
                    Logger.DebugLogLine("Data lexed successfully.");
                }
            }

            parser.End();
        }
        
        private void LexArrayOfValues(IList<object> keys, IList<IList<object>> dataValues, ExportConfig exportConfig)
        {
            parser.StartArrayOfObjects();
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
                            var key = keys[i].ToString().Trim();
                            var value = values[i];

                            if (!string.Equals(key, StringConstants.COMMENT_ANNOTATION) && !string.IsNullOrEmpty(value.ToString()))
                            {
                                Logger.DebugLogLine($"Lexing data with key: {key} and value: {value}");
                                
                                parser.StartField();

                                LexKey(key);

                                if (exportConfig.literalKeys.Contains(key))
                                {
                                    parser.Name(value.ToString());
                                }
                                else
                                {
                                    LexValue(value.ToString());   
                                }

                                parser.EndField();

                                Logger.DebugLogLine("Data lexed successfully.");
                            }
                        }
                    }

                    parser.EndProperty();
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
                throw new ArgumentException($"[Lexer] value is not a valid token, character nr {positionInLine}, please read the documentation or tests for the supported characters: {value}");
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

