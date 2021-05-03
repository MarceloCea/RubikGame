using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public enum TypeOfCube { TwoxTwo, ThreexThree, FourxFour, FivexFive, SixxSix };
    public enum TypeOfRotate {UpToRight,UpToLeft,MiddleToRight,MiddleToLeft,DownToRight,DownToLeft,LeftToRight,LeftToLeft,MiddleVerFrontToRight,
                              MiddleVerFrontToLeft,RightToRight,RightToLeft, FrontToRight,FrontToLeft,MiddleSideToRight,MiddleSideToLeft,BackToLeft,BackToRight,
                              DownToRight2,DownToRight3, DownToRight4, DownToLeft2, DownToLeft3, DownToLeft4, RightToRight2, RightToRight3, RightToRight4, RightToLeft2, RightToLeft3,
                              RightToLeft4, BackToLeft2, BackToLeft3, BackToLeft4, BackToRight2, BackToRight3, BackToRight4}

    private List<GameObject> AllPieces=new List<GameObject>();
    /*[HideInInspector]*/ public List<GameObject> front=new List<GameObject>();
    [HideInInspector] public List<GameObject> back = new List<GameObject>();
    [HideInInspector] public List<GameObject> up = new List<GameObject>();
    [HideInInspector] public List<GameObject> down = new List<GameObject>();
    [HideInInspector] public List<GameObject> left = new List<GameObject>();
    [HideInInspector] public List<GameObject> right = new List<GameObject>();

    //pieces
    List<GameObject> MiddleHorizontal = new List<GameObject>();
    List<GameObject> MiddleVerticalFront = new List<GameObject>();
    List<GameObject> MiddleVerticalSide = new List<GameObject>();
    List<GameObject> Frontside = new List<GameObject>();
    List<GameObject> BackSide = new List<GameObject>();
    List<GameObject> BackSide2 = new List<GameObject>();
    List<GameObject> BackSide3 = new List<GameObject>();
    List<GameObject> BackSide4 = new List<GameObject>();
    List<GameObject> UpSide = new List<GameObject>();
    List<GameObject> DownSide = new List<GameObject>();
    List<GameObject> DownSide2 = new List<GameObject>();
    List<GameObject> DownSide3 = new List<GameObject>();
    List<GameObject> DownSide4 = new List<GameObject>();
    List<GameObject> LeftSide = new List<GameObject>();
    List<GameObject> RightSide = new List<GameObject>();
    List<GameObject> RightSide2 = new List<GameObject>();
    List<GameObject> RightSide3 = new List<GameObject>();
    List<GameObject> RightSide4 = new List<GameObject>();
    

    [HideInInspector] public List<Material> MatFront= new List<Material>();
    [HideInInspector] public List<Material> MatBack = new List<Material>();
    [HideInInspector] public List<Material> MatUp = new List<Material>();
    [HideInInspector] public List<Material> MatDown = new List<Material>();
    [HideInInspector] public List<Material> MatLeft = new List<Material>();
    [HideInInspector] public List<Material> MatRight = new List<Material>();
    //standard materials
    public Material Blue;
    public Material Green;
    public Material Orange;
    public Material Red;
    public Material White;
    public Material Yellow;

    public TypeOfCube CubeSelected;
    public Transform Center;
    public bool bCanRotate=true;
    public int speedAngle=5;
    public List<TypeOfRotate> UndoRotate;
    private bool bNotAddToUndo;
    private ReadSides SCR_ReadSides;
    [HideInInspector] public GameObject ActualSidePiece;
    [HideInInspector] public GameObject piece;
    OrbitCamera orbitState;
    public int NumberOfRandomMoves=2;
    public bool DebugMode;
    //complete scene
    public bool bCompleted;
    //button undo
    public Button Undo;


    //Timer 
    private float InicialTime;
    public string Timer;
    public bool StartTimer;
    public Text TimerText;

    //save properties
    [System.Serializable]
    public struct CubeProperties
    {
        public string SceneName;
        public float StateOfTimer;
        public List<string> FrontMaterial;
        public List<string> BackMaterial;
        public List<string> UpMaterial;
        public List<string> DownMaterial;
        public List<string> LeftMaterial;
        public List<string> RightMaterial;
    }
    public bool bLoadingScene;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Load") == 1)
        {
            bLoadingScene = true;
            PlayerPrefs.SetInt("Load", 0);
        }
    }

    void Start()
    {
        bCanRotate = true;
        SCR_ReadSides = GameObject.FindObjectOfType<ReadSides>();
        AllPieces = GetAllPieces();
        if (Center == null)
        {
            Center = AllPieces[0].transform.parent.transform;
        }
        GetPiecesForSides();
        orbitState = GameObject.FindObjectOfType<OrbitCamera>();
        UpdateTimer(InicialTime);
        if (!DebugMode)
        {
            if (!bLoadingScene)
            {
                StartCoroutine(RandomRotate());
            }
            else
            {
                LoadData();
            }
        }
        else
        {
            StartTimer = true;
        }
       
    }

   
    void Update()
    {
        if (StartTimer && !bCompleted)
        {
            InicialTime += Time.deltaTime;
            UpdateTimer(InicialTime);
        }

        if (!bCompleted && UndoRotate.Count!=0 && !Undo.interactable)
        {
            Undo.interactable = true;
        }
        else if((bCompleted || UndoRotate.Count == 0) && Undo.interactable)
        {
            Undo.interactable = false;
        }

       /* if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            LoadData();
        }*/
    }

    void UpdateTimer(float time)
    {
        int minutes = 0;
        int seconds = 0;
        int milliseconds = 0;
        if (time < 0)
        {
            time = 0;
        }
        minutes = (int)time / 60;
        seconds = (int)time % 60;
        milliseconds= (int)((time *1000)%1000);
        Timer = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds,milliseconds);
        if (TimerText != null)
        {
            TimerText.text = Timer;
        }
    }

   public void RotateSpecificPiece(TypeOfRotate type)
    {
        if (type == TypeOfRotate.UpToRight)
        {
            StartCoroutine(Rotate(UpSide, new Vector3(0, -1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.UpToLeft);
            }
        }
        else if (type == TypeOfRotate.UpToLeft)
        {
            StartCoroutine(Rotate(UpSide, new Vector3(0, 1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.UpToRight);
            }
        }
        else if (type == TypeOfRotate.MiddleToRight)
        {
            StartCoroutine(Rotate(MiddleHorizontal, new Vector3(0, -1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.MiddleToLeft);
            }
        }
        else if (type == TypeOfRotate.MiddleToLeft)
        {
            StartCoroutine(Rotate(MiddleHorizontal, new Vector3(0, 1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.MiddleToRight);
            }
        }
        else if (type == TypeOfRotate.DownToRight)
        {
            StartCoroutine(Rotate(DownSide, new Vector3(0, -1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToLeft);
            }
        }
        else if (type == TypeOfRotate.DownToRight2)
        {
            StartCoroutine(Rotate(DownSide2, new Vector3(0, -1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToLeft2);
            }
        }
        else if (type == TypeOfRotate.DownToRight3)
        {
            StartCoroutine(Rotate(DownSide3, new Vector3(0, -1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToLeft3);
            }
        }
        else if (type == TypeOfRotate.DownToRight4)
        {
            StartCoroutine(Rotate(DownSide4, new Vector3(0, -1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToLeft4);
            }
        }
        else if (type == TypeOfRotate.DownToLeft)
        {
            StartCoroutine(Rotate(DownSide, new Vector3(0, 1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToRight);
            }
        }
        else if (type == TypeOfRotate.DownToLeft2)
        {
            StartCoroutine(Rotate(DownSide2, new Vector3(0, 1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToRight2);
            }
        }
        else if (type == TypeOfRotate.DownToLeft3)
        {
            StartCoroutine(Rotate(DownSide3, new Vector3(0, 1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToRight3);
            }
        }
        else if (type == TypeOfRotate.DownToLeft4)
        {
            StartCoroutine(Rotate(DownSide4, new Vector3(0, 1, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.DownToRight4);
            }
        }
        else if (type == TypeOfRotate.LeftToRight)
        {
            StartCoroutine(Rotate(LeftSide, new Vector3(0, 0, -1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.LeftToLeft);
            }
        }
        else if (type == TypeOfRotate.LeftToLeft)
        {
            StartCoroutine(Rotate(LeftSide, new Vector3(0, 0, 1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.LeftToRight);
            }
        }
        else if (type == TypeOfRotate.MiddleVerFrontToRight)
        {
            StartCoroutine(Rotate(MiddleVerticalFront, new Vector3(0, 0, -1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.MiddleVerFrontToLeft);
            }
        }
        else if (type == TypeOfRotate.MiddleVerFrontToLeft)
        {
            StartCoroutine(Rotate(MiddleVerticalFront, new Vector3(0, 0, 1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.MiddleVerFrontToRight);
            }
        }
        else if (type == TypeOfRotate.RightToRight)
        {
            StartCoroutine(Rotate(RightSide, new Vector3(0, 0, -1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToLeft);
            }
        }
        else if (type == TypeOfRotate.RightToRight2)
        {
            StartCoroutine(Rotate(RightSide2, new Vector3(0, 0, -1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToLeft2);
            }
        }
        else if (type == TypeOfRotate.RightToRight3)
        {
            StartCoroutine(Rotate(RightSide3, new Vector3(0, 0, -1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToLeft3);
            }
        }
        else if (type == TypeOfRotate.RightToRight4)
        {
            StartCoroutine(Rotate(RightSide4, new Vector3(0, 0, -1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToLeft4);
            }
        }
        else if (type == TypeOfRotate.RightToLeft)
        {
            StartCoroutine(Rotate(RightSide, new Vector3(0, 0, 1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToRight);
            }
        }
        else if (type == TypeOfRotate.RightToLeft2)
        {
            StartCoroutine(Rotate(RightSide2, new Vector3(0, 0, 1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToRight2);
            }
        }
        else if (type == TypeOfRotate.RightToLeft3)
        {
            StartCoroutine(Rotate(RightSide3, new Vector3(0, 0, 1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToRight3);
            }
        }
        else if (type == TypeOfRotate.RightToLeft4)
        {
            StartCoroutine(Rotate(RightSide4, new Vector3(0, 0, 1)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.RightToRight4);
            }
        }
        else if (type == TypeOfRotate.FrontToRight)
        {
            StartCoroutine(Rotate(Frontside, new Vector3(-1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.FrontToLeft);
            }
        }
        else if (type == TypeOfRotate.FrontToLeft)
        {
            StartCoroutine(Rotate(Frontside, new Vector3(1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.FrontToRight);
            }
        }
        else if (type == TypeOfRotate.MiddleSideToRight)
        {
            StartCoroutine(Rotate(MiddleVerticalSide, new Vector3(-1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.MiddleSideToLeft);
            }
        }
        else if (type == TypeOfRotate.MiddleSideToLeft)
        {
            StartCoroutine(Rotate(MiddleVerticalSide, new Vector3(1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.MiddleSideToRight);
            }
        }
        else if (type == TypeOfRotate.BackToRight)
        {
            StartCoroutine(Rotate(BackSide, new Vector3(-1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToLeft);
            }
        }
        else if (type == TypeOfRotate.BackToRight2)
        {
            StartCoroutine(Rotate(BackSide2, new Vector3(-1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToLeft2);
            }
        }
        else if (type == TypeOfRotate.BackToRight3)
        {
            StartCoroutine(Rotate(BackSide3, new Vector3(-1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToLeft3);
            }
        }
        else if (type == TypeOfRotate.BackToRight4)
        {
            StartCoroutine(Rotate(BackSide4, new Vector3(-1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToLeft4);
            }
        }
        else if (type == TypeOfRotate.BackToLeft)
        {
            StartCoroutine(Rotate(BackSide, new Vector3(1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToRight);
            }
        }
        else if (type == TypeOfRotate.BackToLeft2)
        {
            StartCoroutine(Rotate(BackSide2, new Vector3(1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToRight2);
            }
        }
        else if (type == TypeOfRotate.BackToLeft3)
        {
            StartCoroutine(Rotate(BackSide3, new Vector3(1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToRight3);
            }
        }
        else if (type == TypeOfRotate.BackToLeft4)
        {
            StartCoroutine(Rotate(BackSide4, new Vector3(1, 0, 0)));
            if (!bNotAddToUndo)
            {
                UndoRotate.Add(TypeOfRotate.BackToRight4);
            }
        }
    }




    List<GameObject> GetAllPieces()
    {
        List<GameObject> pieces= new List<GameObject>();
        GameObject[] list = GameObject.FindGameObjectsWithTag("piece");
        foreach (GameObject piece in list)
        {
            pieces.Add(piece);
        }

        return pieces;
    }

    public void GetPiecesForSides()
    {
        if (CubeSelected == TypeOfCube.ThreexThree)
        {
            MiddleHorizontal = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
            MiddleVerticalFront= AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
            MiddleVerticalSide= AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
            Frontside= AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
            BackSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 1);
            UpSide= AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 1);
            DownSide= AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
            LeftSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
            RightSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -1);

        }
        else if (CubeSelected == TypeOfCube.TwoxTwo)
        {
            MiddleHorizontal = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
            MiddleVerticalFront = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
            MiddleVerticalSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
            Frontside = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
            UpSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 1);         
            LeftSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);

        }
        else if (CubeSelected == TypeOfCube.FourxFour)
        {
            MiddleHorizontal = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
            MiddleVerticalFront = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
            MiddleVerticalSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
            Frontside = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
            BackSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 1);
            BackSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 2);
            UpSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 1);
            DownSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
            DownSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
            LeftSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
            RightSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -1);
            RightSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -2);

        }
        else if (CubeSelected == TypeOfCube.FivexFive)
        {
            MiddleHorizontal = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
            MiddleVerticalFront = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
            MiddleVerticalSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
            Frontside = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
            BackSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 1);
            BackSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 2);
            BackSide3 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 3);
            UpSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 1);
            DownSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
            DownSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
            DownSide3 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -3);
            LeftSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
            RightSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -1);
            RightSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -2);
            RightSide3 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -3);

        }
        else if (CubeSelected == TypeOfCube.SixxSix)
        {
            MiddleHorizontal = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
            MiddleVerticalFront = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
            MiddleVerticalSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
            Frontside = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
            BackSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 1);
            BackSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 2);
            BackSide3 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 3);
            BackSide4 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 4);
            UpSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 1);
            DownSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
            DownSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
            DownSide3 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -3);
            DownSide4 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -4);
            LeftSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
            RightSide = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -1);
            RightSide2 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -2);
            RightSide3 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -3);
            RightSide4 = AllPieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == -4);
        }
    }
   
    /// only usable when the player swipe the cube
    public void MovePieces(string direction)
    {
        string side = FindSide(ActualSidePiece);
        if (StartTimer && bCanRotate && !bCompleted)
        {
            if (side != "up" && side != "down")
            {
                //Debug.Log("side: " + side);
                if (direction == "left")
                {
                    if (Mathf.Round(piece.transform.localPosition.y) == 1)
                    {
                        RotateSpecificPiece(TypeOfRotate.UpToLeft);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == 0)
                    {
                        RotateSpecificPiece(TypeOfRotate.MiddleToLeft);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -1)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToLeft);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -2)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToLeft2);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -3)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToLeft3);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -4)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToLeft4);
                    }
                }
                else if (direction == "Right")
                {
                    if (Mathf.Round(piece.transform.localPosition.y) == 1)
                    {
                        RotateSpecificPiece(TypeOfRotate.UpToRight);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == 0)
                    {
                        RotateSpecificPiece(TypeOfRotate.MiddleToRight);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -1)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToRight);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -2)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToRight2);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -3)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToRight3);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.y) == -4)
                    {
                        RotateSpecificPiece(TypeOfRotate.DownToRight4);
                    }
                }
                else if (direction == "Up")
                {

                    if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.LeftToRight);
                    }

                    else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToRight);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToRight2);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToRight3);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToRight4);
                    }

                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToRight);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToRight2);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToRight3);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToRight4);
                    }

                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.FrontToRight);
                    }


                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                    }
                    if (CubeSelected == TypeOfCube.ThreexThree)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.TwoxTwo)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.FourxFour)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.FivexFive)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.SixxSix)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToLeft4);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                        }
                    }
                }
                else if (direction == "Down")
                {

                    if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                    }

                    else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToLeft);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToLeft2);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToLeft3);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "front")
                    {
                        RotateSpecificPiece(TypeOfRotate.RightToLeft4);
                    }

                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToLeft);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                    }
                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                    }


                    else if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "left")
                    {
                        RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                    }
                    if (CubeSelected == TypeOfCube.ThreexThree)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToRight);
                        }              
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.TwoxTwo)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                        }

                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.FourxFour)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToRight);
                        }

                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.FivexFive)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight3);
                        } 
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToRight);
                        }

                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                        }
                    }
                    else if (CubeSelected == TypeOfCube.SixxSix)
                    {
                        if (Mathf.Round(piece.transform.localPosition.z) == 1 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.LeftToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == 0 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -1 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -2 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -3 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "back")
                        {
                            RotateSpecificPiece(TypeOfRotate.RightToRight4);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 2 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight2);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 3 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight3);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 4 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.BackToRight4);
                        }
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == -1 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.FrontToRight);
                        }
                        
                        else if (Mathf.Round(piece.transform.localPosition.z) == -4 && Mathf.Round(piece.transform.localPosition.x) == 0 && side == "right")
                        {
                            RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                        }
                    }

                }

            }
            else
            {
                if (side == "up")
                {
                    if (orbitState.x >= 45.0f && orbitState.x < 135.0f || orbitState.x <= -225.0f && orbitState.x > -315.0f)
                    {
                        if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                                
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                    else if (orbitState.x >= 135.0f && orbitState.x < 225.0f || orbitState.x <= -135.0f && orbitState.x > -225.0f)
                    {
                        if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                    else if (orbitState.x >= 225.0f && orbitState.x < 315.0f || orbitState.x <= -45.0f && orbitState.x > -135.0f)
                    {
                        if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                    else if ((orbitState.x >= 315.0f && orbitState.x <= 360.0f || orbitState.x >= 0.0f && orbitState.x < 45.0f) || (orbitState.x <= -315.0f && orbitState.x >= -360.0f || orbitState.x < 0.0f && orbitState.x > -45.0f))
                    {
                        if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                }
                else if (side == "down")
                {
                    if (orbitState.x >= 45.0f && orbitState.x < 135.0f || orbitState.x <= -225.0f && orbitState.x > -315.0f)
                    {
                        if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                    else if (orbitState.x >= 135.0f && orbitState.x < 225.0f || orbitState.x <= -135.0f && orbitState.x > -225.0f)
                    {
                        if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                    else if (orbitState.x >= 225.0f && orbitState.x < 315.0f || orbitState.x <= -45.0f && orbitState.x > -135.0f)
                    {
                        if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                    else if ((orbitState.x >= 315.0f && orbitState.x <= 360.0f || orbitState.x >= 0.0f && orbitState.x < 45.0f) || (orbitState.x <= -315.0f && orbitState.x >= -360.0f || orbitState.x < 0.0f && orbitState.x > -45.0f))
                    {
                        if (direction == "Right")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft2);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft3);

                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToLeft4);

                            }
                        }
                        else if (direction == "left")
                        {
                            if (Mathf.Round(piece.transform.localPosition.z) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.LeftToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleVerFrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -2)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -3)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.z) == -4)
                            {
                                RotateSpecificPiece(TypeOfRotate.RightToRight4);
                            }
                        }
                        else if (direction == "Down")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToRight4);
                            }
                        }
                        else if (direction == "Up")
                        {
                            if (Mathf.Round(piece.transform.localPosition.x) == -1)
                            {
                                RotateSpecificPiece(TypeOfRotate.FrontToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 0)
                            {
                                RotateSpecificPiece(TypeOfRotate.MiddleSideToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 1)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 2)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft2);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 3)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft3);
                            }
                            else if (Mathf.Round(piece.transform.localPosition.x) == 4)
                            {
                                RotateSpecificPiece(TypeOfRotate.BackToLeft4);
                            }
                        }
                    }
                }
            }
        }
    }

    string FindSide(GameObject side)
    {
        foreach(GameObject ActualSide in front)
        {
            if (ActualSide==side)
            {
                return "front";
            }
        }
        foreach (GameObject ActualSide in back)
        {
            if (ActualSide == side)
            {
                return "back";
            }
        }
        foreach (GameObject ActualSide in up)
        {
            if (ActualSide == side)
            {
                return "up";
            }
        }
        foreach (GameObject ActualSide in down)
        {
            if (ActualSide == side)
            {
                return "down";
            }
        }
        foreach (GameObject ActualSide in left)
        {
            if (ActualSide == side)
            {
                return "left";
            }
        }
        foreach (GameObject ActualSide in right)
        {
            if (ActualSide == side)
            {
                return "right";
            }
        }

        return "none";
    }

    public void GetMaterials()
    {
        MatFront = ShowMaterials(front);
        MatBack = ShowMaterials(back);
        MatUp = ShowMaterials(up);
        MatDown = ShowMaterials(down);
        MatLeft = ShowMaterials(left);
        MatRight = ShowMaterials(right);
    }

    List<Material> ShowMaterials(List<GameObject> Objects)
    {
        List<Material> materials= new List<Material>();
        foreach (GameObject Object in Objects)
        {
           Material mat= Object.GetComponent<Renderer>().material;
            materials.Add(mat);
        }
            return materials;

    }

    IEnumerator Rotate(List<GameObject> pieces, Vector3 rotation)
    {
        bCanRotate = false;
        int angle = 0;
        while (angle < 90)
        {
            foreach(GameObject piece in pieces)
            {
                piece.transform.RotateAround(Center.position, rotation, speedAngle);
                
            }
            angle += speedAngle;
            yield return null;
        }

        
        bCanRotate = true;
        if (bNotAddToUndo)
        {
            bNotAddToUndo = false;
        }
        SCR_ReadSides.ReadState();
        if (StartTimer && !bCompleted) {
            yield return null;
            ScanStateOfCube();
        }
    }

    public void UndoStep()
    {
        if (UndoRotate.Count != 0)
        {
            if (bCanRotate)
            {
                bNotAddToUndo = true;
                RotateSpecificPiece(UndoRotate[UndoRotate.Count - 1]);
                UndoRotate.Remove(UndoRotate[UndoRotate.Count - 1]);
            }
        }
    }
  
    IEnumerator RandomRotate()
    {
        SCR_ReadSides.ReadState();
        
        List<TypeOfRotate> ListOfCommandAvailable = new List<TypeOfRotate>();
        if (CubeSelected == TypeOfCube.ThreexThree)
        {
            ListOfCommandAvailable= new List<TypeOfRotate>() { TypeOfRotate.UpToRight,TypeOfRotate.UpToLeft,TypeOfRotate.MiddleToRight,TypeOfRotate.MiddleToLeft,
                                                                          TypeOfRotate.DownToRight,TypeOfRotate.DownToLeft,TypeOfRotate.LeftToRight,TypeOfRotate.LeftToLeft,
                                                                          TypeOfRotate.MiddleVerFrontToRight,
                                                                          TypeOfRotate.MiddleVerFrontToLeft,TypeOfRotate.RightToRight,TypeOfRotate.RightToLeft,
                                                                          TypeOfRotate.FrontToRight,TypeOfRotate.FrontToLeft,TypeOfRotate.MiddleSideToRight,
                                                                          TypeOfRotate.MiddleSideToLeft,TypeOfRotate.BackToLeft,TypeOfRotate.BackToRight
                                                                          };
        }
        else if (CubeSelected == TypeOfCube.TwoxTwo)
        {
            ListOfCommandAvailable = new List<TypeOfRotate>() { TypeOfRotate.UpToRight,TypeOfRotate.UpToLeft,TypeOfRotate.MiddleToRight,TypeOfRotate.MiddleToLeft,
                                                                          TypeOfRotate.LeftToRight,TypeOfRotate.LeftToLeft,
                                                                          TypeOfRotate.MiddleVerFrontToRight,TypeOfRotate.MiddleVerFrontToLeft,
                                                                          TypeOfRotate.FrontToRight,TypeOfRotate.FrontToLeft,TypeOfRotate.MiddleSideToRight,TypeOfRotate.MiddleSideToLeft,
                                                                          };
        }
        else if (CubeSelected == TypeOfCube.FourxFour)
        {
            ListOfCommandAvailable = new List<TypeOfRotate>() { TypeOfRotate.UpToRight,TypeOfRotate.UpToLeft,TypeOfRotate.MiddleToRight,TypeOfRotate.MiddleToLeft,
                                                                          TypeOfRotate.DownToRight,TypeOfRotate.DownToRight2,
                                                                          TypeOfRotate.DownToLeft,TypeOfRotate.DownToLeft2,
                                                                          TypeOfRotate.LeftToRight,TypeOfRotate.LeftToLeft,
                                                                          TypeOfRotate.MiddleVerFrontToRight,TypeOfRotate.MiddleVerFrontToLeft,
                                                                          TypeOfRotate.RightToRight,TypeOfRotate.RightToRight2,
                                                                          TypeOfRotate.RightToLeft,TypeOfRotate.RightToLeft2,
                                                                          TypeOfRotate.FrontToRight,TypeOfRotate.FrontToLeft,TypeOfRotate.MiddleSideToRight,TypeOfRotate.MiddleSideToLeft,
                                                                          TypeOfRotate.BackToLeft,TypeOfRotate.BackToLeft2,
                                                                          TypeOfRotate.BackToRight,TypeOfRotate.BackToRight2};
        }
        else if (CubeSelected == TypeOfCube.FivexFive)
        {
            ListOfCommandAvailable = new List<TypeOfRotate>() { TypeOfRotate.UpToRight,TypeOfRotate.UpToLeft,TypeOfRotate.MiddleToRight,TypeOfRotate.MiddleToLeft,
                                                                          TypeOfRotate.DownToRight,TypeOfRotate.DownToRight2,TypeOfRotate.DownToRight3,
                                                                          TypeOfRotate.DownToLeft,TypeOfRotate.DownToLeft2,TypeOfRotate.DownToLeft3,
                                                                          TypeOfRotate.LeftToRight,TypeOfRotate.LeftToLeft,
                                                                          TypeOfRotate.MiddleVerFrontToRight,TypeOfRotate.MiddleVerFrontToLeft,
                                                                          TypeOfRotate.RightToRight,TypeOfRotate.RightToRight2,TypeOfRotate.RightToRight3,
                                                                          TypeOfRotate.RightToLeft,TypeOfRotate.RightToLeft2,TypeOfRotate.RightToLeft3,
                                                                          TypeOfRotate.FrontToRight,TypeOfRotate.FrontToLeft,TypeOfRotate.MiddleSideToRight,TypeOfRotate.MiddleSideToLeft,
                                                                          TypeOfRotate.BackToLeft,TypeOfRotate.BackToLeft2,TypeOfRotate.BackToLeft3,
                                                                          TypeOfRotate.BackToRight,TypeOfRotate.BackToRight2,TypeOfRotate.BackToRight3};
        }
        else if(CubeSelected == TypeOfCube.SixxSix)
        {
            ListOfCommandAvailable = new List<TypeOfRotate>() { TypeOfRotate.UpToRight,TypeOfRotate.UpToLeft,TypeOfRotate.MiddleToRight,TypeOfRotate.MiddleToLeft,
                                                                          TypeOfRotate.DownToRight,TypeOfRotate.DownToRight2,TypeOfRotate.DownToRight3,TypeOfRotate.DownToRight4,
                                                                          TypeOfRotate.DownToLeft,TypeOfRotate.DownToLeft2,TypeOfRotate.DownToLeft3,TypeOfRotate.DownToLeft4,
                                                                          TypeOfRotate.LeftToRight,TypeOfRotate.LeftToLeft,
                                                                          TypeOfRotate.MiddleVerFrontToRight,TypeOfRotate.MiddleVerFrontToLeft,
                                                                          TypeOfRotate.RightToRight,TypeOfRotate.RightToRight2,TypeOfRotate.RightToRight3,TypeOfRotate.RightToRight4,
                                                                          TypeOfRotate.RightToLeft,TypeOfRotate.RightToLeft2,TypeOfRotate.RightToLeft3,TypeOfRotate.RightToLeft4,
                                                                          TypeOfRotate.FrontToRight,TypeOfRotate.FrontToLeft,TypeOfRotate.MiddleSideToRight,TypeOfRotate.MiddleSideToLeft,
                                                                          TypeOfRotate.BackToLeft,TypeOfRotate.BackToLeft2,TypeOfRotate.BackToLeft3,TypeOfRotate.BackToLeft4,
                                                                          TypeOfRotate.BackToRight,TypeOfRotate.BackToRight2,TypeOfRotate.BackToRight3,TypeOfRotate.BackToRight4};
        }


        List<TypeOfRotate> RandomList = new List<TypeOfRotate>();

        for(int i=0; i < NumberOfRandomMoves; i++)
        {
            TypeOfRotate ActualMove = ListOfCommandAvailable[Random.Range(0,ListOfCommandAvailable.Count-1)];
            RandomList.Add(ActualMove);
        }
        yield return new WaitForSeconds(1.0f);
        foreach (TypeOfRotate move in RandomList)
        {
            bNotAddToUndo = true;
            RotateSpecificPiece(move);
            while (!bCanRotate)
            {
                yield return null;

            }

        }
        StartTimer = true;
    }
    //scan the cube if is completed
    void ScanStateOfCube()
    {
        if (!bCompleted)
        {
            int matfrontstate = 0;

            foreach (Material mat in MatFront)
            {
                if (mat.name == MatFront[0].name)
                {
                    matfrontstate++;
                }
            }
            int matBackstate = 0;
            foreach (Material mat in MatBack)
            {
                if (mat.name == MatBack[0].name)
                {
                    matBackstate++;
                }
            }
            int matUpstate = 0;
            foreach (Material mat in MatUp)
            {
                if (mat.name == MatUp[0].name)
                {
                    matUpstate++;
                }
            }
            int matDownstate = 0;
            foreach (Material mat in MatDown)
            {
                if (mat.name == MatDown[0].name)
                {
                    matDownstate++;
                }
            }
            int matLeftstate = 0;
            foreach (Material mat in MatLeft)
            {
                if (mat.name == MatLeft[0].name)
                {
                    matLeftstate++;
                }
            }
            int matRightstate = 0;
            foreach (Material mat in MatRight)
            {
                if (mat.name == MatRight[0].name)
                {
                    matRightstate++;
                }
            }
            //Debug.LogError(matfrontstate + " matfront:" + MatFront.Count);
            if (matfrontstate == MatFront.Count && matBackstate == MatBack.Count && matUpstate == MatUp.Count && matDownstate == MatDown.Count && matLeftstate == MatLeft.Count && matRightstate == MatRight.Count)
            {
                 orbitState.bCompletedCinematic = true;
                 bCompleted = true;
            }
        }
    }

    public void SaveGame()
    {
        SCR_ReadSides.ReadState();
        CubeProperties data = new CubeProperties();

        data.SceneName = SceneManager.GetActiveScene().name;
        data.StateOfTimer = InicialTime;

        for (int i = 1; i < 7; i++)
        {
            List<string> Tempmaterial = new List<string>();
            if (i == 1)
            {
                foreach (Material mat in MatFront)
                {
                    Tempmaterial.Add(mat.name);
                }
                data.FrontMaterial = Tempmaterial;
            }
            else if (i == 2)
            {
                foreach (Material mat in MatBack)
                {
                    Tempmaterial.Add(mat.name);
                }
                data.BackMaterial = Tempmaterial;
            }
            else if (i == 3)
            {
                foreach (Material mat in MatUp)
                {
                    Tempmaterial.Add(mat.name);
                }
                data.UpMaterial = Tempmaterial;
            }
            else if (i == 4)
            {
                foreach (Material mat in MatDown)
                {
                    Tempmaterial.Add(mat.name);
                }
                data.DownMaterial = Tempmaterial;
            }
            else if (i == 5)
            {
                foreach (Material mat in MatLeft)
                {
                    Tempmaterial.Add(mat.name);
                }
                data.LeftMaterial = Tempmaterial;
            }
            else if (i == 6)
            {
                foreach (Material mat in MatRight)
                {
                    Tempmaterial.Add(mat.name);
                }
                data.RightMaterial = Tempmaterial;
            }
           
        }
        if (File.Exists(Application.dataPath + "saveCube.txt"))
        {
            File.Delete(Application.dataPath + "saveCube.txt");
        }

        string jsonSerializer = JsonUtility.ToJson(data);
        File.WriteAllText(Application.dataPath + "saveCube.txt",jsonSerializer);
        //Debug.LogError(Application.dataPath + "saveCube.txt");
    }

    void LoadData()
    {
        if (File.Exists(Application.dataPath + "saveCube.txt")) {
            CubeProperties LoadData = JsonUtility.FromJson<CubeProperties>(File.ReadAllText(Application.dataPath + "saveCube.txt"));
            SCR_ReadSides.ReadState();

            InicialTime = LoadData.StateOfTimer;

            List<List<GameObject>> sidelists = new List<List<GameObject>>() { front, back, up, down, left, right };
            List<List<string>> NameMatLists = new List<List<string>>() { LoadData.FrontMaterial, LoadData.BackMaterial, LoadData.UpMaterial, LoadData.DownMaterial, LoadData.LeftMaterial, LoadData.RightMaterial };

            for (int i = 0; i < sidelists.Count; i++)
            {
                for (int t = 0; t < sidelists[i].Count; t++)
                {
                    if (NameMatLists[i][t].Contains("Blue"))
                    {
                        sidelists[i][t].GetComponent<Renderer>().material = Blue;
                    }
                    else if (NameMatLists[i][t].Contains("Green"))
                    {
                        sidelists[i][t].GetComponent<Renderer>().material = Green;
                    }
                    else if (NameMatLists[i][t].Contains("Orange"))
                    {
                        sidelists[i][t].GetComponent<Renderer>().material = Orange;
                    }
                    else if (NameMatLists[i][t].Contains("Red"))
                    {
                        sidelists[i][t].GetComponent<Renderer>().material = Red;
                    }
                    else if (NameMatLists[i][t].Contains("white"))
                    {
                        sidelists[i][t].GetComponent<Renderer>().material = White;
                    }
                    else if (NameMatLists[i][t].Contains("Yellow"))
                    {
                        sidelists[i][t].GetComponent<Renderer>().material = Yellow;
                    }
                }
            }
        }
        StartTimer = true;
    }
}
