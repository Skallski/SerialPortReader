using UnityEngine;
using UnityEngine.Events;

namespace SerialPortReader
{
    public class SerialPortReaderMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private bool _useHighestComPort = true;
        [SerializeField] private string _serialPortName;
        [SerializeField] private int _baudRate = 9600;
        [SerializeField] private float _dataReadDelay = 0;
        
        [SerializeField] private UnityEvent _onSerialPortOpened;
        [SerializeField] private UnityEvent<string> _onSerialPortDataReceived;
        [SerializeField] private UnityEvent _onSerialPortClosed;

        private void Start()
        {
            if (_useHighestComPort)
            {
                _serialPortName = SerialPortUtils.GetHighestComPortName();
            }

            try
            {
                SerialPortReader.Create(this, _serialPortName, _baudRate, OnPortOpened, OnDataReceived, 
                    OnPortClosed, _dataReadDelay);
            }
            catch (SerialPortReaderException e)
            {
                Debug.LogError(e.Message);
                OnSerialPortOpenError();
            }
        }

        private void OnPortOpened()
        {
            _onSerialPortOpened?.Invoke();
        }

        private void OnDataReceived(string serialPortLine)
        {
            _onSerialPortDataReceived?.Invoke(serialPortLine);
        }

        private void OnPortClosed()
        {
            _onSerialPortClosed?.Invoke();
        }

        private void OnDestroy()
        {
            try
            {
                SerialPortReader.Remove(_serialPortName);
            }
            catch (SerialPortReaderException e)
            {
                Debug.LogError(e.Message);
                OnSerialPortCloseError();
            }
        }

        protected virtual void OnSerialPortOpenError()
        {
            
        }

        protected virtual void OnSerialPortCloseError()
        {
            
        }
    }
}