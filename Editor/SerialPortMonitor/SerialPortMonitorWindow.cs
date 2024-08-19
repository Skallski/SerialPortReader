using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEditor;
using UnityEngine;

namespace SerialPortReader.Editor.SerialPortMonitor
{
    public class SerialPortMonitorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        private string _portName;
        private int _baudRate;
        private SerialPort _port;
        private readonly List<string> _portMessages = new List<string>();
        
        internal static void OpenWindow(string portName, int baudRate)
        {
            SerialPortMonitorWindow window = GetWindow<SerialPortMonitorWindow>();
            window.titleContent = new GUIContent($"Serial Port Monitor: {portName}",
                EditorGUIUtility.IconContent("d_SignalReceiver Icon").image);
            
            window.Show();
            window.Setup(portName, baudRate);
        }

        private void Setup(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;

            ClosePort();
            
            if (_port == null)
            {
                OpenPort();
            }
        }

        private void OnDestroy()
        {
            ClosePort();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, 
                GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            foreach (string message in _portMessages)
            {
                EditorGUILayout.LabelField(message);
            }

            EditorGUILayout.EndScrollView();
        }

        private void OpenPort()
        {
            _portMessages.Clear();

            try
            {
                _port = new SerialPort(_portName, _baudRate);
                _port.Open();
                
                if (_port.IsOpen)
                {
                    _portMessages.Add($"Listening to {_portName} with {_baudRate} baud rate...");
                    EditorApplication.update += OnUpdate;
                }
            }
            catch (Exception e)
            {
                _portMessages.Add(e.Message);
            }
        }

        private void ClosePort()
        {
            if (_port is { IsOpen: true })
            {
                _port.Close();
                _port = null;
            }
            
            EditorApplication.update -= OnUpdate;
            
            _portMessages.Clear();
        }

        private void OnUpdate()
        {
            if (_port is not { IsOpen: true })
            {
                return;
            }

            string message = GetNewestMessageFromPort();
            if (string.IsNullOrEmpty(message) == false)
            {
                _portMessages.Add(message);
            }
            
            Repaint();
        }

        private string GetNewestMessageFromPort()
        {
            if (_port is not { IsOpen: true })
            {
                return null;
            }
            
            string existingData = _port.ReadExisting();
            if (string.IsNullOrEmpty(existingData))
            {
                return null;
            }

            DateTime now = DateTime.Now;
            string currentTime = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";

            string processedData = existingData.Replace("\n", string.Empty);
            return $"[{currentTime}]: {processedData}";
        }
    }
}