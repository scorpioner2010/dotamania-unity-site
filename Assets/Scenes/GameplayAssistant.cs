using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/*using Cysharp.Threading.Tasks;
using RedDeck.Cards;
using RedDeck.DataManager;
using RedDeck.Netwroking;
using RedDeck.Plugins;
using RedDeck.Tavern;
using ServerTCP.Gaming.Enums;
using ServerTCP.Gaming.Infos;*/
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RedDeck.Utilities
{
    public static class GameplayAssistant
    {
        private static readonly System.Random _random = new();
        private static readonly Dictionary<string, Texture2D> CashedTextures = new ();
        
        /*public static void UpdateStats(CardSlot slot)
        {
            CardInfo model = slot.GetCardInfo();
            ITavernController controller = ServiceLocator.Get<ITavernController>();
            CardBase cardBase = controller.GetCardByModelUid(model.CardUid);
                
            if (cardBase != null)
            {
                var view = cardBase as MiniCardView;
                    
                if (view != null)
                {
                    //view.DeactivateMechanics();
                }
                    
                cardBase.UpdateParameters(model);
                    
                if (view != null)
                {
                    view.RefreshMechanics(model);
                }
            }
            else
            {
                Debug.LogError("null");
            }
        }*/
        
        public static int GetCorrectIndex(int index, int massLength)
        {
            if (index < 0)
            {
                index = 0;
            } 
            else if (index > massLength -1)
            {
                index = massLength - 1;
            }

            return index;
        }
        public static bool GetTexture(string url, out Texture2D texture2D)
        {
            if (!CashedTextures.TryGetValue(url, out Texture2D texture))
            {
                texture2D = null;
                return false;
            }
            else
            {
                texture2D = texture;
                return true;
            }
        }

        public static void AddTexture(string url, Texture2D texture)
        {
            if (!CashedTextures.ContainsKey(url))
            {
                CashedTextures.Add(url, texture);
            }
        }
        public static IEnumerator DownloadTexture(string url, Image image, Action onSuccess)
        {
            if (!CashedTextures.TryGetValue(url, out Texture2D texture))
            {
                CashedTextures.Add(url, null);
                
                using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
#if !UNITY_EDITOR
                uwr.SetRequestHeader("Origin", "https://formula.funtico.com");
#endif
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    //Debug.LogError(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    texture = DownloadHandlerTexture.GetContent(uwr);
                    CashedTextures[url] = texture;
                }
            }
            else if (texture == null) // is loading one, wait him 10sec
            {
                float step = Time.fixedDeltaTime;
                float timer = 10000;
                while (timer > 0 && texture == null)
                {
                    yield return new WaitForFixedUpdate();
                    timer -= step;
                }
            }
            
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite.Create(texture, rec,new Vector2(0,0),1);
            image.sprite = Sprite.Create(texture, rec,new Vector2(0,0),.01f);
            onSuccess?.Invoke();
        }

        /*public static async UniTask<Sprite> GetDownloadTexture(string url, Action func = null)
        {
            if (!CashedTextures.TryGetValue(url, out Texture2D texture))
            {
                CashedTextures.Add(url, null);
                
                using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
                //uwr.SetRequestHeader("Origin", "https://formula.funtico.com"); 

                await uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    //Debug.LogError(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    texture = DownloadHandlerTexture.GetContent(uwr);
                    CashedTextures[url] = texture;
                }
            }
            else if (texture == null) // is loading one, wait him 10sec
            {
                float step = Time.fixedDeltaTime;
                float timer = 10000;
                while (timer > 0 && texture == null)
                {
                    await UniTask.Yield();
                    timer -= step;
                }
            }
            
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite.Create(texture, rec, new Vector2(0, 0), 1);
            Sprite sprite = Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
            func?.Invoke();
            return sprite;
        }*/

        public static Vector3 Random(this Vector3 obj, float value, bool isAbsolute = false)
        {
            float from = isAbsolute ? 0 : -value;
            float to = value;
            
            return new Vector3(
                UnityEngine.Random.Range(from, to),
                UnityEngine.Random.Range(from, to),
                UnityEngine.Random.Range(from, to));
        }

        public static Vector2 ToVector2(this Vector3 obj) => new (obj.x, obj.y);

        public static float ParseToFloat(string data)
        {
            if (data.Contains('.'))
            {
                string[] d1 = data.Split('.');
                float value = float.Parse(d1[0]) + float.Parse(d1[1]) / Mathf.Pow(10, d1[1].Length);
                return value;
            }

            return float.Parse(data);
        }
        
        public static string RemoveLastChars(string text, int amount)
        {
            for (var index = 0; index < amount; index++)
            {
                text = text.Remove(text.Length - 1);
            }

            return text;
        }

        public static Quaternion Random(this Quaternion obj, float value, bool isAbsolute = false)
        {
            float from = isAbsolute ? 0 : -value;
            float to = value;
            
            return new Quaternion(
                UnityEngine.Random.Range(from, to), 
                UnityEngine.Random.Range(from, to),
                UnityEngine.Random.Range(from, to),
                UnityEngine.Random.Range(from, to));
        }

        public static string AddSpaceBeforeCapitalLetters(string input)
        {
            string result = "";
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    result += " " + c;
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }

        public static T RandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static int RemoveAllNull<T>(this List<T> list)
        {
            return list.RemoveAll(item => item.Equals(default(T)));
        }
        
        public static int RandomIndex<T>(this T[] array)
        {
            return UnityEngine.Random.Range(0, array.Length);
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[array.RandomIndex()];
        }
        
        public static string GetRandomElement(this string[] list)
        {
            int index = UnityEngine.Random.Range(0, list.Length);
            return list[index];
        }

        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            if (layerMask == (layerMask | (1 << layer)))
            {
                return true;
            }

            return false;
        }
        
        public static bool IsParentNotActive(Transform parent)
        {
            if (parent != null)
            {
                if (parent.gameObject.activeSelf)
                {
                    return IsParentNotActive(parent.parent);
                }
                
                return false;
            }
            
            return true;
        }
        
        public static float Remap(float value, float startMin, float startMax, float resultMin, float resultMax, bool clamp = false)
        {
            if (clamp)
            {
                value = startMin > startMax ? Mathf.Clamp(value, startMax, startMin) : Mathf.Clamp(value, startMin, startMax);
            }
            
            return (value - startMin) / (startMax - startMin) * (resultMax - resultMin) + resultMin;
        }
        
        public static Vector3 LerpAxis(Vector3 euler, float tgtVal, float delta, bool isDrift = false)
        {
            Vector3 val = isDrift ? Vector3.zero : euler;
            float value = Mathf.LerpAngle(euler.y, tgtVal, delta);
            return new Vector3(val.x, value, val.z);
        }

#if UNITY_EDITOR
        public static void CreateVectorDirection(Transform obj, float lengthLine = 2.5f, float widthLine = 10f)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(obj.position, obj.position + obj.forward * lengthLine, widthLine);
            
            Handles.color = Color.red;
            Handles.DrawLine(obj.position, obj.position + obj.right * lengthLine, widthLine);
            
            Handles.color = Color.green;
            Handles.DrawLine(obj.position, obj.position + obj.up * lengthLine, widthLine);
        }
#endif
        

        public static string GenerateName(int len)
        {
            System.Random r = new System.Random();
            
            string[] consonants =
            {
                "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v",
                "w", "x"
            };
            
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string name = "";
            name += consonants[r.Next(consonants.Length)].ToUpper();
            name += vowels[r.Next(vowels.Length)];
            int b = 2;
            
            while (b < len)
            {
                name += consonants[r.Next(consonants.Length)];
                b++;
                name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return name;
        }
        
        public static string GetRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        
        #if UNITY_EDITOR
        public static void DrawArrow(Color color, Vector3 a, Vector3 b, float arrowheadAngle = 20, float arrowheadDistance = 1, float arrowheadLength = 1, float widthLine = 1.5f)
        {
            Vector3 dir = b - a;
            Vector3 arrowPos = a + (dir * arrowheadDistance);

            Vector3 up = Quaternion.LookRotation(dir) * new Vector3(0f, Mathf.Sin(arrowheadAngle / 72), -1f) * arrowheadLength;
            Vector3 down = Quaternion.LookRotation(dir) * new Vector3(0f, -Mathf.Sin(arrowheadAngle / 72), -1f) * arrowheadLength;
            Vector3 left = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin(arrowheadAngle / 72), 0f, -1f) * arrowheadLength;
            Vector3 right = Quaternion.LookRotation(dir) * new Vector3(-Mathf.Sin(arrowheadAngle / 72), 0f, -1f) * arrowheadLength;

            Vector3 upPos = arrowPos + up;
            Vector3 downPos = arrowPos + down;
            Vector3 leftPos = arrowPos + left;
            Vector3 rightPos = arrowPos + right;

            Handles.color = color;
            Handles.DrawLine(a, b, widthLine);
            Handles.DrawLine(arrowPos, upPos, widthLine);
            Handles.DrawLine(arrowPos, leftPos, widthLine);
            Handles.DrawLine(arrowPos, downPos, widthLine);
            Handles.DrawLine(arrowPos, rightPos, widthLine);

            Handles.DrawLine(upPos, leftPos, widthLine);
            Handles.DrawLine(leftPos, downPos, widthLine);
            Handles.DrawLine(downPos, rightPos, widthLine);
            Handles.DrawLine(rightPos, upPos, widthLine);
        }
        #endif

        public static List<T> GetObjectsByDistance<T>(List<T> objects, Vector3 pos, float distance , MonoBehaviour exception = null)
        {
            List<T> all = new List<T>();
            
            foreach (T obj in objects)
            {
                MonoBehaviour behaviour = obj as MonoBehaviour;
                float dis = Vector3.Distance(behaviour.transform.position, pos);
                
                if (exception != null)
                {
                    if (dis < distance && behaviour != exception)
                    {
                        all.Add(obj);
                    }
                }
                else
                {
                    if (dis < distance)
                    {
                        all.Add(obj);
                    }
                }
            }

            return all;
        }

        public static T GetNearObject<T>(List<T> objects, Vector3 pos, MonoBehaviour exception)
        {
            T nearObject = default;
            float dist = float.MaxValue;

            foreach (T obj in objects)
            {
                MonoBehaviour behaviour = obj as MonoBehaviour;
                float dis = Vector3.Distance(behaviour.transform.position, pos);

                if (dis < dist && behaviour != exception)
                {
                    dist = dis;
                    nearObject = obj;
                }
            }

            return nearObject;
        }

        public static Vector3 ClosestPoint(Vector3 origin, Vector3 end, Vector3 point) 
        { 
            Vector3 heading = (end - origin); 
            float magnitudeMax = heading.magnitude; 
            heading = heading.normalized; 
            Vector3 lhs = point - origin; 
            float dotP = Vector3.Dot(lhs, heading); 
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax); 
            return origin + heading * dotP; 
        }

        public static bool PresenceObjectOnScreen(Camera camera, Vector3 obj)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(obj);
            return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
        }

        public static void CreateSphereDots(Vector3 pos, float size, int amountDots = 1000, float lifeTime = 6, float sizeDot = 0.5f)
        {
            for (int i = 0; i < amountDots; i++)
            {
                Vector3 c = pos + UnityEngine.Random.onUnitSphere * size;
                CreateSphere(Color.blue, sizeDot, c, lifeTime);
            }
        }

        public static void CreateSphere(Color color, float size, Vector3 pos, float lifeTime = 6)
        {
            GameObject f = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            f.transform.position = pos;
            f.transform.localScale = Vector3.one * size;
            f.GetComponent<MeshRenderer>().material.color = color;
            GameObject.Destroy(f.GetComponent<Collider>());
            GameObject.Destroy(f, lifeTime);
        }
        
        public static void DrawLine(Vector3 from, Vector3 to, Color color, float lifeTime = 6)
        {
            Vector3 dir2 = (to - from).normalized;
            float dis = Vector3.Distance(to, from);
            GameObject gm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gm.GetComponent<MeshRenderer>().material.color = color;
            gm.GetComponent<Collider>().enabled = false;
            gm.transform.position = from;
            gm.transform.position += dir2 * (dis / 2);
            gm.transform.LookAt(from);
            gm.transform.localScale = new Vector3(.2f, .2f, dis);
            GameObject.Destroy(gm, lifeTime);
        }

        public static void DrawCube(Vector3 pos, Color color, float scale, float lifeTime = 6)
        {
            GameObject gm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gm.GetComponent<MeshRenderer>().material.color = color;
            gm.transform.position = pos;
            gm.GetComponent<Collider>().enabled = false;
            gm.transform.localScale = Vector3.one * scale;
            GameObject.Destroy(gm, lifeTime);
        }

        private static readonly Vector4[] s_UnitSphere = MakeUnitSphere(16);
        public static void DebugDrawSphere(Vector4 pos, float radius, Color color, float duration)
        {
            Vector4[] v = s_UnitSphere;
            int len = s_UnitSphere.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var sX = pos + radius * v[0 * len + i];
                var eX = pos + radius * v[0 * len + (i + 1) % len];
                var sY = pos + radius * v[1 * len + i];
                var eY = pos + radius * v[1 * len + (i + 1) % len];
                var sZ = pos + radius * v[2 * len + i];
                var eZ = pos + radius * v[2 * len + (i + 1) % len];
                Debug.DrawLine(sX, eX, color, duration);
                Debug.DrawLine(sY, eY, color, duration);
                Debug.DrawLine(sZ, eZ, color, duration);
            }
        }

        private static Vector4[] MakeUnitSphere(int len)
        {
            Debug.Assert(len > 2);
            var v = new Vector4[len * 3];
            for (int i = 0; i < len; i++)
            {
                var f = i / (float)len;
                float c = Mathf.Cos(f * (float)(Math.PI * 2.0));
                float s = Mathf.Sin(f * (float)(Math.PI * 2.0));
                v[0 * len + i] = new Vector4(c, s, 0, 1);
                v[1 * len + i] = new Vector4(0, c, s, 1);
                v[2 * len + i] = new Vector4(s, 0, c, 1);
            }
            return v;
        }
    }
}