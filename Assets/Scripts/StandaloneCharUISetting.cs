using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StandaloneCharUISetting : MonoBehaviour {

    static private StandaloneCharUISetting _instance;
    static public StandaloneCharUISetting Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<StandaloneCharUISetting>();                
            }
            return _instance;
        }
    }

    [SerializeField]
    private Button jump;

    [SerializeField]
    private Button UpHand;

	public void RegisterJumpMethod(UnityAction action)
    {
        jump.onClick.AddListener(action);
    }

    public void RegisterUpHandMethod(UnityAction action)
    {
        UpHand.onClick.AddListener(action);
    }
}
