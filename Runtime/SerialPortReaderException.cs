using System;

namespace SerialPortMonitor
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