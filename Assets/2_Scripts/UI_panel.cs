using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI_panel : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup group;
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }
    // Start is called before the first frame update
    void Start()
    {
       // Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close(bool instant = false){
        if(instant)
        {
            group.alpha = 0f;
            gameObject.SetActive(false);
        }
        else
        {
            LeanTween.alphaCanvas(group,0f,0f);
            Invoke(nameof(ClosePanel),0f);
        }
        
        group.interactable = group.blocksRaycasts = false;
    }
    public void ClosePanel() 
    {
        gameObject.SetActive(false);
    }
    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
    public void Open(bool instant = false){
        if(instant)
        {
            group.alpha = 1f;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
            LeanTween.alphaCanvas(group,1f,0);
        }

        group.interactable = group.blocksRaycasts = true;
    }
}
