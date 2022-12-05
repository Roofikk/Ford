using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteSave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => Delete());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    void Delete()
    {
        var item = ItemLoad.prev ?? null;
        if (item != null)
        {
            Destroy(item.gameObject);
            File.Delete(item.path);
        }
    }
}
