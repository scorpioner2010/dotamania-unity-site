using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;

namespace Scenes
{
    public class FileReceiver : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void OpenFileSelectionJS();

        [FormerlySerializedAs("name")] public TMP_InputField nameContainer;
        [FormerlySerializedAs("text")] public TMP_InputField descriptionContainer;
        [FormerlySerializedAs("image")] public Image loadedImage;

        public void OpenFileSelection()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Викликаємо функцію з .jslib
            OpenFileSelectionJS();
#else
            Debug.Log("Функція OpenFileSelectionJS викликана в Editor. У WebGL працює правильно.");
#endif
        }

        // Метод викликається з JavaScript (через SendMessage)
        public void ReceiveFileContent(string data)
        {
            Debug.Log("Отримано дані: " + data);

            string[] splitData = data.Split('|');
            if (splitData.Length < 2)
                return;

            string fileType = splitData[0];
            string fileContent = splitData[1];

            if (fileType == "Text")
                DisplayText(fileContent);
            else if (fileType == "Image")
                DisplayImage(fileContent);
        }

        private void DisplayText(string fileContent)
        {
            if (descriptionContainer != null)
                descriptionContainer.text = fileContent;
        }

        private void DisplayImage(string base64Data)
        {
            byte[] imageData = Convert.FromBase64String(base64Data);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            Sprite newSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            if (loadedImage != null)
                loadedImage.sprite = newSprite;
        }
    }
}