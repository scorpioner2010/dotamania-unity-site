using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using TMPro;
using UnityEngine.UI;

public class ServerRequest : MonoBehaviour
{
    //private string serverUrl = "https://dotamania.bsite.net/api"; // URL твого сервера
    private string serverUrl = "http://localhost:51754/api";

    public Button send;
    public TMP_InputField sentText;
    public TMP_Text result;

    public Image img;
    
    private void Start()
    {
        send.onClick.AddListener(() =>
        {
            //StartCoroutine(SendMessageToServer(sentText.text));
            StartCoroutine(UploadSprite(img.sprite));
        });
    }
    
    private IEnumerator UploadSprite(Sprite sprite)
    {
        // Отримання текстури з спрайту
        Texture2D texture = sprite.texture;
    
        // Перетворення текстури в PNG (масив байтів)
        byte[] imageData = texture.EncodeToPNG();
    
        // Створення форми для завантаження
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, "sprite.png", "image/png");
    
        // Відправка запиту до серверного endpoint
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl+"/upload", form))
        {
            yield return www.SendWebRequest();
        
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("Sprite uploaded successfully!");
            else
                Debug.LogError("Upload error: " + www.error);
        }
    }

    public IEnumerator SendMessageToServer(string message)
    {
        string jsonData = $"{{\"message\": \"{message}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            MessageRequest data = JsonUtility.FromJson<MessageRequest>(request.downloadHandler.text);
            result.text = data.message;
        }
        else
        {
            result.text = request.error;
        }
    }
    
    [Serializable]
    public class MessageRequest
    {
        public string message;
    }
}