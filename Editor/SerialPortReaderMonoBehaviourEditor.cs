using UnityEditor;

namespace SerialPortReader.Editor
{
    [CustomEditor(typeof(SerialPortReaderMonoBehaviour))]
    public class SerialPortReaderMonoBehaviourEditor : UnityEditor.Editor
    {
        private SerializedProperty _useHighestComPort;
        private SerializedProperty _serialPortName;
        private SerializedProperty _baudRate;
        private SerializedProperty _dataReadDelay;
        private SerializedProperty _onSerialPortOpened;
        private SerializedProperty _onSerialPortDataReceived;
        private SerializedProperty _onSerialPortClosed;

        private void OnEnable()
        {
            _useHighestComPort = serializedObject.FindProperty("_useHighestComPort");
            _serialPortName = serializedObject.FindProperty("_serialPortName");
            _baudRate = serializedObject.FindProperty("_baudRate");
            _dataReadDelay = serializedObject.FindProperty("_dataReadDelay");
            _onSerialPortOpened = serializedObject.FindProperty("_onSerialPortOpened");
            _onSerialPortDataReceived = serializedObject.FindProperty("_onSerialPortDataReceived");
            _onSerialPortClosed = serializedObject.FindProperty("_onSerialPortClosed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(_useHighestComPort);
            if (_useHighestComPort.boolValue == false)
            {
                EditorGUILayout.PropertyField(_serialPortName);
            }

            EditorGUILayout.PropertyField(_baudRate);
            EditorGUILayout.PropertyField(_dataReadDelay);
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onSerialPortOpened);
            EditorGUILayout.PropertyField(_onSerialPortDataReceived);
            EditorGUILayout.PropertyField(_onSerialPortClosed);

            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}