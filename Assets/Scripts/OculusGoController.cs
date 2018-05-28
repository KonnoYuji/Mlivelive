using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OculusGoController : MonoBehaviour {
    
    public UnityAction TouchedPad;
    public UnityAction ClickedPad;
    public UnityAction ClickedTrigger;

    private OculusGoController controller;

    private void Update()
    {
        OVRInput.Update();

        if (OVRInput.Get(OVRInput.Button.One))
        {
            ClickedPad();
            return;
        }
        else if(OVRInput.Get(OVRInput.Touch.One))
        {
            TouchedPad();
            return;
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            ClickedTrigger();
            return;
        }

    }
}
