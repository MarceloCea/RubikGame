using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteMenu : MonoBehaviour
{
   public CanvasGroup CanvasCompleteMenu;

    public void ShowCompleteMenu()
    {
        CanvasCompleteMenu.alpha = 1;
        CanvasCompleteMenu.blocksRaycasts = true;
        CanvasCompleteMenu.interactable = true;
    }

    public void CloseCompleteMenu()
    {
        CanvasCompleteMenu.alpha = 0;
        CanvasCompleteMenu.blocksRaycasts = false;
        CanvasCompleteMenu.interactable = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("TitleMenu");
    }
}
