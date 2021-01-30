using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Button : MonoBehaviour
{
    private Canvas CanvasObject; // Assign in inspector

    private void Awake()
    {
        CanvasObject = GetComponent<Canvas>();
        CanvasObject.enabled = false;
    }

    void Start()
    {
        // CanvasObject = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CanvasObject.enabled = !CanvasObject.enabled;
        }
    }
}
