using UnityEngine;

public class SwitchAndMoveCamera : MonoBehaviour
{
    /*
        The movement script was taken from: https://gist.github.com/RyanBreaker/932dc35302787d2f39df6b614a50c0c9
      
        Written by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.
        Converted to C# 27-02-13 - no credit wanted.
        Reformatted and cleaned by Ryan Breaker 23-6-18
        Original comment:
        Simple flycam I made, since I couldn't find any others made public.
        Made simple to use (drag and drop, done) for regular keyboard layout.
        
        Controls:
        WASD  : Directional movement
        Shift : Increase speed
        Space : Moves camera directly up per its local Y-axis
        Left Control : Moves camera directly down per its local Y-axis
    */

    private Camera OverviewCamera;
    private Vector3 OriginalPosition;
    private Quaternion OriginalRotation;
    private bool MovementActivated;

    float mainSpeed = 5.0f; //regular speed
    float shiftAdd = 10.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 50.0f; //Maximum speed when holdin gshift
    float camSens = 0.15f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    private void Start()
    {
        OverviewCamera = GetComponent<Camera>();
        OriginalPosition = transform.position;
        OriginalRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OverviewCamera.enabled = !OverviewCamera.enabled;
        }

        if (Input.GetKeyDown(KeyCode.R) && OverviewCamera.enabled)
        {
            transform.position = OriginalPosition;
            transform.rotation = OriginalRotation;
        }

        if (Input.GetKeyDown(KeyCode.M) && OverviewCamera.enabled)
        {
            MovementActivated = !MovementActivated;
        }

        if (OverviewCamera.enabled && MovementActivated)
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
            // Mouse camera angle done.  

            // Keyboard commands
            Vector3 p = GetBaseInput();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p *= totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p *= mainSpeed;
            }

            p *= Time.deltaTime;
            transform.Translate(p);
        } 
    }

    private Vector3 GetBaseInput()
    { 
        Vector3 p_Velocity = new Vector3();
        // Forwards
        if (Input.GetKey(KeyCode.W))
            p_Velocity += new Vector3(0, 0, 1);

        // Backwards
        if (Input.GetKey(KeyCode.S))
            p_Velocity += new Vector3(0, 0, -1);

        // Left
        if (Input.GetKey(KeyCode.A))
            p_Velocity += new Vector3(-1, 0, 0);

        // Right
        if (Input.GetKey(KeyCode.D))
            p_Velocity += new Vector3(1, 0, 0);

        // Up
        if (Input.GetKey(KeyCode.Space))
            p_Velocity += new Vector3(0, 1, 0);

        // Down
        if (Input.GetKey(KeyCode.LeftControl))
            p_Velocity += new Vector3(0, -1, 0);

        return p_Velocity;
    }
}
