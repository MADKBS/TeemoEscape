using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InGameMgr : MonoBehaviourPunCallbacks
{
    public static InGameMgr Inst = null;
    private PhotonView PV;
    [HideInInspector]public int heroCount = 0;

    [HideInInspector] public Vector3[] SpawnPoint = new Vector3[5];

    public Text TimeTxt;
    public GameObject LoadingPanel;
    int LoadCount = 0;

    public GameObject InventoryPanel;
    public GameObject Camera;

    public GameObject SkillPanel;
    public Text Skill1Txt;
    public Text Skill2Txt;
    public Text Skill3Txt;
    public Text Skill4Txt;
    public Text Skill5Txt;

    public GameObject[] PlayerObj;
    public Text[] PlayerTxt;
    public Text[] PlayerDeadTxt;
    public Image PoisonedPanel;

    public Text AttackTxt;

    public GameObject GameOverPanel;
    public Text ResultTxt;
    public Button BackToLobbyBtn;

    [HideInInspector] public List<string> TeemoList = new List<string>();
    float timer = 0.0f;
    float globaltimer = 900.0f;
    bool Poisoned = false;
    [HideInInspector] public bool SettingPanelActive = false;
    public GameObject SettingPanel;
    public GameObject CanvasObj;

    [HideInInspector] public bool ChannelingAble = false;
    [HideInInspector] public GameObject TargetItem = null;
    [HideInInspector] public bool Channeling = false;
    [HideInInspector] public bool DoorChannelingAble = false;
    [HideInInspector] public bool ExitChannlingAble = false;
    [HideInInspector] public bool isExit = false;

    [HideInInspector] public int Mushroom1 = 2;
    [HideInInspector] public int Mushroom2 = 2;
    [HideInInspector] public int Mushroom3 = 2;
    [HideInInspector] public int Mushroom4 = 2;
    [HideInInspector] public int Branch = 2;
    [HideInInspector] public bool FlashLight = false;
    [HideInInspector] public int DoorKey = 0;
    [HideInInspector] public int Trap = 0;
    [HideInInspector] public int PoisonedDart = 1;
    [HideInInspector] public int ExitKey = 0;

    [HideInInspector] public List<GameObject> TriggeredItem = new List<GameObject>();

    [HideInInspector] public bool DoorTriggered = false;
    [HideInInspector] public bool ExitTriggered = false;

    [HideInInspector] public int TeemoCount = 0;

    [HideInInspector] public bool UIMode = false;

    private void Awake()
    {
        Inst = this;
        PV = GetComponent<PhotonView>();

        Application.targetFrameRate = 60;   //실행 프레임 속도 60프레임으로 고정 시키기.. 코드
        QualitySettings.vSyncCount = 0;     //모니터 주사율(플레임율)이 다른 컴퓨터일 경우 캐릭터 조작시 빠르게 움직일 수 있다.

        SpawnPoint[0] = new Vector3(0, 4.6f, 0);
        //SpawnPoint[1] = new Vector3(200, 4.6f, 150);
        SpawnPoint[1] = new Vector3(400, 4.6f, -400);
        //SpawnPoint[2] = new Vector3(10, 4.6f, -36);
        SpawnPoint[2] = new Vector3(400, 4.6f, 400);
        SpawnPoint[3] = new Vector3(-390, 4.6f, -370);
        SpawnPoint[4] = new Vector3(-390, 4.6f, 370);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        if (!GlobalValue.Host)
            PV.RPC("LoadedScene", RpcTarget.MasterClient);

        if (!GlobalValue.IamTeemo)
            SkillPanel.SetActive(false);

        SetPlayerPanel();

        Mushroom1 = 0;
        Mushroom2 = 0;
        Mushroom3 = 0;
        Mushroom4 = 0;
        Branch = 0;

        RefreshSkillUI();

        BackToLobbyBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LobbyScene");
            PhotonNetwork.LeaveRoom();
        });

        TeemoCount = GlobalValue.playerlist.Count - 1;
    }

    private void OnGUI()
    {
        string str = "my name : " + GlobalValue.Unique_ID + "  ";

        for (int i = 0; i < GlobalValue.playerlist.Count; i++)
        {
            str += GlobalValue.playerlist[i].PlayerName + " : " + GlobalValue.playerlist[i].IamTeemo + "  ";
        }
        if (PhotonNetwork.CurrentRoom != null)
            str += "현재 인원 : " + PhotonNetwork.CurrentRoom.PlayerCount;
        str += "  생성된 인스턴스 수 : " + heroCount;

        GUI.Label(new Rect(10, 1, 1500, 60), str);
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIMode)
            Cursor.visible = false;
        else
            Cursor.visible = true;

        if (Input.GetKeyDown(KeyCode.Escape) && !SettingPanelActive)
        {
            GameObject SetPN = (GameObject)Instantiate(SettingPanel);
            SetPN.transform.SetParent(CanvasObj.transform, false);
            SettingPanelActive = true;
        }

        globaltimer -= Time.deltaTime;
        int minute = ((int)globaltimer) / 60;
        int second = ((int)globaltimer) % 60;
        TimeTxt.text = minute.ToString() + ":" + second.ToString();

        if (globaltimer <= 0)
            WarwickWinRPC();

        if (GlobalValue.Host && LoadCount == GlobalValue.playerlist.Count - 1)
        {
            LoadCount++;
            PV.RPC("CreateHero", RpcTarget.All);
        }       

        if (GlobalValue.IamTeemo)
        {
            if (TeemoController.Inst != null)
            {
                if (TeemoController.Inst.isDead)
                    InventoryPanel.SetActive(false);

                if (Input.GetKeyDown(KeyCode.I) && !TeemoController.Inst.isDead)
                {
                    InventoryPanel.SetActive(!InventoryPanel.activeSelf);
                    InventoryMgr.Inst.InventoryReset();
                    if (InventoryPanel.activeSelf)
                        UIMode = true;
                    else
                        UIMode = false;
                }
            }
        }

        if (Poisoned)
        {
            timer -= Time.deltaTime;
            PoisonedPanel.color = new Color32(255, 255, 255, (byte)(255 * (timer / 5.0f)));
            if (timer <= 0)
            {
                Poisoned = false;
                PoisonedPanel.gameObject.SetActive(false);
            }
        }
    }

    void SetPlayerPanel()
    {
        int a = 1;

        for(int i = 0; i < GlobalValue.playerlist.Count; i++)
        {
            if (GlobalValue.playerlist[i].IamTeemo)
            {
                PlayerObj[a].SetActive(true);
                PlayerTxt[a].text = GlobalValue.playerlist[i].PlayerName;
                a++;
                TeemoList.Add(GlobalValue.playerlist[i].PlayerName);
            }
            else
            {
                PlayerObj[0].SetActive(true);
                PlayerTxt[0].text = GlobalValue.playerlist[i].PlayerName;
            }
        }
    }

    public void PoisonedDartFunc()
    {
        PV.RPC("IamPoisoned", RpcTarget.All);
    }

    public void RefreshSkillUI()
    {
        if (FlashLight)
            Skill1Txt.text = "on";
        else
            Skill1Txt.text = "off";

        Skill2Txt.text = "X" + DoorKey.ToString();
        Skill3Txt.text = "X" + Trap.ToString();
        Skill4Txt.text = "X" + PoisonedDart.ToString();
        Skill5Txt.text = "X" + ExitKey.ToString();
    }

    [PunRPC]
    void CreateHero()
    {
        int spawnnum = 1;

        for(int i = 0; i < GlobalValue.playerlist.Count; i++)
        {
            if (GlobalValue.Unique_ID == GlobalValue.playerlist[i].PlayerName)
            {
                if (!GlobalValue.IamTeemo)
                    PhotonNetwork.Instantiate("Warwick", SpawnPoint[0], Quaternion.identity, 0);
                else
                    PhotonNetwork.Instantiate("Teemo", SpawnPoint[spawnnum], Quaternion.identity, 0);
            }
            else if (GlobalValue.playerlist[i].IamTeemo)
                spawnnum++;
        }

        LoadingPanel.SetActive(false);
    }

    [PunRPC]
    void LoadedScene()
    {
        LoadCount++;
    }

    [PunRPC]
    void IamPoisoned()
    {
        if (!GlobalValue.IamTeemo)
        {
            PoisonedPanel.gameObject.SetActive(true);
            PoisonedPanel.color = new Color32(255, 255, 255, 255);
            Poisoned = true;
            timer = 5;
            WarwickController.Inst.Stunned = true;
            WarwickController.Inst.timer = 5.0f;
        }
    }

    [PunRPC]
    void ExittedTeemoCount()
    {
        if (!GlobalValue.IamTeemo)
        {
            TeemoCount--;
            if (TeemoCount == 0 && !isExit)
                GameOverFunc(false);
        }
    }

    [PunRPC]
    void WarwickWinRPC()
    {
        WarwickWin();
    }

    public void DeliverMeExit()
    {
        PV.RPC("ExittedTeemoCount", RpcTarget.All);
    }

    public void AllJailFunc()
    {
        PV.RPC("WarwickWinRPC", RpcTarget.All);
    }

    public void GameOverFunc(bool isWin)
    {
        if (isWin)
        {
            GameOverPanel.SetActive(true);
            ResultTxt.text = "You Win";
        }
        else
        {
            GameOverPanel.SetActive(true);
            ResultTxt.text = "You Lose";
        }
    }

    public void WarwickWin()
    {
        if (GlobalValue.IamTeemo)
        {
            GameOverPanel.SetActive(true);
            ResultTxt.text = "You Lose";
        }
        else
        {
            GameOverPanel.SetActive(true);
            ResultTxt.text = "You Win";
        }
    }
}
