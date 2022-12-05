using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaves : MonoBehaviour
{
    public Transform parentLoad;
    public Sprite sprite;
    public Font font;

    void Start()
    {
        Load();
    }

    public void Load()
    {
        string path = Application.dataPath + "/Saves/";
        string[] files = Directory.GetFiles(path);

        foreach (var e in files)
        {
            if (!e.Contains(".meta") && e.Contains(".xml"))
            {
                string[] split = e.Split('/');
                string s = split[split.Length - 1].Replace(".xml", "");

                GameObject item = new GameObject(s);
                item.transform.SetParent(parentLoad);
                ItemLoad itemLoad = item.AddComponent<ItemLoad>();
                itemLoad.name = s;
                itemLoad.path = e;

                Image image = item.AddComponent<Image>();
                image.sprite = sprite;
                image.type = Image.Type.Sliced;

                HorizontalLayoutGroup horizontalLayoutGroup = item.AddComponent<HorizontalLayoutGroup>();
                horizontalLayoutGroup.padding.left = 5;
                horizontalLayoutGroup.padding.top = 8;
                horizontalLayoutGroup.padding.right = 5;
                horizontalLayoutGroup.padding.bottom = 8;

                ContentSizeFitter contentSizeFitter = item.AddComponent<ContentSizeFitter>();
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                GameObject textObject = new GameObject("Text");
                textObject.transform.SetParent(item.transform);
                Text text = textObject.AddComponent<Text>();
                text.raycastTarget = false;
                text.font = font;
                text.color = Color.black;
                text.text = s;
            }
        }
    }
}
