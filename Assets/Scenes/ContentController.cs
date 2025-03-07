using System.Collections.Generic;
using RedDeck.Utilities; // <-- Якщо у вас є цей неймспейс, залиште. Інакше приберіть.
using UnityEngine;

namespace Scenes
{
    public class ContentController : MonoBehaviour
    {
        public Element prefabElement;
        public Transform content;
        public List<Element> elements = new();

        public void Remove(string nameContainer)
        {
            elements.RemoveAllNull();
            
            foreach (Element element in elements)
            {
                if (element.nameContainer.text == nameContainer)
                {
                    Destroy(element.gameObject);
                }
            }
        }

        public void RemoveAll()
        {
            foreach (Element element in elements)
            {
                if (element != null)
                {
                    Destroy(element.gameObject);
                }
            }
            elements.Clear();
        }

        // Новий публічний метод, який викликає ServerRequest
        public void CreateElement(string nameContainer, string descriptionContainer, Sprite iconContainer)
        {
            Element o = Instantiate(prefabElement, content, true);
            elements.Add(o);
            o.transform.localScale = Vector3.one;
            o.imageContainer.sprite = iconContainer;
            o.nameContainer.text = nameContainer;
            o.descriptionContainer.text = descriptionContainer;
        }
    }
}