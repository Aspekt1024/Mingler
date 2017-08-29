using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private const int LEFT_MOUSE_BUTTON = 0;
    private const int RIGHT_MOUSE_BUTTON = 1;

    private static InputManager inputManager;
    public static InputManager Instance
    {
        get { return inputManager; }
    }

    private void Awake()
    {
        if (inputManager != null)
        {
            Debug.LogError("Found multiple instances of AppHandler. There should only be one!");
            enabled = false;
        }
        inputManager = FindObjectOfType<InputManager>();
        if (inputManager == null)
        {
            Debug.LogError("Could not find an instance of AppHandler. Please add one to the scene");
        }
    }

    public static Vector2 GetPointerPosition()
    {
#if UNITY_EDITOR
        Vector2 pos = AppHandler.Instance.MainCam.ScreenToWorldPoint(Input.mousePosition);
        return pos;
#endif  
        if (Input.touches.Length == 0) return Vector2.zero;

        return AppHandler.Instance.MainCam.ScreenToWorldPoint(Input.touches[0].position);
    }
}
