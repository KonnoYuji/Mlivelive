using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PhotonManager : Photon.MonoBehaviour {

    static private PhotonManager _instance;
    static public PhotonManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PhotonManager>();
            }
            return _instance;
        }
    }

    public Button ConPhotonButton;
    
    public Button JoinVWorldButton;

    public Button JoinVRoomButton;
    
    public Button LeaveButton;

    //public Button MakeRoomAndJoinButton;
    //public Button JoinRoomButton;
    //public GameObject playerSelection;

    public GameObject offset;

    public Text status;
    public Text Error;
    public Text RcmdCnt;

    public RoomInfo[] rooms;
    public UnityAction leaveEvent;

    private GameObject myPlayer;

    private int playerNumOneFrameBefore = 0;

    private string joinableVRoom;

    private string joinableVWorld;

    // [SerializeField]
    // private GameObject mainChar, vipChar, audienceChar;

    [SerializeField]
    private bool isDebuged = false;

    public enum PlayerStyle
    {
        Main, Vip, Audience
    };

    // Use this for initialization
    void Awake()
    {
        ConPhotonButton.onClick.AddListener(ConnectPhoton);
        LeaveButton.onClick.AddListener(LeaveRoom);

        PhotonNetwork.networkingPeer.QuickResendAttempts = 4;    
        PhotonNetwork.CrcCheckEnabled = true;
        PhotonNetwork.MaxResendsBeforeDisconnect = 10;

        if(!isDebuged)
        {
            RcmdCnt.text = "";
        }

    }

    private void Update()
    {
        status.text = PhotonNetwork.connectionStateDetailed.ToString();

        if (!PhotonNetwork.connected)
        {
            return;
        }

        if(isDebuged)
        {
            RcmdCnt.text = "RcmdCnt : " + PhotonNetwork.ResentReliableCommands.ToString() + "\n";
        }

        if(!PhotonNetwork.inRoom || playerNumOneFrameBefore == PhotonNetwork.room.PlayerCount)
        {
            return;
        }

        if(playerNumOneFrameBefore > PhotonNetwork.room.PlayerCount && PhotonNetwork.isMasterClient)
        {            
            RemoveConFailedClient();
        }

        playerNumOneFrameBefore = PhotonNetwork.room.PlayerCount;

    }
    private void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }

    private void OnJoinedLobby()
    {
        Debug.Log("PhotonManager OnJoinedLobby");
        status.text = "Connected to Photon Cloud";
        Error.text = "ErrorMsg";
        ConPhotonButton.interactable = false;
    }

    private void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom!");        
        JoinVRoomButton.interactable = false;
        JoinVWorldButton.interactable = false;
        LeaveButton.interactable = true;     

        StartCoroutine(InitializeCameraPosition());   
    }
    
    private IEnumerator InitializeCameraPosition()
    {
        yield return null;
        yield return null;
        yield return null;

        var controllers = FindObjectsOfType<VWorldCharController>();

        for(int i=0; i<controllers.Length; i++)
        {
            if(controllers[i].name.Contains("unitychan"))
            {
                Camera.main.transform.parent.transform.position = controllers[i].transform.position + new Vector3(0, 0, -2.0f);
                break;
            }
        }    
        offset.SetActive(true);
    }
    private void OnReceivedRoomListUpdate()
    {
        rooms= PhotonNetwork.GetRoomList();

        if (rooms.Length == 0)
        {
            Debug.Log("ルームが一つもありません");
        }
        else
        {
            //ルームが1件以上ある時ループでRoomInfo情報をログ出力
            for (int i = 0; i < rooms.Length; i++)
            {
                if(rooms[i].Name.Contains("VRoom"))
                {
                    JoinVRoomButton.interactable = true;
                    joinableVRoom = rooms[i].Name;
                    JoinVRoomButton.onClick.AddListener(JoinVRoom);
                }
                else if(rooms[i].Name.Contains("VWorld"))
                {
                    JoinVWorldButton.interactable = true;
                    joinableVWorld = rooms[i].Name;
                    JoinVWorldButton.onClick.AddListener(JoinVWorld);                    
                }                
            }                                     
        }
    }

    private void JoinVRoom()
    {        
        if(!PhotonNetwork.JoinRoom(joinableVRoom))
        {
            Debug.Log("Failed to Join Room");            
        }        
    }

    private void JoinVWorld()
    {        
        if(!PhotonNetwork.JoinRoom(joinableVWorld))
        {
            Debug.Log("Failed to Join Room");            
        }        
    }

    private void LeaveRoom()
    {
        if(leaveEvent != null)
        {
            leaveEvent();
        }

        PhotonNetwork.LeaveRoom();
    }

    private void OnLeftRoom()
    {
        LeaveButton.interactable = false;
        DeletePhotonObi();
        playerNumOneFrameBefore = 0;
     
        if (offset.activeSelf)
        {
            offset.SetActive(false);
        }
    }

    private void OnPhotonJoinRoomFailed()
    {
        Debug.Log("PhotonManager: ルーム入室に失敗");

    }

    private void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed To Photon; error code {0}", cause);
        Error.text = string.Format("Err: {0}", cause);
    }

    private void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed ; error code {0}", cause);
        Debug.LogErrorFormat("Recent command counter : {0}", PhotonNetwork.ResentReliableCommands.ToString());
        Debug.LogErrorFormat("PacketLossCountByCrc : {0}", PhotonNetwork.PacketLossByCrcCheck.ToString());
        Debug.LogErrorFormat("Ping ; {0}", PhotonNetwork.GetPing().ToString());
        Error.text = string.Format("Err: {0}", cause);
        playerNumOneFrameBefore = 0;

        if (leaveEvent != null)
        {
            leaveEvent();
        }

        JoinVRoomButton.interactable = false;

        JoinVWorldButton.interactable = false;        
   
        if(offset.activeSelf)
        {
            offset.SetActive(false);
        }        
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
        ConPhotonButton.interactable = true;
        DeletePhotonObi();
    }

    private void DeletePhotonObi()
    {
        var objs = FindObjectsOfType<PhotonView>();

        for(int i=0; i<objs.Length; i++)
        {
            Destroy(objs[i].gameObject);
        }
    }

    private void RemoveConFailedClient()
    {
        var charControllers = FindObjectsOfType<BaseVCharacter>();

        if(charControllers == null || charControllers.Length == 0)
        {
            Debug.Log("No CharController");
            return;
        }

        for(int i = 0; i<charControllers.Length; i++)
        {
            var isMine = charControllers[i].GetComponent<PhotonView>().isMine;
            if(!charControllers[i].isMyPlayer && isMine)
            {
                PhotonNetwork.Destroy(charControllers[i].gameObject);
            }
        }
    }
}
