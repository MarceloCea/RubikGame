using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;


   
    public float x = 0.0f;
    float y = 0.0f;

    Swipe swipe;
    //cinematic complete
   
    CompleteMenu CompMenu;
    public bool bCompletedCinematic;
    public float xComplete=0;
    public float velocity = 1f;
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
       
        swipe = GameObject.FindObjectOfType<Swipe>();
        CompMenu= GameObject.FindObjectOfType<CompleteMenu>();
    }

    void LateUpdate()
    {
        if (target)
        {
            
            if (x>360.0f||x<-360.0f)
            {
                x = 0;
            }
            if (!bCompletedCinematic)
            {
                if (Input.GetMouseButton(1) || !swipe.Dragging && Input.GetMouseButton(0))
                {

                    x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                    y = ClampAngle(y, yMinLimit, yMaxLimit);


                }
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    distance -= Input.GetAxis("Mouse ScrollWheel") * velocity;
                    distance = Mathf.Clamp(distance, distanceMin, distanceMax);
                }
            }
            else
            {
                if (xComplete < 360.0f)
                {
                    xComplete += Time.deltaTime * 65.0f;
                    x += Time.deltaTime * 65.0f;
                }
                else if(xComplete>=360.0f)
                {
                    bCompletedCinematic = false;
                    CompMenu.ShowCompleteMenu();
                }
            }
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;
           
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
