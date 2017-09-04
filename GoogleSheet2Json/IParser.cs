using System;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Its basically a token collector that will receive commands from the lexer and 
    /// interact with the builder to build the JSON file and the data structure for the code generator
    /// </summary>

    // This interface is required to create mocks for tests

    public interface IParser
    {
        void Start();
        void End();
        void Name(string name);
        void StartProperty();
        void EndProperty();
        void Comma();
        void OpenBrace();
        void CloseBrace();
        void OpenSquareBrackets();
        void CloseSquareBrackets();
        void Range();
        void WhiteSpace();
    }

    public enum ParserEvent
    {
        START,
        END,
        START_PROP,
        END_PROP,
        SET_NAME,
        SET_COMMA,
        OPEN_BRACE,
        CLOSE_BRACE,
        OPEN_SQUARE_BRACE,
        CLOSE_SQUARE_BRACE,
        SET_RANGE,
        WHITE_SPACE
    }

    public enum ParserState
    {
        DATA,
        PROP_CONTAINER_DEF,
        PROP_DEF,
        PROP,
        PROP_END,
        PROP_VALUE,
        PROP_RANGE,
        PROP_COLL,
        PROP_COLL_VALUE,
        PROP_MAP,
        PROP_MAP_KEY,
        PROP_MAP_COMMA,
        PROP_MAP_VALUE,
    }
}
