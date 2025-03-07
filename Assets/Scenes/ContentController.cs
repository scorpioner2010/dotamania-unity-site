using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    public class ContentController : MonoBehaviour
    {
        public Button create;
        public Button clearAll;
        
        public Element prefabElement;
        public ScrollRect scrollRect;
        public Transform content;

        public List<Element> elements = new();
        public FileReceiver receiver;

        private void Start()
        {
            create.onClick.AddListener(Create);
            clearAll.onClick.AddListener(ClearAll);
        }

        private void Create()
        {
            Element o = Instantiate(prefabElement, content, true);
            elements.Add(o);
            o.transform.localScale = Vector3.one;
            o.image.sprite = receiver.image.sprite;
            o.description.text = receiver.text.text;
        }

        public void ClearAll()
        {
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
            }
            
            elements.Clear();
        }
    }
}
