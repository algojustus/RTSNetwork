using UnityEditor;
using UnityEngine;

namespace RTSNetwork.Editor.GUI
{
    public class NetworkGUI : EditorWindow
    {
        private string ipAddress;
        private string serverPort;
        private bool groupEnabled = true;
        private bool checkBox = false;
        private float sliderStart = 1f;

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            ipAddress = EditorGUILayout.TextField("Server IP:", ipAddress);
            serverPort = EditorGUILayout.TextField("Server Port:", serverPort);

            //Playground zum testen
            GUILayout.Space(10);
            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            checkBox = EditorGUILayout.Toggle("Toggle", checkBox);
            sliderStart = EditorGUILayout.Slider("Slider", sliderStart, -3, 3);
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("Save"))
            {
                IfSaved();
            }
        }

        private void IfSaved()
        {
            PlayerPrefs.SetString("ServerIP", ipAddress);
            //PlayerPrefs.Save();
            PlayerPrefs.SetString("ServerPort", serverPort);
            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            ipAddress = PlayerPrefs.GetString("ServerIP");
            serverPort = PlayerPrefs.GetString("ServerPort");
        }

        public void OpenWindow()
        {
            NetworkGUI window = (NetworkGUI) GetWindow(typeof(NetworkGUI));
            window.Show();
        }
    }
}
