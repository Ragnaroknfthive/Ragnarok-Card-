using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject unionGameObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowUnionGameObject(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowUnionGameObject(false);
    }

    private void ShowUnionGameObject(bool show)
    {
        unionGameObject.SetActive(show);
    }
}
