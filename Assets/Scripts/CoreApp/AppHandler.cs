using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppHandler : MonoBehaviour {

    public Camera MainCam;

    private static AppHandler appHandler;
    public static AppHandler Instance
    {
        get { return appHandler; }
    }

    private void Awake()
    {
        if (appHandler != null)
        {
            Debug.LogError("Found multiple instances of AppHandler. There should only be one!");
            enabled = false;
        }
        appHandler = FindObjectOfType<AppHandler>();
        if (appHandler == null)
        {
            Debug.LogError("Could not find an instance of AppHandler. Please add one to the scene");
        }
    }

    private void Start()
    {
        MainCam = FindObjectOfType<Camera>();
    }

}
