using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiHandlerMenu : MonoBehaviour, IPointerClickHandler
{
    public ItemLoad itemLoad;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<ItemLoad>(out itemLoad))
        {
            if (ItemLoad.prev != null)
            {
                ItemLoad.prev.isSelected = false;
                Unselect(ItemLoad.prev.transform);
            }

            itemLoad.isSelected = true;
            ItemLoad.prev = itemLoad;
            Select(itemLoad.transform);
        }
    }

    private void Start()
    {

    }

    void Select(Transform item)
    {
        Image image = item.GetComponent<Image>();
        image.color = new Color { a = 1f, b = 0.5f, g = 192f / 255f, r = 1f };
    }

    void Unselect(Transform item)
    {
        Image image = item.GetComponent<Image>();
        image.color = new Color { a = 1f, b = 1f, g = 1f, r = 1f };
    }
}
