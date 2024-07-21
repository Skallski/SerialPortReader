using System.IO.Ports;
using UnityEditor;
using UnityEngine;

namespace SerialPortMonitor.Editor.SerialPortMonitor
{
    public class SerialPortSelectWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private int _baudRate = 9600;
        
        [MenuItem("Window/Serial Port Monitor")]
        private static void OpenWindow()
        {
            SerialPortSelectWindow window = GetWindow<SerialPortSelectWindow>();
            window.titleContent = new GUIContent("Serial Port Monitor", 
                EditorGUIUtility.IconContent("d_SignalReceiver Icon").image);
            
            window.Show();
        }

        private void OnGUI()
        {
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length == 0)
            {
                EditorGUILayout.LabelField(new GUIContent("No available serial ports found!"));
            }
            else
            {
                _baudRate = EditorGUILayout.IntField(new GUIContent("Baud Rate"), _baudRate);
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField(new GUIContent("Available serial ports:"));

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                foreach (string portName in portNames)
                {
                    if (GUILayout.Button(new GUIContent(portName)))
                    {
                        SerialPortMonitorWindow.OpenWindow(portName, _baudRate);
                    }
                }
            
                EditorGUILayout.EndScrollView();
            }
        }
    }
}