using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    public class FileSelector : MonoBehaviour
    {
        public Button loadFile;

        private void Awake()
        {
            loadFile.onClick.AddListener(() =>
            {
                OpenFileSelection();
            });
        }

        // Викликає JavaScript функцію для відкриття вікна вибору файлу
        public void OpenFileSelection()
        {
            Application.ExternalCall("triggerFileSelection");
        }
    }
}