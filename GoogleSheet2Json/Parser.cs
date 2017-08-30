using System;

namespace GoogleSheet2Json
{
    /// <summary>
    /// Its basically a token collector that will receive commands from the lexer and 
    /// interact with the builder to build the JSON file and the data structure for the code generator
    /// </summary>

    public class Parser : IParser
    {
        private ParserState currentState;
        private string setName;

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

        private Transition[] transitions = new Transition[]
        {
            // states for starting and ending the parser
            new Transition(ParserState.DATA,                ParserEvent.START,                  ParserState.PROP_CONTAINER_DEF,     null),
            new Transition(ParserState.PROP_CONTAINER_DEF,  ParserEvent.SET_NAME,               ParserState.PROP_DEF,               null),
            new Transition(ParserState.PROP_DEF,            ParserEvent.END,                    ParserState.DATA,                   null),
            new Transition(ParserState.PROP_DEF,            ParserEvent.START_PROP,             ParserState.PROP,                   null),
            new Transition(ParserState.PROP_DEF,            ParserEvent.END_PROP,               ParserState.PROP_DEF,               null),
            new Transition(ParserState.PROP,                ParserEvent.START_PROP,             ParserState.PROP,                   null),

            // states for building an array or single value
            new Transition(ParserState.PROP_DEF,            ParserEvent.SET_NAME,               ParserState.PROP,                   null),
            new Transition(ParserState.PROP,                ParserEvent.SET_NAME,               ParserState.PROP_VALUE,             null),
            new Transition(ParserState.PROP_VALUE,          ParserEvent.SET_COMMA,              ParserState.PROP,                   null),
            new Transition(ParserState.PROP_VALUE,          ParserEvent.END_PROP,               ParserState.PROP_DEF,               null),
            new Transition(ParserState.PROP,                ParserEvent.END_PROP,               ParserState.PROP_DEF,               null),

            // states for building range value
            new Transition(ParserState.PROP_VALUE,          ParserEvent.SET_DASH,               ParserState.PROP_RANGE,             null),
            new Transition(ParserState.PROP_RANGE,          ParserEvent.SET_NAME,               ParserState.PROP,                   null),

            // states for building collections
            new Transition(ParserState.PROP,                ParserEvent.OPEN_SQUARE_BRACE,      ParserState.PROP_COLL,              null),
            new Transition(ParserState.PROP_COLL,           ParserEvent.SET_NAME,               ParserState.PROP_COLL_VALUE,        null),
            new Transition(ParserState.PROP_COLL_VALUE,     ParserEvent.SET_COMMA,              ParserState.PROP_COLL,              null),
            new Transition(ParserState.PROP_COLL_VALUE,     ParserEvent.CLOSE_SQUARE_BRACE,     ParserState.PROP_DEF,               null),
            new Transition(ParserState.PROP_COLL,           ParserEvent.END_PROP,               ParserState.PROP_DEF,               null),

            // states for building a map
            new Transition(ParserState.PROP,                ParserEvent.OPEN_BRACE,             ParserState.PROP_MAP,               null),
            new Transition(ParserState.PROP_MAP,            ParserEvent.SET_NAME,               ParserState.PROP_MAP_KEY,           null),
            new Transition(ParserState.PROP_MAP_KEY,        ParserEvent.SET_COMMA,              ParserState.PROP_MAP_COMMA,         null),
            new Transition(ParserState.PROP_MAP_COMMA,      ParserEvent.SET_NAME,               ParserState.PROP_MAP_VALUE,         null),
            new Transition(ParserState.PROP_MAP_VALUE,      ParserEvent.CLOSE_BRACE,            ParserState.PROP,                   null),
        };

        private void HandleEvent(ParserEvent pEvent)
        {
            foreach(var transition in transitions)
            {
                if (transition.state.Equals(currentState) 
                    && transition.pEvent.Equals(pEvent))
                {
                    if (transition.action != null)
                        transition.action();

                    #if DEBUG
                    Console.WriteLine("[Parser] Change state from: " + currentState + " to: " + transition.nextState + " from event: " + pEvent);
                    #endif

                    currentState = transition.nextState;

                    return;
                }
            }

            throw new Exception(string.Format("[Parser] Received invalid event {0} when in state {1}", pEvent, currentState));
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

