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
        // Приклад локального сервера
        private string serverUrl = "http://localhost:51754/api";
        
        public Button send;       // Кнопка для завантаження контейнера
        public Button delete;     // Кнопка для видалення контейнера
        public TMP_Text result;
    
        public Image imgContainer;
        public TMP_InputField nameContainer;
        public TMP_InputField descriptionContainer;

        public ContentController contentController;

        private void Start()
        {
            // Після запуску отримуємо весь список із сервера
            StartCoroutine(GetContainersFromServer());

            send.onClick.AddListener(() =>
            {
                // Завантажуємо контейнер, а потім оновлюємо список
                StartCoroutine(UploadContainer(imgContainer.sprite));
            });
        
            delete.onClick.AddListener(() =>
            {
                // Видаляємо контейнер, а потім оновлюємо список
                StartCoroutine(DeleteContainer(nameContainer.text));
            });
        }
    
        private IEnumerator UploadContainer(Sprite sprite)
        {
            Texture2D texture;
    
            // 1. Якщо спрайт порожній – створюємо 64x64 білу текстуру
            if (sprite == null)
            {
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
                // Якщо спрайт не null, беремо його текстуру
                texture = sprite.texture;
            }

            // 2. Перетворення текстури в PNG (масив байтів)
            byte[] imageData = texture.EncodeToPNG();

            // 3. Формування форми для завантаження
            WWWForm form = new WWWForm();
            // Поля мають співпадати з ContainerCreateDto на сервері
            form.AddField("Name", nameContainer.text);
            form.AddField("Description", descriptionContainer.text);
            form.AddBinaryData("Image", imageData, "sprite.png", "image/png");

            // 4. Відправка запиту (POST /api/containers)
            using (UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/containers", form))
            {
                yield return www.SendWebRequest();
         
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Container uploaded successfully!");
                    result.text = "Container uploaded successfully!";
                 
                    // Оновлюємо список контейнерів
                    StartCoroutine(GetContainersFromServer());
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
         
            // Формуємо URL для DELETE (DELETE /api/containers/{name})
            string url = serverUrl + "/containers/" + containerName;
            UnityWebRequest request = UnityWebRequest.Delete(url);
            yield return request.SendWebRequest();
         
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Container deleted successfully!");
                result.text = "Container deleted successfully!";
             
                // Оновлюємо список контейнерів
                StartCoroutine(GetContainersFromServer());
            }
            else
            {
                Debug.LogError("Delete error: " + request.error);
                result.text = "Delete error: " + request.error;
            }
        }

        // ================== ОНОВЛЕННЯ СПИСКУ КОНТЕЙНЕРІВ ==================

        private IEnumerator GetContainersFromServer()
        {
            // GET /api/containers
            using (UnityWebRequest www = UnityWebRequest.Get(serverUrl + "/containers"))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Отримуємо JSON
                    string json = www.downloadHandler.text;
                    // Парсимо масив ContainerInfo[]
                    ContainerInfo[] containers = JsonHelper.FromJson<ContainerInfo>(json);

                    // Очищаємо локальний список
                    contentController.RemoveAll();

                    // Створюємо елементи для кожного контейнера
                    foreach (ContainerInfo c in containers)
                    {
                        // Створюємо текстуру
                        Texture2D tex = new Texture2D(2, 2);
                     
                        if (!string.IsNullOrEmpty(c.imageBase64))
                        {
                            byte[] bytes = Convert.FromBase64String(c.imageBase64);
                            tex.LoadImage(bytes);
                        }
                        else
                        {
                            // Якщо картинки немає – робимо білу 64x64
                            tex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                            Color32[] pixels = new Color32[64 * 64];
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                pixels[i] = new Color32(255, 255, 255, 255);
                            }
                            tex.SetPixels32(pixels);
                            tex.Apply();
                        }

                        // Створюємо спрайт
                        Sprite sprite = Sprite.Create(tex,
                            new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f));

                        // Додаємо новий елемент у список
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
        }

        // ====== КЛАСИ-ДОПОМОЖНИКИ ======

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

        // Unity не вміє напряму парсити масиви через JsonUtility,
        // тому використовуємо обгортку
        public static class JsonHelper
        {
            public static T[] FromJson<T>(string json)
            {
                // Обгортаємо масив у поле "Items"
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
