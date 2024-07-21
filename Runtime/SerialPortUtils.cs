using System.IO.Ports;

namespace SerialPortMonitor
{
    public static class SerialPortUtils
    {
        /// <summary>
        /// Returns COMX serial port name, where X is highest number of available port
        /// </summary>
        /// <returns></returns>
        public static string GetHighestComPortName()
        {
            int highestNumber = 0;
                
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                return string.Empty;
            }
                
            foreach (string port in ports)
            {
                string numberString = port[3..]; // Assuming "COMX" format, where X is the number
                if (int.TryParse(numberString, out int number))
                {
                    if (number > highestNumber)
                    {
                        highestNumber = number;
                    }
                }
            }

            return $"COM{highestNumber}";
        }
    }
}