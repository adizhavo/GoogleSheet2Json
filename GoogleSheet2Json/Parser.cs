namespace GoogleSheet2Json
{
    /// <summary>
    /// Its basically a token collector that will receive commands from the lexer and 
    /// interact with the builder to build the JSON file and the data structure to generate code
    /// </summary>

    public class Parser
    {
        public Parser()
        {
            
        }

        public void Start()
        {
            // handle { event
        }

        public void End()
        {
            // handle } event
        }

        public void Name(string name)
        {
            // handle "value" event
        }

        public void StartValue()
        {
            // handle : event
        }

        public void Comma()
        {
            // handle , event
        }

        public void OpenBrace()
        {
            // handle ( event
        }

        public void CloseBrace()
        {
            // handle ) event
        }

        public void OpenSquareBrackets()
        {
            // handle ] event
        }

        public void CloseSquareBrackets()
        {
            // handle [ event
        }

        public void Dash()
        {
            // handle - event   
        }
    }
}

