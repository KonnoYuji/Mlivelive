using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManagerAutoLogin : MonoBehaviour {

    public RoomInfo[] rooms;

    private GameObject myPlayer;

    [SerializeField]
    private GameObject vipChar;

    public enum PlayerStyle
    {
        Main, Vip, Crowd
    };

    // Use this for initialization
    void Awake()
    {
        PhotonNetwork.networkingPeer.QuickResendAttempts = 6;
        PhotonNetwork.networkingPeer.SentCountAllowance = 14;
        ConnectPhoton();
    }

    private void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }

    private void OnJoinedLobby()
    {
        Debug.Log("PhotonManager OnJoinedLobby");
        CreateAndJoinRoom();
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
        InstantiateMyChar();
    }

    public void InstantiateMyChar()
    {
        Vector3 initialPosition = new Vector3(0.0f, 0.0f, 0.0f);

        myPlayer = PhotonNetwork.Instantiate(vipChar.name, vipChar.transform.position, vipChar.transform.rotation, 0);
        initialPosition = vipChar.transform.position;
    }

    private void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
        rooms = PhotonNetwork.GetRoomList();

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
            JoinRoom();
        }
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(rooms[0].Name);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnLeftRoom()
    {
        Debug.Log("Left room.");
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
    }

    private void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed ; error code {0}", cause);
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
    }
}
