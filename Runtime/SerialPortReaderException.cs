using System;

namespace SerialPortReader
{
    public class SerialPortReaderException : Exception
    {
        public SerialPortReaderException()
        {
            
        }

        public SerialPortReaderException(string message)
            : base(message)
        {
            
        }
    }
}