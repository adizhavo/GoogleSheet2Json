using System;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Its basically a token collector that will receive commands from the lexer and 
    /// interact with the builder to build the JSON file and the data structure for the code generator
    /// </summary>

    public class Parser : IParser
    {
        public ParserState currentState { get; private set; }

        private string setName;
        private readonly IBuilder builder;

        public Parser(IBuilder builder)
        {
            this.builder = builder;
            BuildTransitionTable();
        }

        public void StartSingleObject()
        {
            currentState = ParserState.START;
            setName = string.Empty;
            HandleEvent(ParserEvent.OBJECT);
        }
        
        public void StartArrayOfObjects()
        {
            currentState = ParserState.START;
            setName = string.Empty;
            HandleEvent(ParserEvent.ARRAY);
        }

        public void End()
        {
            HandleEvent(ParserEvent.END);
        }

        public void Name(string name)
        {
            setName = name.Trim();
            HandleEvent(ParserEvent.SET_NAME);
        }

        public void StartProperty()
        {
            HandleEvent(ParserEvent.START_PROP);
        }

        public void EndProperty()
        {
            HandleEvent(ParserEvent.END_PROP);
        }
        
        public void StartField()
        {
            HandleEvent(ParserEvent.START_FIELD);
        }

        public void EndField()
        {
            HandleEvent(ParserEvent.END_FIELD); 
        }

        public void Comma()
        {
            HandleEvent(ParserEvent.SET_COMMA);
        }

        public void OpenBrace()
        {
            HandleEvent(ParserEvent.OPEN_BRACE);
        }

        public void CloseBrace()
        {
            HandleEvent(ParserEvent.CLOSE_BRACE);
        }

        public void OpenSquareBrackets()
        {
            HandleEvent(ParserEvent.OPEN_SQUARE_BRACE);
        }

        public void CloseSquareBrackets()
        {
            HandleEvent(ParserEvent.CLOSE_SQUARE_BRACE);
        }

        public void Range()
        {
            HandleEvent(ParserEvent.SET_RANGE);
        }

        public void WhiteSpace()
        {
            HandleEvent(ParserEvent.WHITE_SPACE);
        }

        private Transition[] transitions; 
        
        private void BuildTransitionTable()
        {
            // TODO : state transitions can be improved, this is the first version and may have some edge cases
            // tests are covering most of the usage of the parser, please report any error
            
            transitions = new[]
            {
                new Transition(ParserState.START,            ParserEvent.OBJECT,             ParserState.OBJECT,               builder.StartBuildSingleObject),
                new Transition(ParserState.START,            ParserEvent.ARRAY,              ParserState.ARRAY,                builder.StartBuildArrayOfObjects),
                new Transition(ParserState.OBJECT,           ParserEvent.END,                ParserState.END,                  builder.EndBuild),
                new Transition(ParserState.ARRAY,            ParserEvent.END,                ParserState.END,                  builder.EndBuild),
                // Specific case for the Object state
                new Transition(ParserState.OBJECT,           ParserEvent.START_FIELD,        ParserState.FIELD_DEF,            null),
                
                // Specific case for the Array State
                new Transition(ParserState.ARRAY,            ParserEvent.SET_NAME,           ParserState.ARR_PROP_DEF,         () => builder.SetRootName(setName)),
                new Transition(ParserState.ARR_PROP_DEF,     ParserEvent.START_PROP,         ParserState.FIELD_DEF,            builder.StartProperty),
                new Transition(ParserState.FIELD_DEF,        ParserEvent.START_FIELD,        ParserState.FIELD_DEF,            null), 
                new Transition(ParserState.FIELD_DEF,        ParserEvent.SET_NAME,           ParserState.FIELD,                () => builder.StartField(setName)),
                new Transition(ParserState.FIELD_DEF,        ParserEvent.END_PROP,           ParserState.ARR_PROP_DEF,         builder.EndProperty),
                new Transition(ParserState.FIELD_DEF,        ParserEvent.END,                ParserState.END,                  builder.EndBuild),
                new Transition(ParserState.ARR_PROP_DEF,     ParserEvent.END,                ParserState.END,                  builder.EndBuild),
                new Transition(ParserState.FIELD,            ParserEvent.END_FIELD,          ParserState.FIELD_DEF,            builder.EndField),
                
                new Transition(ParserState.FIELD,            ParserEvent.SET_NAME,           ParserState.FIELD_VALUE,          () => builder.SetField(setName)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.SET_NAME,           ParserState.FIELD_VALUE,          () => builder.SetField(setName)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.WHITE_SPACE,        ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.EMPTY_SPACE)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.OPEN_BRACE,         ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.OPEN_BRACKET)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.CLOSE_BRACE,        ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.CLOSE_BRACKET)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.OPEN_SQUARE_BRACE,  ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.OPEN_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.CLOSE_SQUARE_BRACE, ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.CLOSE_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.SET_COMMA,          ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.COMMA)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.SET_RANGE,          ParserState.FIELD_RANGE,          () => builder.TryAddMinRange(StringConstants.RANGE_CHAR)),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.SET_NAME,           ParserState.FIELD_VALUE,          () => builder.TryAddMaxRange(setName)),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.WHITE_SPACE,        ParserState.FIELD_RANGE,          null),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.OPEN_BRACE,         ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.OPEN_BRACKET)),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.CLOSE_BRACE,        ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.CLOSE_BRACKET)),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.OPEN_SQUARE_BRACE,  ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.OPEN_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.CLOSE_SQUARE_BRACE, ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.CLOSE_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_RANGE,      ParserEvent.SET_COMMA,          ParserState.FIELD_VALUE,          () => builder.SetField(StringConstants.COMMA)),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.END_FIELD,          ParserState.FIELD_DEF,            builder.EndField),
                new Transition(ParserState.FIELD_VALUE,      ParserEvent.START_FIELD,        ParserState.FIELD_DEF,            builder.EndField),
                
                new Transition(ParserState.FIELD,            ParserEvent.OPEN_SQUARE_BRACE,  ParserState.FIELD_COLL_VALUE,     builder.StartCollection),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.SET_NAME,           ParserState.FIELD_COLL_VALUE,     () => builder.SetField(setName)),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.SET_RANGE,          ParserState.FIELD_COLL_VALUE,     () => builder.SetField(StringConstants.RANGE_CHAR)),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.WHITE_SPACE,        ParserState.FIELD_COLL_VALUE,     () => builder.SetField(StringConstants.EMPTY_SPACE)),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.OPEN_BRACE,         ParserState.FIELD_COLL_VALUE,     () => builder.SetField(StringConstants.OPEN_BRACKET)),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.CLOSE_BRACE,        ParserState.FIELD_COLL_VALUE,     () => builder.SetField(StringConstants.CLOSE_BRACKET)),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.SET_COMMA,          ParserState.FIELD_COLL_COMMA,     builder.AddFieldToCollection),
                new Transition(ParserState.FIELD_COLL_COMMA, ParserEvent.SET_NAME,           ParserState.FIELD_COLL_VALUE,     () => builder.SetField(setName)),
                new Transition(ParserState.FIELD_COLL_COMMA, ParserEvent.SET_RANGE,          ParserState.FIELD_COLL_VALUE,     null),
                new Transition(ParserState.FIELD_COLL_COMMA, ParserEvent.WHITE_SPACE,        ParserState.FIELD_COLL_COMMA,     null),
                new Transition(ParserState.FIELD_COLL_VALUE, ParserEvent.CLOSE_SQUARE_BRACE, ParserState.FIELD_COLL,           builder.AddFieldToCollection),
                new Transition(ParserState.FIELD_COLL,       ParserEvent.END_FIELD,          ParserState.FIELD_DEF,            builder.EndField),
                
                new Transition(ParserState.FIELD,            ParserEvent.OPEN_BRACE,         ParserState.FIELD_MAP_KEY,        builder.StartMap),
                new Transition(ParserState.FIELD_MAP_KEY,    ParserEvent.SET_NAME,           ParserState.FIELD_MAP_KEY,        () => builder.AddKey(setName)),
                new Transition(ParserState.FIELD_MAP_KEY,    ParserEvent.SET_RANGE,          ParserState.FIELD_MAP_KEY,        () => builder.AppendToKey(StringConstants.RANGE_CHAR)),
                new Transition(ParserState.FIELD_MAP_KEY,    ParserEvent.WHITE_SPACE,        ParserState.FIELD_MAP_KEY,        () => builder.AppendToKey(StringConstants.EMPTY_SPACE)),
                new Transition(ParserState.FIELD_MAP_KEY,    ParserEvent.OPEN_SQUARE_BRACE,  ParserState.FIELD_MAP_KEY,        () => builder.AppendToKey(StringConstants.OPEN_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_MAP_KEY,    ParserEvent.CLOSE_SQUARE_BRACE, ParserState.FIELD_MAP_KEY,        () => builder.AppendToKey(StringConstants.CLOSE_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_MAP_KEY,    ParserEvent.SET_COMMA,          ParserState.FIELD_MAP_COMMA,      null),
                new Transition(ParserState.FIELD_MAP_COMMA,  ParserEvent.WHITE_SPACE,        ParserState.FIELD_MAP_COMMA,      null),
                new Transition(ParserState.FIELD_MAP_COMMA,  ParserEvent.SET_NAME,           ParserState.FIELD_MAP_VALUE,      () => builder.AddValue(setName)),
                new Transition(ParserState.FIELD_MAP_COMMA,  ParserEvent.SET_RANGE,          ParserState.FIELD_MAP_VALUE,      null),
                new Transition(ParserState.FIELD_MAP_VALUE,  ParserEvent.CLOSE_BRACE,        ParserState.FIELD_MAP,            null),
                new Transition(ParserState.FIELD_MAP_VALUE,  ParserEvent.SET_RANGE,          ParserState.FIELD_MAP_VALUE,      () => builder.AppendToValue(StringConstants.RANGE_CHAR)),
                new Transition(ParserState.FIELD_MAP_VALUE,  ParserEvent.WHITE_SPACE,        ParserState.FIELD_MAP_VALUE,      () => builder.AppendToValue(StringConstants.EMPTY_SPACE)),
                new Transition(ParserState.FIELD_MAP_VALUE,  ParserEvent.OPEN_SQUARE_BRACE,  ParserState.FIELD_MAP_VALUE,      () => builder.AppendToValue(StringConstants.OPEN_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_MAP_VALUE,  ParserEvent.CLOSE_SQUARE_BRACE, ParserState.FIELD_MAP_VALUE,      () => builder.AppendToValue(StringConstants.CLOSE_SQUARE_BRACKET)),
                new Transition(ParserState.FIELD_MAP_VALUE,  ParserEvent.SET_NAME,           ParserState.FIELD_MAP_VALUE,      () => builder.AppendToValue(setName)),
                new Transition(ParserState.FIELD_MAP,        ParserEvent.END_FIELD,          ParserState.FIELD_DEF,            builder.EndField),
                new Transition(ParserState.FIELD_MAP,        ParserEvent.OPEN_BRACE,         ParserState.FIELD_MAP_KEY,        builder.StartMap)
            };
        }
        

        private void HandleEvent(ParserEvent pEvent)
        {
            foreach(var transition in transitions)
            {
                if (transition.state.Equals(currentState) && transition.pEvent.Equals(pEvent))
                {
                    transition.action?.Invoke();
                    Logger.DebugLogLine($"[Parser] Change state from: {currentState} to: {transition.nextState} from event: {pEvent}");
                    currentState = transition.nextState;
                    return;
                }
            }

            if (pEvent != ParserEvent.WHITE_SPACE)
            {
                throw new Exception($"[Parser] Received invalid event {pEvent} when in state {currentState}, this is an edge case, please contact the developer");
            }
        }

        public class Transition
        {
            public ParserState state;
            public ParserEvent pEvent;
            public ParserState nextState;
            public Action action;

            public Transition(ParserState state, ParserEvent pEvent, ParserState nextState, Action action)
            {
                this.state = state;
                this.pEvent = pEvent;
                this.nextState = nextState;
                this.action = action;
            }
        }
    }
}

