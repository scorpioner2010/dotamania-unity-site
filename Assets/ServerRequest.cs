using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ServerRequest : MonoBehaviour
{
    private string serverUrl = "https://dotamania.bsite.net/api/hello";

    void Start()
    {
        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Відповідь сервера: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Помилка запиту: " + request.error);
            }
        }
    }
}
