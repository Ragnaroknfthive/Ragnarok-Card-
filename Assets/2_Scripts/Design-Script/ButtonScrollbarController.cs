using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonScrollbarController : MonoBehaviour, IPointerClickHandler
{
    public Scrollbar scrollbar;
    public float targetValue;

    private bool isClicked;

    private static ButtonScrollbarController currentClickedButton;

    private void Update()
    {
        if (isClicked)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetValue, Time.deltaTime * 5f);
            if (Mathf.Abs(scrollbar.value - targetValue) < 0.01f)
            {
                scrollbar.value = targetValue;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentClickedButton != null && currentClickedButton != this)
        {
            currentClickedButton.isClicked = false;
        }

        isClicked = !isClicked;
        currentClickedButton = this;
    }
}
