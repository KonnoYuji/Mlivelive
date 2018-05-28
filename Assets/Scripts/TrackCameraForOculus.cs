using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraForOculus : MonoBehaviour {

    [SerializeField]
    private Transform Controller;

    [SerializeField]
    private GameObject UI;

    private Vector3 diffVector;

    private void Awake()
    {
        diffVector = transform.position - Controller.position;
        //var oculusController = FindObjectOfType<OculusGoController>();
        //oculusController.TouchedPad += () => {
        //    var active = UI.activeSelf;
        //    UI.SetActive(!active);
        //};
    }

    private void Update()
    {      
        transform.position = Controller.position + diffVector;
    }
}
