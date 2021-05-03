using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public CanvasGroup CanvasPauseMenu;
    int selectiontype = 0; //0=reset scene; 1=back to menu
    public GameObject pauseMessage;
    public GameObject ConfirmMessage;
    public GameState SaveGame;
    private GameState GS;

    private void Start()
    {
        GS = GameObject.FindObjectOfType<GameState>();
    }
    public void OpenMenu()
    {
        CanvasPauseMenu.alpha = 1;
        CanvasPauseMenu.interactable = true;
        CanvasPauseMenu.blocksRaycasts = true;
        pauseMessage.SetActive(true);
        ConfirmMessage.SetActive(false);
    }
    public void CloseMenu()
    {
        CanvasPauseMenu.alpha = 0;
        CanvasPauseMenu.interactable = false;
        CanvasPauseMenu.blocksRaycasts = false;
        pauseMessage.SetActive(true);
        ConfirmMessage.SetActive(false);
    }

    public void ButtonSelected(int selection)
    {
        selectiontype = selection;
        pauseMessage.SetActive(false);
        ConfirmMessage.SetActive(true);
    }

    public void CancelConfirmMessage()
    {
        pauseMessage.SetActive(true);
        ConfirmMessage.SetActive(false);
    }

    public void AcceptConfirmMessage()
    {
        if (selectiontype == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            if (!GS.bCompleted)
            {
                SaveGame.SaveGame();
            }
            SceneManager.LoadScene("TitleMenu");
        }
    }
}
