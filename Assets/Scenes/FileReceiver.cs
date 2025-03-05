using UnityEngine;

namespace Scenes
{
    public class FileReceiver : MonoBehaviour
    {
        // Цей метод викликається з JavaScript
        public void ReceiveFileContent(string fileContent)
        {
            Debug.Log("Вміст файлу: " + fileContent);
            // Обробка вмісту файлу за потребою
        }
    }
}