using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

namespace SerialPortMonitor
{
    /// <summary>
    /// This class allows reading data from serial ports
    /// </summary>
    public static class SerialPortReader
    {
        /// <summary>
        /// This class represents single serial port data reading
        /// </summary>
        private class SerialPortData
        {
            private static readonly HashSet<SerialPortData> Instances = new HashSet<SerialPortData>();

            private SerialPort _serialPort;
            private MonoBehaviour _caller;
            private Action<string> _dataReadCallback;
            private Action _portOpenedCallback;
            private Action _portClosedCallback;
            private YieldInstruction _dataReadDelay;
            private bool _isCoroutineRunning;

            private string SerialPortName => _serialPort?.PortName ?? string.Empty;
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="portName"></param>
            /// <returns></returns>
            internal static SerialPortData GetPortByName(string portName)
            {
                return Instances.FirstOrDefault(portData => portData.SerialPortName.Equals(portName));
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="portName"></param>
            /// <returns></returns>
            internal static bool IsPortOpened(string portName)
            {
                return Instances.Any(portData => portData.SerialPortName.Equals(portName));
            }

            public SerialPortData(SerialPort serialPort, MonoBehaviour caller, Action portOpenedCallback,
                Action<string> dataReadCallback, Action portClosedCallback, float dataReadDelay = 0)
            {
                _serialPort = serialPort;
                _caller = caller;
                _dataReadCallback = dataReadCallback;
                _portOpenedCallback = portOpenedCallback;
                _portClosedCallback = portClosedCallback;
                _dataReadDelay = dataReadDelay > 0 
                    ? null 
                    : new WaitForSeconds(dataReadDelay);
            }

            /// <summary>
            /// Opens serial port and starts coroutine that reads data from that port
            /// </summary>
            public void Open()
            {
                _serialPort.Open();
                if (_serialPort.IsOpen)
                {
                    Debug.Log($"Port {SerialPortName} opened");

                    Instances.Add(this);
                    
                    _portOpenedCallback?.Invoke();
                    
                    _isCoroutineRunning = true;
                    _caller.StartCoroutine(ReadData_Coroutine());
                }
                
                // Local method
                // Coroutine that reads data from serial port in loop
                IEnumerator ReadData_Coroutine()
                {
                    while (_isCoroutineRunning)
                    {
                        if (_serialPort.IsOpen)
                        {
                            string existingData = _serialPort.ReadExisting();
                            if (string.IsNullOrEmpty(existingData) == false)
                            {
                                _dataReadCallback?.Invoke(existingData);
                            }
                        }

                        yield return _dataReadDelay;
                    }
                }
            }

            /// <summary>
            /// Stops coroutine that reads data from serial port and closes that port
            /// </summary>
            public void Close()
            {
                _isCoroutineRunning = false;
                
                _serialPort.Close();
                Instances.Remove(this);
                
                _portClosedCallback?.Invoke();
                
                Debug.Log($"Port {SerialPortName} closed");
            }
        }

        /// <summary>
        /// Starts reading data from serial port
        /// </summary>
        /// <param name="caller"> MonoBehaviour that handles the coroutine that reads data from the serial port </param>
        /// <param name="portName"> e.g. COM3 </param>
        /// <param name="baudRate"> e.g. 9600 </param>
        /// <param name="onDataRead"> Callback with the line read from the serial port </param>
        /// <param name="dataReadDelay"> Time interval in which the callback is executed </param>
        public static void Create(MonoBehaviour caller, string portName, int baudRate, Action onPortOpened, 
            Action<string> onDataRead, Action onPortClosed, float dataReadDelay = 0)
        {
            Debug.Log($"Attempting to open port {portName} with baud rate {baudRate}...");

            if (IsPortAvailable(portName, out string message) == false)
            {
                throw new SerialPortReaderException($"Port {portName} cannot be opened: {message}");
            }

            try
            {
                SerialPort serialPort = new SerialPort(portName, baudRate);
                SerialPortData serialPortData = 
                    new SerialPortData(serialPort, caller, onPortOpened, onDataRead, onPortClosed, dataReadDelay);
                serialPortData.Open();
            }
            catch (Exception e)
            {
                throw new SerialPortReaderException($"Port {portName} cannot be opened: {e.Message}");
            }

            // Local method
            // Checks if port with certain name is available
            static bool IsPortAvailable(string portName, out string message)
            {
                message = string.Empty;
                
                if (string.IsNullOrEmpty(portName))
                {
                    message = "Port name cannot be null or empty string!";
                    return false;
                }
            
                string[] portNames = SerialPort.GetPortNames();
                if (portNames.Any(pName => pName.Equals(portName)) == false)
                {
                    message = "Port not found!";
                    return false;
                }

                if (SerialPortData.IsPortOpened(portName))
                {
                    message = "Port is already opened!";
                    return false;
                }
            
                return true;
            }
        }

        /// <summary>
        /// Stops reading data from serial port
        /// </summary>
        /// <param name="portName"> e.g. COM3 </param>
        public static void Remove(string portName)
        {
            Debug.Log($"Attempting to close port {portName}...");

            if (string.IsNullOrEmpty(portName))
            {
                throw new SerialPortReaderException(
                    $"Port {portName} cannot be closed: Port name cannot be null or empty string!");
            }
            
            SerialPortData serialPortData = SerialPortData.GetPortByName(portName);
            if (serialPortData == null)
            {
                throw new SerialPortReaderException($"Port {portName} cannot be closed: Port not found!");
            }
            
            serialPortData.Close();
        }
    }
}