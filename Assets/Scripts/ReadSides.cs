using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadSides : MonoBehaviour
{
    public Transform RayFront;
    public Transform RayBack;
    public Transform RayLeft;
    public Transform RayRight;
    public Transform RayUp;
    public Transform RayDown;

    public LayerMask Mask;

    public GameObject EmptyExample;

    public float MaxDistanceRay=1.5f;

    private List<GameObject> FrontRays = new List<GameObject>();
    private List<GameObject> BackRays = new List<GameObject>();
    private List<GameObject> LeftRays = new List<GameObject>();
    private List<GameObject> RightRays = new List<GameObject>();
    private List<GameObject> UpRays = new List<GameObject>();
    private List<GameObject> DownRays = new List<GameObject>();

    GameState SCR_GameState;
    public int TypeOfCube;
    // Start is called before the first frame update
    void Start()
    {
        SetRayTransforms();
        SCR_GameState = FindObjectOfType<GameState>();
        
    }

   /* private void Update()
    {
        ReadState();
    }*/

    public void ReadState()
    {
        if (SCR_GameState == null)
        {
            SCR_GameState = FindObjectOfType<GameState>();
        }

        SCR_GameState.up = ReadFace(UpRays, RayUp);
        SCR_GameState.down = ReadFace(DownRays, RayDown);
        SCR_GameState.left = ReadFace(LeftRays, RayLeft);
        SCR_GameState.right = ReadFace(RightRays, RayRight);
        SCR_GameState.front = ReadFace(FrontRays, RayFront);
        SCR_GameState.back = ReadFace(BackRays, RayBack);
        SCR_GameState.GetPiecesForSides();
        SCR_GameState.GetMaterials();
    }


    void SetRayTransforms()
    {
        UpRays = CreateRays(RayUp, new Vector3(90, 90, 0));
        DownRays = CreateRays(RayDown, new Vector3(270,90,0));
        FrontRays = CreateRays(RayFront, new Vector3(0, 90, 0));
        BackRays = CreateRays(RayBack, new Vector3(0,270, 0));
        LeftRays = CreateRays(RayLeft, new Vector3(0, 180, 0));
        RightRays = CreateRays(RayRight, new Vector3(0, 0, 0));
    }

    List<GameObject> CreateRays(Transform RayTransform,Vector3 Direction)
    {
        int Raycount = 0;
        List<GameObject> Rays = new List<GameObject>();
        if (TypeOfCube == 2)
        {
            for (int y = 1; y > -1; y--)
            {
                for (int x = -1; x < 1; x++)
                {
                    Vector3 StartPosition = new Vector3(RayTransform.localPosition.x + x, RayTransform.localPosition.y + y, RayTransform.localPosition.z);
                    GameObject RayStart = Instantiate(EmptyExample, StartPosition, Quaternion.identity, RayTransform);
                    RayStart.name = Raycount.ToString();
                    Rays.Add(RayStart);
                    Raycount++;
                }
            }
        }
        if (TypeOfCube == 3)
        {
            for (int y = 1; y > -2; y--)
            {
                for (int x = -1; x < 2; x++)
                {
                    Vector3 StartPosition = new Vector3(RayTransform.localPosition.x + x, RayTransform.localPosition.y + y, RayTransform.localPosition.z);
                    GameObject RayStart = Instantiate(EmptyExample, StartPosition, Quaternion.identity, RayTransform);
                    RayStart.name = Raycount.ToString();
                    Rays.Add(RayStart);
                    Raycount++;
                }
            }
        }
        if (TypeOfCube == 4)
        {
            for (int y = 1; y > -3; y--)
            {
                for (int x = -1; x < 3; x++)
                {
                    Vector3 StartPosition = new Vector3(RayTransform.localPosition.x + x, RayTransform.localPosition.y + y, RayTransform.localPosition.z);
                    GameObject RayStart = Instantiate(EmptyExample, StartPosition, Quaternion.identity, RayTransform);
                    RayStart.name = Raycount.ToString();
                    Rays.Add(RayStart);
                    Raycount++;
                }
            }
        }
        if (TypeOfCube == 5)
        {
            for (int y = 1; y > -4; y--)
            {
                for (int x = -1; x < 4; x++)
                {
                    Vector3 StartPosition = new Vector3(RayTransform.localPosition.x + x, RayTransform.localPosition.y + y, RayTransform.localPosition.z);
                    GameObject RayStart = Instantiate(EmptyExample, StartPosition, Quaternion.identity, RayTransform);
                    RayStart.name = Raycount.ToString();
                    Rays.Add(RayStart);
                    Raycount++;
                }
            }
        }
        if (TypeOfCube == 6)
        {
            for (int y = 1; y > -5; y--)
            {
                for (int x = -1; x < 5; x++)
                {
                    Vector3 StartPosition = new Vector3(RayTransform.localPosition.x + x, RayTransform.localPosition.y + y, RayTransform.localPosition.z);
                    GameObject RayStart = Instantiate(EmptyExample, StartPosition, Quaternion.identity, RayTransform);
                    RayStart.name = Raycount.ToString();
                    Rays.Add(RayStart);
                    Raycount++;
                }
            }
        }
        RayTransform.localRotation = Quaternion.Euler(Direction);
        
        return Rays;
    }

    public List<GameObject> ReadFace(List<GameObject>ListOfRays, Transform RayTransform)
    {
        List<GameObject>FaceHits= new List<GameObject>();

        foreach (GameObject RayStart in ListOfRays)
        {
            Vector3 ray = RayStart.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(ray, RayTransform.forward, out hit, MaxDistanceRay, Mask))
            {
                Debug.DrawRay(ray, RayTransform.forward * hit.distance, Color.green);
                FaceHits.Add(hit.collider.gameObject);
            }
            else
            {
                Debug.DrawRay(ray, RayTransform.forward * 20, Color.red);
            }

        }
            return FaceHits;
    }
   
}
