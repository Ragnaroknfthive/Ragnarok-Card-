using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageSwitchController : MonoBehaviour
{
    public List<GameObject> imagePanels;
    public List<Button> ellipsisButtons;
    public List<GameObject> clickedStates;

    private int currentIndex;
    private GameObject currentClickedState;

    private void Start()
    {
        currentIndex = 0;
        currentClickedState = null;

        for (int i = 0; i < ellipsisButtons.Count; i++)
        {
            int index = i;
            ellipsisButtons[i].onClick.AddListener(() => SwitchImage(index));

            var trigger = ellipsisButtons[i].gameObject.AddComponent<EventTrigger>();
            AddEventTrigger(trigger, EventTriggerType.PointerClick, (eventData) => OnPointerClick(index));
        }
    }

    public void SwitchImage(int index)
    {
        if (index == currentIndex || index >= imagePanels.Count)
        {
            return;
        }

        imagePanels[currentIndex].SetActive(false);
        imagePanels[index].SetActive(true);
        currentIndex = index;
    }

    private void OnPointerClick(int index)
    {
        if (currentClickedState != null)
        {
            currentClickedState.SetActive(false);
        }

        ShowUnionGameObject(currentIndex, clickedStates[currentIndex], false);
        ShowUnionGameObject(index, clickedStates[index], true);

        currentIndex = index;
        currentClickedState = clickedStates[index];
    }

    private void ShowUnionGameObject(int index, GameObject state, bool show)
    {
        state.SetActive(show);
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}
