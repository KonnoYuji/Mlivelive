using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VWorldCharController : BaseVCharacter {
    private bool isJumped = false;

    private bool isHandUp = false;

    private void Awake()
    {
        if(!myView.isMine)
        {
            return;
        }   

        AttachInputIvent += RegisterInputEvent;
        //DetachInputEvent += RemoveInputEvent;

        base.Awake();
    }

    // Use this for initialization
    void Start () {        
        
        AudioManager.Instance.PlayCheerSound();
        currentPos = transform.position;

        if(myView.isMine)
        {
            for(int i=0; i<offRenderingParts.Length; i++)
            {
                offRenderingParts[i].SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Mathf.Approximately(Mathf.Floor(currentPos.x * 5), Mathf.Floor(transform.position.x * 5)) && 
            Mathf.Approximately(Mathf.Floor(currentPos.z * 5), Mathf.Floor(transform.position.z * 5)))
        {
            if(!isJumped)
            {
                myAnim.SetTrigger("Idle");                
            }
            return;
        }

        currentPos = transform.position;

        if(!isHandUp)
        {
            myAnim.SetTrigger("Walk");
        }
        
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(isJumped);
            stream.SendNext(isHandUp);
        }
        else
        {
            this.isJumped = (bool)stream.ReceiveNext();
            Jump(isJumped);
            this.isHandUp = (bool)stream.ReceiveNext();
            UpHand(isHandUp);
        }
    }

    private void ChangeJumpState()
    {
        if (myView.isMine)
        {
            isJumped = !isJumped;
            Jump(isJumped);
        }
    }

    private void ChangeHandUpState()
    {
        if (myView.isMine)
        {
            isHandUp = !isHandUp;
            UpHand(isHandUp);
        }
    }

    private void Jump(bool state)
    {
        bool? curentState = myAnim.GetBool("Jump");
        if (curentState == null || curentState == state)
        {
            return;
        }
        myAnim.SetBool("Jump", state);
    }

    private void UpHand(bool state)
    {
        bool? currentState = myAnim.GetBool("UpHand");
        if (currentState == null ||  currentState == state)
        {
            return;
        }
        myAnim.SetBool("UpHand", state);
    }

    void RegisterInputEvent()
    {
#if VIVE
        leftHand = FindObjectOfType<ViveLeftHandController>();
        rightHand = FindObjectOfType<ViveRightHandController>();

        if (leftHand != null && rightHand != null)
        {
            leftHand.TouchPadClicked += ChangeJumpState;
            rightHand.TouchPadClicked += ChangeHandUpState;
        }
#elif OCULUS_GO
        oculusController = FindObjectOfType<OculusController>();

        if(oculusController != null)
        {
            oculusController.ClickedPad += ChangeJumpState;
            oculusController.TouchedPad += ChangeHandUpState;
        }        
#elif OCULUS_RIFT
        oculusController = FindObjectOfType<OculusController>();

        if (oculusController != null)
        {
            oculusController.LeftDpad += ChangeJumpState;
            oculusController.RightDpad += ChangeHandUpState;
        }
#elif OCULUS_TOUCH
        oculusController = FindObjectOfType<OculusController>();

        if (oculusController != null)
        {
            oculusController.ThreeClicked += ChangeJumpState;
            oculusController.FourClicked += ChangeHandUpState;
        }
#elif DISPLAY
        var charUISetting = StandaloneCharUISetting.Instance;
        charUISetting.ListenJumpMethod(ChangeJumpState, true);
        charUISetting.ListenUpHandMethod(ChangeHandUpState, true);
#endif
    }

    void RemoveInputEvent()
    {
        //コントローラー入力
#if VIVE
        if (leftHand != null && rightHand != null)
        {       
            leftHand.TouchPadClicked -= ChangeJumpState;
            rightHand.TouchPadClicked -= ChangeHandUpState;
        }

#elif OCULUS_GO
        
        if(oculusController != null)
        {
            oculusController.ClickedPad -= ChangeJumpState;
            oculusController.TouchedPad -= ChangeHandUpState;
        }        
#elif OCULUS_RIFT

        if (oculusController != null)
        {
            oculusController.LeftDpad -= ChangeJumpState;
            oculusController.RightDpad -= ChangeHandUpState;
        }
#elif OCULUS_TOUCH
        if (oculusController != null)
        {
            oculusController.ThreeClicked -= ChangeJumpState;
            oculusController.FourClicked -= ChangeHandUpState;
        }

#elif DISPLAY
        var charUISetting = StandaloneCharUISetting.Instance;
        charUISetting.ListenJumpMethod(ChangeJumpState, false);
        charUISetting.ListenUpHandMethod(ChangeHandUpState, false);
#endif
    }
}
