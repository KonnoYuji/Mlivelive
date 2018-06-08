using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseVCharacter : Photon.MonoBehaviour {

    [SerializeField]
    protected Animator myAnim;

    [SerializeField]
    protected PhotonView myView;

    protected Vector3 currentPos;

#if VIVE
    protected ViveLeftHandController leftHand;
    protected ViveRightHandController rightHand;

#elif OCULUS_GO || OCULUS_RIFT || OCULUS_TOUCH
    protected OculusController oculusController;
#endif

    [SerializeField]
    protected GameObject[] offRenderingParts;

    public bool isMyPlayer = false;

	protected UnityAction AttachInputIvent;

	protected UnityAction DetachInputEvent;

	protected void Awake()
    {
        if(myView == null)
        {
            myView = GetComponent<PhotonView>();
        }

        if(myAnim == null)
        {
            myAnim = GetComponent<Animator>();
        }

        //コントローラー入力イベントの登録

		if(AttachInputIvent != null)
		{
			AttachInputIvent();
		}

		if(DetachInputEvent != null)
		{
			PhotonManager.Instance.leaveEvent += DetachInputEvent;	
		}    
    }

    private void OnDestroy()
    {
        Debug.LogFormat("PlayerName : {0}", this.gameObject.name);
        Debug.LogFormat("MasterPlayer : {0}", PhotonNetwork.isMasterClient);
        Debug.LogFormat("PhotonNetwork.inRoom : {0}", PhotonNetwork.inRoom);
        Debug.LogFormat("isMine : {0}", myView.isMine);        
    }
}
