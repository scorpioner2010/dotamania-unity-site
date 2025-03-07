using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Scenes
{
    public class FileSelector : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void OpenFileSelectionJS();

        public Button loadFileButton;

        private void Awake()
        {
            loadFileButton.onClick.AddListener(OpenFileSelection);
        }

        public void OpenFileSelection()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Викликаємо функцію з .jslib
            OpenFileSelectionJS();
#else
            Debug.Log("Функція OpenFileSelectionJS викликана в Editor. У WebGL працює правильно.");
#endif
        }
    }
}