using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuState : MonoBehaviour
{
    GameState.CubeProperties loadPropierties;
    public Text DataInfo;
    private Button LoadGameButton;
    public CanvasGroup Title;
    public CanvasGroup Cubeselect;

    // Start is called before the first frame update
    void Start()
    {
        LoadDataInfo();
    }

   public void LoadGame()
    {
        PlayerPrefs.SetInt("Load", 1);
        SceneManager.LoadScene(loadPropierties.SceneName);
    }

    void LoadDataInfo()
    {
        if (File.Exists(Application.dataPath + "saveCube.txt"))
        {

            loadPropierties = JsonUtility.FromJson<GameState.CubeProperties>(File.ReadAllText(Application.dataPath + "saveCube.txt"));

            float time = loadPropierties.StateOfTimer;
            int minutes = 0;
            int seconds = 0;
            int milliseconds = 0;
            if (time < 0)
            {
                time = 0;
            }
            minutes = (int)time / 60;
            seconds = (int)time % 60;
            milliseconds = (int)((time * 1000) % 1000);
           string Timer = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            DataInfo.text = loadPropierties.SceneName + " Time: " + Timer;

            DataInfo.rectTransform.parent.GetComponent<Button>().interactable = true;
        }
        else
        {
            DataInfo.rectTransform.parent.GetComponent<Button>().interactable = false;
        }
    }

    public void NewGameCube(int cube)
    {
        if (cube==0)
        {
            SceneManager.LoadScene("Cube2x2");
        }
        else if (cube == 1)
        {
            SceneManager.LoadScene("Cube3x3");
        }
        else if (cube == 2)
        {
            SceneManager.LoadScene("Cube4x4");
        }
        else if (cube == 3)
        {
            SceneManager.LoadScene("Cube5x5");
        }
        else if (cube == 4)
        {
            SceneManager.LoadScene("Cube6x6");
        }
    }

    public void Back()
    {
        Title.alpha = 1;
        Title.interactable = true;
        Title.blocksRaycasts = true;

        Cubeselect.alpha = 0;
        Cubeselect.interactable = false;
        Cubeselect.blocksRaycasts = false;
    }

    public void NewGame()
    {
        Title.alpha = 0;
        Title.interactable = false;
        Title.blocksRaycasts = false;

        Cubeselect.alpha = 1;
        Cubeselect.interactable = true;
        Cubeselect.blocksRaycasts = true;
    }
}
