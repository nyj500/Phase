using UnityEngine;

public class MouseCursorController : MonoBehaviour
{
    public float idleTime = 2f;
    private float timer;
    private Vector3 lastMousePosition;
    private bool cursorHidden = false;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            timer = 0f;
            lastMousePosition = Input.mousePosition;

            if (cursorHidden)
            {
                Cursor.visible = true;
                cursorHidden = false;
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= idleTime && !cursorHidden)
            {
                Cursor.visible = false;
                cursorHidden = true;
            }
        }
    }
}
