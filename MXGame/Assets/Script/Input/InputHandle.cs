using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandle : MonoBehaviour
{
    private Camera MainCamera;

    private void Awake()
    {
        MainCamera = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext Context)
    {
        if (!Context.canceled) return;

        var RayHit = Physics2D.GetRayIntersection(MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));

        if (!RayHit.collider) return;
        
        var OnDetect =  RayHit.collider.gameObject.GetComponent<House>();

        if (OnDetect)
        {
            OnDetect.OnClick();
        }
    }
}
