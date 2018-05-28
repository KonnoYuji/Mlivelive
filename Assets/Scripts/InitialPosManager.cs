using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPosManager : MonoBehaviour {

    static private InitialPosManager _instance;
    static public InitialPosManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<InitialPosManager>();
                return _instance;
            }
            return _instance;
        }

    }

    static private Dictionary<PhotonManager.PlayerStyle, Vector3> offsetDicWithCamera;

    private void Awake()
    {
        offsetDicWithCamera = new Dictionary<PhotonManager.PlayerStyle, Vector3>();
        offsetDicWithCamera.Add(PhotonManager.PlayerStyle.Main, new Vector3(0.35f, -2.38f, 24.014f));
        offsetDicWithCamera.Add(PhotonManager.PlayerStyle.Vip, new Vector3(-1.67f, -3.531f, 21.81f));
        offsetDicWithCamera.Add(PhotonManager.PlayerStyle.Audience, new Vector3(4.612f, -3.71f, 16.7f));
    }

    public Vector3 MyCharOffset(PhotonManager.PlayerStyle myChar)
    {
        return offsetDicWithCamera[myChar];
    }
}
