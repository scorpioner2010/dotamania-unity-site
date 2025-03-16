using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Scenes
{
    public class ServerRequest : MonoBehaviour
    {
        // Вкажіть віддалену адресу сервера!
        private string serverUrl = "https://dotamania.bsite.net/api";
        // Приклад локального сервера (залиште закоментованим)
        // public string serverUrl = "http://localhost:51754/api";
        
        public Button send;       // Кнопка для завантаження контейнера
        public Button delete;     // Кнопка для видалення контейнера
        public TMP_Text result;
    
        public Image imgContainer;
        public TMP_InputField nameContainer;
        public TMP_InputField descriptionContainer;
    
        public ContentController contentController;
    
        private void Start()
        {
            // Отримання списку контейнерів після запуску
            StartCoroutine(GetContainersFromServer());
    
            send.onClick.AddListener(() =>
            {
                StartCoroutine(UploadContainer(imgContainer.sprite));
            });
        
            delete.onClick.AddListener(() =>
            {
                StartCoroutine(DeleteContainer(nameContainer.text));
            });
        }
    
        private IEnumerator UploadContainer(Sprite sprite)
        {
            Debug.Log("<color=yellow>=== UploadContainer START ===</color>");
    
            Texture2D texture;
    
            if (sprite == null)
            {
                Debug.Log("Sprite is null. Creating white 64x64 texture...");
                texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                Color32[] pixels = new Color32[64 * 64];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color32(255, 255, 255, 255);
                }
                texture.SetPixels32(pixels);
                texture.Apply();
            }
            else
            {
                Debug.Log("Sprite is valid. Using sprite texture...");
                texture = sprite.texture;
            }
    
            byte[] imageData = texture.EncodeToPNG();
            Debug.Log($"Encoded image size: {imageData.Length} bytes.");
    
            WWWForm form = new WWWForm();
            // Поля мають співпадати з ContainerCreateDto на сервері
            form.AddField("Name", nameContainer.text);
            form.AddField("Description", descriptionContainer.text);
            form.AddBinaryData("Image", imageData, "sprite.png", "image/png");
    
            string postUrl = serverUrl + "/containers";
            Debug.Log($"<color=yellow>Sending POST to: {postUrl}</color>");
    
            using (UnityWebRequest www = UnityWebRequest.Post(postUrl, form))
            {
                yield return www.SendWebRequest();
    
                Debug.Log($"HTTP response code: {www.responseCode}");
                Debug.Log($"IsNetworkError: {www.isNetworkError}");
                Debug.Log($"IsHttpError: {www.isHttpError}");
                Debug.Log($"Error: {www.error}");
                Debug.Log($"Downloaded text: {www.downloadHandler.text}");
    
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("<color=green>Container uploaded successfully!</color>");
                    result.text = "Container uploaded successfully!";
                    StartCoroutine(GetContainersFromServer());
                }
                else
                {
                    Debug.LogError("Upload error: " + www.error);
                    result.text = "Upload error: " + www.error;
                }
            }
    
            Debug.Log("<color=yellow>=== UploadContainer END ===</color>");
        }
    
        private IEnumerator DeleteContainer(string containerName)
        {
            Debug.Log("<color=yellow>=== DeleteContainer START ===</color>");
    
            if (string.IsNullOrEmpty(containerName))
            {
                Debug.LogWarning("Container name is required for deletion.");
                result.text = "Container name is required for deletion.";
                yield break;
            }
    
            string url = serverUrl + "/containers/" + containerName;
            Debug.Log($"<color=yellow>Sending DELETE to: {url}</color>");
            UnityWebRequest request = UnityWebRequest.Delete(url);
            yield return request.SendWebRequest();
    
            Debug.Log($"HTTP response code: {request.responseCode}");
            Debug.Log($"IsNetworkError: {request.isNetworkError}");
            Debug.Log($"IsHttpError: {request.isHttpError}");
            Debug.Log($"Error: {request.error}");
            Debug.Log($"Downloaded text: {request.downloadHandler.text}");
    
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("<color=green>Container deleted successfully!</color>");
                result.text = "Container deleted successfully!";
                StartCoroutine(GetContainersFromServer());
            }
            else
            {
                Debug.LogError("Delete error: " + request.error);
                result.text = "Delete error: " + request.error;
            }
    
            Debug.Log("<color=yellow>=== DeleteContainer END ===</color>");
        }
    
        private IEnumerator GetContainersFromServer()
        {
            Debug.Log("<color=yellow>=== GetContainersFromServer START ===</color>");
    
            string getUrl = serverUrl + "/containers";
            Debug.Log($"<color=yellow>Sending GET to: {getUrl}</color>");
    
            using (UnityWebRequest www = UnityWebRequest.Get(getUrl))
            {
                yield return www.SendWebRequest();
    
                Debug.Log($"HTTP response code: {www.responseCode}");
                Debug.Log($"IsNetworkError: {www.isNetworkError}");
                Debug.Log($"IsHttpError: {www.isHttpError}");
                Debug.Log($"Error: {www.error}");
                Debug.Log($"Downloaded text: {www.downloadHandler.text}");
    
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;
                    Debug.Log($"JSON from server: {json}");
    
                    ContainerInfo[] containers = JsonHelper.FromJson<ContainerInfo>(json);
                    contentController.RemoveAll();
    
                    foreach (ContainerInfo c in containers)
                    {
                        Texture2D tex = new Texture2D(2, 2);
    
                        if (!string.IsNullOrEmpty(c.imageBase64))
                        {
                            byte[] bytes = Convert.FromBase64String(c.imageBase64);
                            tex.LoadImage(bytes);
                            Debug.Log($"Loaded image from base64 for container '{c.name}'.");
                        }
                        else
                        {
                            Debug.Log($"No imageBase64 for container '{c.name}'. Using white 64x64.");
                            tex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                            Color32[] pixels = new Color32[64 * 64];
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                pixels[i] = new Color32(255, 255, 255, 255);
                            }
                            tex.SetPixels32(pixels);
                            tex.Apply();
                        }
    
                        Sprite sprite = Sprite.Create(tex,
                            new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f));
    
                        contentController.CreateElement(c.name, c.description, sprite);
                    }
    
                    result.text = "Containers updated: " + containers.Length;
                }
                else
                {
                    Debug.LogError("GetContainers error: " + www.error);
                    result.text = "GetContainers error: " + www.error;
                }
            }
    
            Debug.Log("<color=yellow>=== GetContainersFromServer END ===</color>");
        }
    
        [Serializable]
        public class MessageRequest
        {
            public string message;
        }
    
        [Serializable]
        public class ContainerInfo
        {
            public string name;
            public string description;
            public string imageBase64;
        }
    
        public static class JsonHelper
        {
            public static T[] FromJson<T>(string json)
            {
                string newJson = "{ \"Items\": " + json + "}";
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
                return wrapper.Items;
            }
    
            [Serializable]
            private class Wrapper<T>
            {
                public T[] Items;
            }
        }
    }
}
