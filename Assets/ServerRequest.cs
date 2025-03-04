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
    
    void Start()
    {
        send.onClick.AddListener(() =>
        {
            StartCoroutine(SendMessageToServer(sentText.text));
        });
        
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
            Debug.Log("Відповідь сервера: " + request.downloadHandler.text);
            result.text = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Помилка запиту: " + request.error);
            result.text = "error!";
        }
    }
}