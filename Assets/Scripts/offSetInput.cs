using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offSetInput : MonoBehaviour {

    public Transform CameraRig;
    
    public void UpCameraRig()
    {   
        CameraRig.position += new Vector3(0, 0.1f, 0);
    }

    public void DownCameraRig()
    {     
        CameraRig.position += new Vector3(0, -0.1f, 0);
    }

    public void LeftCameraRig()
    {
        CameraRig.position += new Vector3(-0.1f, 0, 0);
    }

    public void RightCameraRig()
    {
        CameraRig.position += new Vector3(0.1f, 0, 0);
    }

    public void ForwardCameraRig()
    {
        CameraRig.position += new Vector3(0, 0, 0.1f);
    }

    public void BackCameraRig()
    {
        CameraRig.position += new Vector3(0, 0, -0.1f);
    }

    public void FinishOffset()
    {
        this.gameObject.SetActive(false);
    }
}
