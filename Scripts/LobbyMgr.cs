using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyMgr : MonoBehaviourPunCallbacks
{
    [Header("--기본 UI 관련 변수--")]
    public Text PlayerTxt;
    public Text ChatTxt;
    public InputField ChatIFd;
    public GameObject LoadingPanel;
    public Button BuildRoomBtn;
    public Button HelpBtn;
    public Button LogOutBtn;
    public GameObject HelpPanel;
    public Button HelpOutBtn;

    [Header("--방목록 관련 변수--")]
    public GameObject JoinRoomPanel;
    public GameObject scrollContents;
    public GameObject roomItem;
    public List<RoomInfo> myList = new List<RoomInfo>();

    [Header("--방내부 관련 변수--")]
    public GameObject JoinedRoomPanel;
    public Text RoomNameTxt;
    public Text RoomModeTxt;
    public Text RoomInfoTxt;
    public Button ChangeModeBtn;
    public Button ShuffleBtn;
    public Button StartBtn;
    public Button RoomOutBtn;

    private PhotonView PV;
    bool Chatmode = false;
    bool RoomModeCustom = true;

    public GameObject CanvasObj;
    public GameObject SettingPanel;
    [HideInInspector] public bool SettingPanelActive = false;
    public static LobbyMgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ClearFunc();

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            LoadingPanel.SetActive(false);

        PV = GetComponent<PhotonView>();

        BuildRoomBtn.onClick.AddListener(BuildRoomBtnFunc);

        HelpBtn.onClick.AddListener(() =>
        {
            HelpPanel.SetActive(true);
        });

        LogOutBtn.onClick.AddListener(() =>
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("TitleScene");
        });

        HelpOutBtn.onClick.AddListener(() =>
        {
            HelpPanel.SetActive(false);
        });

        RoomOutBtn.onClick.AddListener(() =>
        {
            PhotonNetwork.LeaveRoom();
            JoinRoomPanel.SetActive(true);
            JoinedRoomPanel.SetActive(false);

            ClearFunc();
        });

        ChangeModeBtn.onClick.AddListener(() =>
        {
            //not builded
        });

        ShuffleBtn.onClick.AddListener(() =>
        {
            int ii = Random.Range(0, GlobalValue.playerlist.Count);
            for (int i = 0; i < GlobalValue.playerlist.Count; i++)
            {
                if (i == ii)
                {
                    GlobalValue.playerlist[i].IamTeemo = false;
                    if (GlobalValue.playerlist[i].PlayerName == GlobalValue.Unique_ID)
                        GlobalValue.IamTeemo = false;
                }
                else
                {
                    GlobalValue.playerlist[i].IamTeemo = true;
                    GlobalValue.IamTeemo = true;
                }                    
            }
            PV.RPC("Announce", RpcTarget.Others, Format(GlobalValue.playerlist));
            RoomSetting();
        });

        StartBtn.onClick.AddListener(() =>
        {
            //PV.RPC("LoadGameScene", RpcTarget.All);
            SceneManager.LoadScene("InGame");
        });

        PlayerTxt.text = "Nice to meet you " + GlobalValue.Unique_ID;

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnGUI()
    {
        string str = "my name : " + GlobalValue.Unique_ID + " : " + GlobalValue.IamTeemo + " ";

        for (int i = 0; i < GlobalValue.playerlist.Count; i++)
        {
            str += GlobalValue.playerlist[i].PlayerName + " : " + GlobalValue.playerlist[i].IamTeemo + "  ";
        }
        if (PhotonNetwork.CurrentRoom != null)
            str += "현재 인원 : " + PhotonNetwork.CurrentRoom.PlayerCount;
        GUI.Label(new Rect(10, 1, 1500, 60), str);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !SettingPanelActive)
        {
            GameObject SetPN = (GameObject)Instantiate(SettingPanel);
            SetPN.transform.SetParent(CanvasObj.transform, false);
            SettingPanelActive = true;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Chatmode = !Chatmode;

            if (Chatmode)
            {
                ChatIFd.gameObject.SetActive(true);
                ChatIFd.ActivateInputField();
            }
            else
            {
                ChatIFd.gameObject.SetActive(false);

                if (ChatIFd.text != "")
                    EnterChat();                    
            }
        }
    }

    void EnterChat()
    {
        string msg = "\n<color=#ffffff>[" + GlobalValue.Unique_ID + "] : " +
            ChatIFd.text + "</color>";
        PV.RPC("LogMsg", RpcTarget.All, msg);
        ChatIFd.text = "";
    }

    void ClearFunc()
    {
        GlobalValue.playerlist.Clear();
        GlobalValue.Host = false;
        GlobalValue.IamTeemo = true;
        RoomModeCustom = true;
        ChangeModeBtn.gameObject.SetActive(false);
        ShuffleBtn.gameObject.SetActive(false);
        StartBtn.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 완료");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료");
        PhotonNetwork.LocalPlayer.NickName = GlobalValue.Unique_ID;
        LoadingPanel.SetActive(false);
        myList.Clear();
    }

    void BuildRoomBtnFunc()
    {
        string roomName = GlobalValue.Unique_ID + "'s Room";

        RoomOptions rmOptions = new RoomOptions();
        rmOptions.IsOpen = true;
        rmOptions.IsVisible = true;
        rmOptions.MaxPlayers = 5;

        PhotonNetwork.CreateRoom(roomName, rmOptions, TypedLobby.Default);
        GlobalValue.Host = true;
        GlobalValue.IamTeemo = false;
        ChangeModeBtn.gameObject.SetActive(true);
        ShuffleBtn.gameObject.SetActive(true);
        StartBtn.gameObject.SetActive(true);

        PlayerList node = new PlayerList();
        node.PlayerName = GlobalValue.Unique_ID;
        node.IamTeemo = false;
        GlobalValue.playerlist.Add(node);
        GlobalValue.HostName = GlobalValue.Unique_ID;
    }

    public override void OnJoinedRoom()
    {        
        JoinRoomPanel.SetActive(false);
        JoinedRoomPanel.SetActive(true);
        LogMsg("\n<color=#00ff00>[시스템 메시지] : " + PhotonNetwork.CurrentRoom.Name + " 방에 입장했습니다.</color>");

        if (!GlobalValue.Host)
            PV.RPC("Introduce", RpcTarget.MasterClient, GlobalValue.Unique_ID);

        RoomSetting();
    }

    void RoomSetting()
    {
        Room currRoom = PhotonNetwork.CurrentRoom;
        RoomNameTxt.text = currRoom.Name;

        string WarwickTxt = "";
        string TeemoTxt = "";

        if (RoomModeCustom)
        {
            RoomModeTxt.text = "Custom";

            for (int i = 0; i < GlobalValue.playerlist.Count; i++)
            {
                if (GlobalValue.playerlist[i].IamTeemo)
                    TeemoTxt += "\n" + GlobalValue.playerlist[i].PlayerName;
                else
                    WarwickTxt += "\n" + GlobalValue.playerlist[i].PlayerName;

                if (GlobalValue.Unique_ID == GlobalValue.playerlist[i].PlayerName)
                    GlobalValue.IamTeemo = GlobalValue.playerlist[i].IamTeemo;
            }

            RoomInfoTxt.text = "- Warwick" + WarwickTxt + "\n- Teemo" + TeemoTxt;
        }
        else//미개발
        {
            RoomModeTxt.text = "Random";

            for(int i = 0; i < GlobalValue.playerlist.Count; i++)
            {
                TeemoTxt += "\n" + GlobalValue.playerlist[i].PlayerName;
            }

            RoomInfoTxt.text = TeemoTxt;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int ii = 0; ii < roomList.Count; ii++)
        {
            if (!roomList[ii].RemovedFromList)
            {
                if (!myList.Contains(roomList[ii])) myList.Add(roomList[ii]);
                else myList[myList.IndexOf(roomList[ii])] = roomList[ii];
            }
            else if (myList.IndexOf(roomList[ii]) != -1)
                myList.RemoveAt(myList.IndexOf(roomList[ii]));
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        for (int ii = 0; ii < myList.Count; ii++)
        {
            if (myList[ii].RemovedFromList)
                continue;

            GameObject room = (GameObject)Instantiate(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            RoomItem rmItem = room.GetComponent<RoomItem>();
            rmItem.DispRoomData(myList[ii].Name);
        }
    }

    public void OnClickedRoomItem(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        Debug.Log(roomName + "에 접속했습니다.");
        string[] str = roomName.Split("'");
        Debug.Log("Host Name is " + str[0]);
        GlobalValue.HostName = str[0];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        LogMsg("\n<color=#00ff00>[시스템 메시지] : " + newPlayer.NickName +
            " 님이 방에 입장했습니다.</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LogMsg("\n<color=#ff0000>[시스템 메시지] : " + otherPlayer.NickName +
            " 님이 방에서 퇴장했습니다.</color>");
        if (otherPlayer.NickName == GlobalValue.HostName)
        {
            PhotonNetwork.LeaveRoom();
            JoinedRoomPanel.SetActive(false);
            JoinRoomPanel.SetActive(true);
            ClearFunc();
        }
        else
        {
            for (int i = 0; i < GlobalValue.playerlist.Count; i++)
            {
                if (otherPlayer.NickName == GlobalValue.playerlist[i].PlayerName)
                {
                    GlobalValue.playerlist.RemoveAt(i);
                    RoomSetting();
                    break;
                }
            }
        }        
    }

    [PunRPC]
    void LogMsg(string msg)
    {
        ChatTxt.text = ChatTxt.text + msg;
    }

    [PunRPC]
    void Introduce(string PName)
    {
        PlayerList node = new PlayerList();
        node.PlayerName = PName;
        node.IamTeemo = true;

        GlobalValue.playerlist.Add(node);
        Debug.Log(PName + "님이 리스트에 추가되었습니다.");

        PV.RPC("Announce", RpcTarget.Others, Format(GlobalValue.playerlist));
        RoomSetting();
    }

    [PunRPC]
    void Announce(string pllf)
    {
        GlobalValue.playerlist = UnFormat(pllf);
        RoomSetting();
    }

    [PunRPC]
    void LoadGameScene()
    {
        SceneManager.LoadScene("InGame");
    }

    string Format(List<PlayerList> pll)
    {
        string pllf = "";

        for(int i = 0; i < pll.Count; i++)
        {
            pllf += pll[i].PlayerName + ',' + pll[i].IamTeemo.ToString() + '@';
        }
        Debug.Log("Format Successful " + pllf);

        return pllf;
    }

    List<PlayerList> UnFormat(string pllf)
    {
        List<PlayerList> pll = new List<PlayerList>();

        string pllfs = pllf.Substring(0, pllf.Length - 1);
        Debug.Log(pllfs);
        string[] pllf2 = pllfs.Split('@');

        for (int i = 0; i < pllf2.Length; i++)
        {
            string[] pllf3 = pllf2[i].Split(',');
            PlayerList node = new PlayerList();
            Debug.Log(pllf3[0]);
            node.PlayerName = pllf3[0];
            if (pllf3[1] == "True")
                node.IamTeemo = true;
            else
                node.IamTeemo = false;
            pll.Add(node);
            if (node.PlayerName == GlobalValue.Unique_ID)
                GlobalValue.IamTeemo = node.IamTeemo;
        }

        return pll;
    }
}
