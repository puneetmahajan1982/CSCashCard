using System;

namespace VirtualCashCard
{
    /// <summary>
    /// Custom Invalid Pin Exception 
    /// </summary>
    public class InvalidPinException : Exception
    {
        public InvalidPinException(string message) : base(message)
        {
        }
    }
}