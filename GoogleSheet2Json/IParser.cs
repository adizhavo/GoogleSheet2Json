﻿using System;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Its basically a token collector that will receive commands from the lexer and 
    /// interact with the builder to build the JSON file and the data structure for the code generator
    /// </summary>

    // This interface is required to create mocks for tests
    public interface IParser
    {
        void StartSingleObject();
        void StartArrayOfObjects();
        void End();
        void Name(string name);
        void StartProperty();
        void EndProperty();
        void StartField();
        void EndField();
        void Comma();
        void OpenBracket();
        void CloseBracket();
        void OpenCurlyBracket();
        void CloseCurlyBracket();
        void OpenSquareBracket();
        void CloseSquareBracket();
        void Range();
        void WhiteSpace();
    }

    public enum ParserEvent
    {
        START,
        END,
        ARRAY,
        OBJECT,
        START_PROP,
        END_PROP,
        START_FIELD,
        END_FIELD,
        SET_NAME,
        SET_COMMA,
        OPEN_BRACKET,
        CLOSE_BRACKET,
        OPEN_CURLY_BRACKET,
        CLOSE_CURLY_BRACKET,
        OPEN_SQUARE_BRACKET,
        CLOSE_SQUARE_BRACKET,
        SET_RANGE,
        WHITE_SPACE
    }

    public enum ParserState
    {
        START,
        OBJECT,
        OBJECT_PROP_START,
        ARRAY,
        ARR_PROP_START,
        PROP_END,
        ARR_PROP_DEF,
        FIELD_DEF,
        FIELD,
        FIELD_VALUE,
        FIELD_RANGE,
        FIELD_MAP,
        FIELD_MAP_KEY,
        FIELD_MAP_VALUE,
        FIELD_MAP_COMMA,
        FIELD_COLL,
        FIELD_COLL_VALUE,
        FIELD_COLL_COMMA,
        END
    }
}