using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offSetInput : MonoBehaviour {

    public Transform CameraRig;

    public TrackCamera trackCamera;

    private bool settedTrackCamera = false;

    private void OnEnable()
    {
        var track = FindObjectOfType<TrackCamera>();

        if (track != null)
        {
            trackCamera = track;
        }
    }
    public void UpCameraRig()
    {
        if(trackCamera == null)
        {
            return;
        }

        CameraRig.position += new Vector3(0, 0.1f, 0);
    }

    public void DownCameraRig()
    {
        if (trackCamera == null)
        {
            return;
        }

        CameraRig.position += new Vector3(0, -0.1f, 0);
    }

    public void LeftCameraRig()
    {
        if (trackCamera == null)
        {
            return;
        }
        CameraRig.position += new Vector3(-0.1f, 0, 0);
    }

    public void RightCameraRig()
    {
        if (trackCamera == null)
        {
            return;
        }
        CameraRig.position += new Vector3(0.1f, 0, 0);
    }

    public void ForwardCameraRig()
    {
        if (trackCamera == null)
        {
            return;
        }
        CameraRig.position += new Vector3(0, 0, 0.1f);
    }

    public void BackCameraRig()
    {
        if (trackCamera == null)
        {
            return;
        }
        CameraRig.position += new Vector3(0, 0, -0.1f);
    }

    public void FinishOffset()
    {
        if (trackCamera == null)
        {
            return;
        }

        trackCamera.settedOffset = true;
        this.gameObject.SetActive(false);
    }
}
