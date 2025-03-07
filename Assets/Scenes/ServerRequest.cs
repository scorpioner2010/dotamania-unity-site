using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Scenes;
using TMPro;
using UnityEngine.UI;

public class ServerRequest : MonoBehaviour
{
    //public string serverUrl = "https://dotamania.bsite.net/api"; // URL твого сервера
    public string serverUrl = "http://localhost:51754/api";

    // Кнопка для завантаження контейнера
    public Button send;
    // Кнопка для видалення контейнера
    public Button delete;
    public TMP_Text result;
    
    public Image imgContainer;
    public TMP_InputField nameContainer;
    public TMP_InputField descriptionContainer;

    public ContentController contentController;
    
    private void Start()
    {
        send.onClick.AddListener(() =>
        {
            // Викликаємо метод завантаження контейнера
            StartCoroutine(UploadContainer(imgContainer.sprite));
        });
        
        delete.onClick.AddListener(() =>
        {
            // Викликаємо метод видалення контейнера за назвою з поля nameContainer
            StartCoroutine(DeleteContainer(nameContainer.text));
        });
    }
    
    private IEnumerator UploadContainer(Sprite sprite)
    {
        Texture2D texture;
    
        // 1. Перевірка, чи спрайт порожній
        if (sprite == null)
        {
            // 2. Створення 64x64 текстури білого кольору
            texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        
            // Масив кольорів
            Color32[] pixels = new Color32[64 * 64];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(255, 255, 255, 255); // білий
            }
        
            texture.SetPixels32(pixels);
            texture.Apply();
        }
        else
        {
            // Якщо спрайт не null, беремо його текстуру
            texture = sprite.texture;
        }
    
        // 3. Перетворення текстури в PNG (масив байтів)
        byte[] imageData = texture.EncodeToPNG();
    
        // 4. Формування форми для завантаження
        WWWForm form = new WWWForm();
        form.AddField("containerName", nameContainer.text);
        form.AddField("description", descriptionContainer.text);
        form.AddBinaryData("file", imageData, "sprite.png", "image/png");
    
        // 5. Відправка запиту до серверного endpoint
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/uploadContainer", form))
        {
            yield return www.SendWebRequest();
        
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Container uploaded successfully!");
                result.text = "Container uploaded successfully!";
            }
            else
            {
                Debug.LogError("Upload error: " + www.error);
                result.text = "Upload error: " + www.error;
            }
        }
    }
    
    private IEnumerator DeleteContainer(string containerName)
    {
        if (string.IsNullOrEmpty(containerName))
        {
            result.text = "Container name is required for deletion.";
            yield break;
        }
        
        // Формуємо URL для DELETE запиту
        string url = serverUrl + "/deleteContainer/" + containerName;
        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Container deleted successfully!");
            result.text = "Container deleted successfully!";
        }
        else
        {
            Debug.LogError("Delete error: " + request.error);
            result.text = "Delete error: " + request.error;
        }
    }
    
    [Serializable]
    public class MessageRequest
    {
        public string message;
    }
}