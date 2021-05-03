using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 SecondPressPos;
    Vector2 CurrentSwipe;
    ReadSides SCR_RS;
    GameState SCR_GS;
    public LayerMask Mask;
    public bool Dragging;
   
    // Start is called before the first frame update
    void Start()
    {
        SCR_RS = GameObject.FindObjectOfType<ReadSides>();
        SCR_GS= GameObject.FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        SwipePiece();
    }

    public void SwipePiece()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SCR_RS.ReadState();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 50.0f, Mask))
            {
                Dragging = true;
                SCR_GS.ActualSidePiece = hit.collider.gameObject;
                SCR_GS.piece = hit.transform.parent.gameObject;
                firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
               
            }
        }
        if (Dragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                SecondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                CurrentSwipe = new Vector2(SecondPressPos.x - firstPressPos.x, SecondPressPos.y - firstPressPos.y);
                CurrentSwipe.Normalize();

                if (leftSwipe(CurrentSwipe))
                {
                    SCR_GS.MovePieces("left");
                   // Debug.Log("left");
                }
                else if (rightSwipe(CurrentSwipe))
                {
                    SCR_GS.MovePieces("Right");
                    //Debug.Log("Right");
                }
                else if (UpSwipe(CurrentSwipe))
                {
                    SCR_GS.MovePieces("Up");
                   // Debug.Log("Up");
                }
                else if (DownSwipe(CurrentSwipe))
                {
                    SCR_GS.MovePieces("Down");
                   // Debug.Log("Down");
                }
                Dragging = false;
            }
        }
       
       
    }

    bool leftSwipe(Vector2 swipe)
    {
        return CurrentSwipe.x < 0 && CurrentSwipe.y > -0.5f && CurrentSwipe.y < 0.5f;
    }

    bool rightSwipe(Vector2 swipe)
    {
        return CurrentSwipe.x > 0 && CurrentSwipe.y > -0.5f && CurrentSwipe.y < 0.5f;
    }

    bool UpSwipe(Vector2 swipe)
    {
        return CurrentSwipe.y > 0 && CurrentSwipe.x > -0.5f && CurrentSwipe.x < 0.5f;
    }
    bool DownSwipe(Vector2 swipe)
    {
        return CurrentSwipe.y < 0 && CurrentSwipe.x > -0.5f && CurrentSwipe.x < 0.5f;
    }
}
