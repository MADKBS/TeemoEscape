using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class JailDoorMgr : MonoBehaviour
{
    PhotonView PV;
    public static JailDoorMgr Inst = null;
    public Text DoorInfoTxt;
    public GameObject LoadingObj;
    public Image LoadingImg;
    [HideInInspector] public float timer;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameMgr.Inst.DoorChannelingAble)
        {
            DoorInfoTxt.gameObject.SetActive(true);
            if (InGameMgr.Inst.Channeling)
            {
                DoorInfoTxt.gameObject.SetActive(false);
                LoadingObj.SetActive(true);
                timer += Time.deltaTime;
                LoadingImg.fillAmount = timer / 5.0f;
                if (timer >= 5.0f)
                {
                    DoorSystem(true);
                    timer = 0;
                    InGameMgr.Inst.DoorKey--;
                    InGameMgr.Inst.RefreshSkillUI();
                }
            }
            else
            {
                LoadingObj.SetActive(false);
                timer = 0;
            }
        }
        else
        {
            DoorInfoTxt.gameObject.SetActive(false);
            LoadingObj.SetActive(false);
        }        
    }

    public void DoorSystem(bool open)
    {
        PV.RPC("DoorMove", RpcTarget.All, open);
    }

    [PunRPC]
    void DoorMove(bool open)
    {
        if (open)
            this.gameObject.transform.position = new Vector3(0, 8, 0);
        else
            this.gameObject.transform.position = new Vector3(0, 0, 0);
    }
}
