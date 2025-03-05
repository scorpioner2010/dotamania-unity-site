using TMPro;
using UnityEngine;

namespace Scenes
{
    public class FileReceiver : MonoBehaviour
    {
        public TMP_Text text;

        // Метод викликається з JavaScript (через SendMessage)
        public void ReceiveFileContent(string fileContent)
        {
            Debug.Log("Вміст файлу: " + fileContent);

            // Відобразимо вміст файлу в TMP_Text
            text.text = fileContent;
        }
    }
}