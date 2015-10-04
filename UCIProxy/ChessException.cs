using System;

namespace UCIProxy
{
    public class ChessException : Exception
    {
        public ChessException(string message) : base(message)
        {
            
        }

        public ChessException(string message, Exception inner) 
            : base(message, inner)
        {

        }
    }
}
