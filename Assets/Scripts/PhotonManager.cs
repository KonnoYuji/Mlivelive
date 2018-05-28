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
    public Button MakeRoomAndJoinButton;
    public Button JoinRoomButton;
    public Button LeaveButton;
    public GameObject playerSelection;
    public GameObject offset;

    public Text status;
    public Text Error;
    public RoomInfo[] rooms;
    public UnityAction leaveEvent;

    private GameObject myPlayer;

    [SerializeField]
    private GameObject mainChar, vipChar, audienceChar;

    public enum PlayerStyle
    {
        Main, Vip, Audience
    };

    // Use this for initialization
    void Awake()
    {
        ConPhotonButton.onClick.AddListener(ConnectPhoton);
        MakeRoomAndJoinButton.onClick.AddListener(CreateAndJoinRoom);
        LeaveButton.onClick.AddListener(LeaveRoom);

        PhotonNetwork.networkingPeer.QuickResendAttempts = 4;    
        PhotonNetwork.CrcCheckEnabled = true;
        PhotonNetwork.MaxResendsBeforeDisconnect = 10;
    }

    private void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }

    private void OnJoinedLobby()
    {
        Debug.Log("PhotonManager OnJoinedLobby");
        status.text = "Connected to Photon Cloud";
        ConPhotonButton.interactable = false;
        MakeRoomAndJoinButton.interactable = true;
    }

    private void CreateAndJoinRoom()
    {
        string userName = "YujiKonno";
        string userId = "001";
        PhotonNetwork.autoCleanUpPlayerObjects = false;

        //カスタムプロパティ
        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        customProp.Add("userId", userId); //ユーザID
        PhotonNetwork.SetPlayerCustomProperties(customProp);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = customProp;

        //ロビーで見えるルーム情報
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "userName", "userId" };
        roomOptions.MaxPlayers = 5; //部屋の最大人数
        roomOptions.IsOpen = true; //入室許可する
        roomOptions.IsVisible = true; //ロビーから見えるようにする
        //userIdが名前のルームがなければ作って入室、あれば普通に入室する。
        PhotonNetwork.JoinOrCreateRoom(userId, roomOptions, null);
    }

    private void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom!");
        status.text = "Joined Room";
        MakeRoomAndJoinButton.interactable = false;
        JoinRoomButton.interactable = false;
        LeaveButton.interactable = true;

        playerSelection.SetActive(true);
    }

    public void InstantiateMyChar(int charNum)
    {
        Vector3 initialPosition = new Vector3(0.0f, 0.0f, 0.0f);
        PlayerStyle myPlayerStyle;

        switch (charNum)
        {
            case (int)PlayerStyle.Main:

                myPlayerStyle = PlayerStyle.Main;
                if(mainChar != null)
                {
                    myPlayer = PhotonNetwork.Instantiate(mainChar.name, InitialPosManager.Instance.MyCharOffset(myPlayerStyle), mainChar.transform.rotation, 0);
                    initialPosition = myPlayer.transform.position;
                }               
                break;

            case (int)PlayerStyle.Vip:

                myPlayerStyle = PlayerStyle.Vip;
                if(vipChar != null)
                {
                    myPlayer = PhotonNetwork.Instantiate(vipChar.name, InitialPosManager.Instance.MyCharOffset(myPlayerStyle), vipChar.transform.rotation, 0);
                    initialPosition = vipChar.transform.position;
                }        
                break;

            case (int)PlayerStyle.Audience:

                myPlayerStyle = PlayerStyle.Audience;
                if(audienceChar != null)
                {
                    myPlayer = PhotonNetwork.Instantiate(audienceChar.name, InitialPosManager.Instance.MyCharOffset(myPlayerStyle), audienceChar.transform.rotation, 0);
                    initialPosition = audienceChar.transform.position;

                }
                break;
        }

        Camera.main.transform.parent.transform.position = initialPosition;
        playerSelection.SetActive(false);

#if VRMode
        offset.SetActive(true);
#endif
    }
    
    private void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
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
                Debug.Log("RoomName:" + rooms[i].Name);
                Debug.Log("userName:" + rooms[i].CustomProperties["userName"]);
                Debug.Log("userId:" + rooms[i].CustomProperties["userId"]);
            }
            JoinRoomButton.onClick.AddListener(JoinRoom);
            JoinRoomButton.interactable = true;
        }
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(rooms[0].Name);
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
        status.text = "Left Room";
        LeaveButton.interactable = false;
        DeletePhotonObi();
    }

    private void OnPhotonJoinRoomFailed()
    {
        Debug.Log("PhotonManager: ルーム入室に失敗");
    }

    private void OnPhotonCreateRoomFailed()
    {
        Debug.Log("PhotonManager: ルーム作成に失敗");
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
        Error.text = string.Format("Err: {0}", cause);
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
        status.text = "Disconnected from Photon";
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
}
