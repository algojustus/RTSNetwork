using UnityEditor;
using UnityEngine;

namespace RTSNetwork.Editor.GUI
{
    public class UnityUserInterface : UnityEditor.Editor
    {
        [MenuItem("Network/Connect with IP", false, 1)]
        private static void OpenNetworkGUI()
        {
            var menuPopup = CreateInstance<NetworkGUI>();
            menuPopup.LoadSettings();
            menuPopup.OpenWindow();
            Debug.Log("Opening Networking GUI now...");
        }

        [MenuItem("Network/Create Network Objects", false, 2)]
        private static void CreateGameObject()
        {
            GameObject connector = new GameObject("RTSConnector");
            connector.AddComponent<ClientMessages>(); //Baukasten zum hinzufügen von Scripts später
            Debug.Log("Creating connection GameObject...");
        }
    }
}