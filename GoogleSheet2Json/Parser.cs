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
        private Builder builder;

        public Parser(Builder builder)
        {
            this.builder = builder;
            BuildTransitionTable();
        }

        public void Start()
        {
            currentState = ParserState.DATA;
            setName = string.Empty;
            HandleEvent(ParserEvent.START);
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

        public void Dash()
        {
            HandleEvent(ParserEvent.SET_DASH);
        }

        public void WhiteSpace()
        {
            HandleEvent(ParserEvent.WHITE_SPACE);
        }

        private Transition[] transitions; 
        
        private void BuildTransitionTable()
        {
            transitions = new Transition[]
            {
                // states for starting and ending the parser
                new Transition(ParserState.DATA,                ParserEvent.START,                  ParserState.PROP_CONTAINER_DEF,     null),
                new Transition(ParserState.PROP_CONTAINER_DEF,  ParserEvent.SET_NAME,               ParserState.PROP_DEF,               () => builder.SetRootName(setName)),
                new Transition(ParserState.PROP_DEF,            ParserEvent.END,                    ParserState.DATA,                   builder.EndBuild),
                new Transition(ParserState.PROP_DEF,            ParserEvent.START_PROP,             ParserState.PROP,                   null),
                new Transition(ParserState.PROP_DEF,            ParserEvent.END_PROP,               ParserState.PROP_DEF,               builder.EndProperty),
                new Transition(ParserState.PROP,                ParserEvent.START_PROP,             ParserState.PROP,                   null),
                new Transition(ParserState.PROP_DEF,            ParserEvent.SET_NAME,               ParserState.PROP,                   () => builder.StartProperty(setName)),
    
                // states for building a single value 
                new Transition(ParserState.PROP,                ParserEvent.SET_NAME,               ParserState.PROP_VALUE,             () => builder.AddField(setName)),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.SET_NAME,               ParserState.PROP_VALUE,             () => builder.AddField(setName)),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.OPEN_BRACE,             ParserState.PROP_VALUE,             () => builder.AddField(")")),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.CLOSE_BRACE,            ParserState.PROP_VALUE,             () => builder.AddField("(")),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.OPEN_SQUARE_BRACE,      ParserState.PROP_VALUE,             () => builder.AddField("[")),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.CLOSE_SQUARE_BRACE,     ParserState.PROP_VALUE,             () => builder.AddField("]")),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.SET_COMMA,              ParserState.PROP_VALUE,             () => builder.AddField(",")),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.WHITE_SPACE,            ParserState.PROP_VALUE,             () => builder.AddField(" ")),
                new Transition(ParserState.PROP_VALUE,          ParserEvent.END_PROP,               ParserState.PROP_DEF,               builder.EndProperty),
                new Transition(ParserState.PROP,                ParserEvent.END_PROP,               ParserState.PROP_DEF,               builder.EndProperty),
    
                // states for building range value
                new Transition(ParserState.PROP_VALUE,          ParserEvent.SET_DASH,               ParserState.PROP_RANGE,             () => builder.TryAddMinRange("-")),
                new Transition(ParserState.PROP_RANGE,          ParserEvent.SET_NAME,               ParserState.PROP_VALUE,             () => builder.TryAddMaxRange(setName)),
                new Transition(ParserState.PROP_RANGE,          ParserEvent.WHITE_SPACE,            ParserState.PROP_RANGE,             null),
                
                // states for building collections
                new Transition(ParserState.PROP,                ParserEvent.OPEN_SQUARE_BRACE,      ParserState.PROP_COLL,              builder.StartCollection),
                new Transition(ParserState.PROP_COLL,           ParserEvent.SET_NAME,               ParserState.PROP_COLL_VALUE,        () => builder.AddCollectionElement(setName)),
                new Transition(ParserState.PROP_COLL_VALUE,     ParserEvent.SET_COMMA,              ParserState.PROP_COLL,              null),
                new Transition(ParserState.PROP_COLL_VALUE,     ParserEvent.CLOSE_SQUARE_BRACE,     ParserState.PROP_DEF,               null),
                new Transition(ParserState.PROP_COLL,           ParserEvent.END_PROP,               ParserState.PROP_DEF,               builder.EndProperty),
                
                // states for building a map
                new Transition(ParserState.PROP,                ParserEvent.OPEN_BRACE,             ParserState.PROP_MAP,               builder.StartMap),
                new Transition(ParserState.PROP_MAP,            ParserEvent.SET_NAME,               ParserState.PROP_MAP_KEY,           () => builder.AddKey(setName)),
                new Transition(ParserState.PROP_MAP_KEY,        ParserEvent.SET_COMMA,              ParserState.PROP_MAP_COMMA,         null),
                new Transition(ParserState.PROP_MAP_COMMA,      ParserEvent.SET_NAME,               ParserState.PROP_MAP_VALUE,         () => builder.AddValue(setName)),
                new Transition(ParserState.PROP_MAP_VALUE,      ParserEvent.CLOSE_BRACE,            ParserState.PROP,                   null),
            };
        }
        

        private void HandleEvent(ParserEvent pEvent)
        {
            foreach(var transition in transitions)
            {
                if (transition.state.Equals(currentState) && transition.pEvent.Equals(pEvent))
                {
                    if (transition.action != null)
                        transition.action();

                    #if DEBUG
                    Console.WriteLine($"[Parser] Change state from: {currentState} to: {transition.nextState} from event: {pEvent}");
                    #endif

                    currentState = transition.nextState;

                    return;
                }
            }

            if (pEvent != ParserEvent.WHITE_SPACE)
            {
                throw new Exception($"[Parser] Received invalid event {pEvent} when in state {currentState}");
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

