using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCamera : MonoBehaviour {

    private Transform mainCamera;

    private Vector3 currentCameraPos;

    private float currentCameraRotationY;

    public bool settedOffset = false;

    private PhotonView myView;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        currentCameraPos = mainCamera.position;
        currentCameraRotationY = mainCamera.rotation.eulerAngles.y;
        myView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myView.isMine)
        {
            return;
        }

        if(!settedOffset)
        {
            return;
        }

        if (mainCamera.position != currentCameraPos)
        {
            transform.position = new Vector3(mainCamera.position.x, transform.position.y, mainCamera.position.z);
            currentCameraPos = mainCamera.position;
        }

        if (mainCamera.rotation.eulerAngles.y != currentCameraRotationY)
        {
            currentCameraRotationY = mainCamera.rotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, currentCameraRotationY, transform.rotation.eulerAngles.z);
        }

    }
}
