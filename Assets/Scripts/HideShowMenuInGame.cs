using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideShowMenuInGame : MonoBehaviour
{
    public Text ButtonText;
    public CanvasGroup canvasMenu;
    bool bOpen=true;
    void Start()
    {
        bOpen = true;
    }
    public void ShowHideMenu()
    {
        bOpen = !bOpen;
        if (bOpen)
        {
            canvasMenu.alpha = 1;
            canvasMenu.interactable = true;
            canvasMenu.blocksRaycasts = true;
            ButtonText.text = "<";
        }
        else
        {
            canvasMenu.alpha = 0;
            canvasMenu.interactable = false;
            canvasMenu.blocksRaycasts = false;
            ButtonText.text = ">";
        }
    }
}
