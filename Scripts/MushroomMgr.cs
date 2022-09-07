using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[System.Serializable]
public class MushroomAnim
{
    public AnimationClip idle;
    public AnimationClip death;
}

public class MushroomMgr : MonoBehaviourPunCallbacks
{
    [HideInInspector] public PhotonView PV = null;
    string ItemName = null;
    public Canvas CanvasObj;
    public Text ItemInfoTxt;
    public GameObject LoadingObj;
    public Image LoadingImg;
    GameObject Camera;
    public MushroomAnim anim;
    [HideInInspector] public Animation _animation;

    float timer = 0.0f;
    float gaintime = 0.0f;
    bool Destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        ItemName = this.gameObject.name;
        SettingItem(ItemName);
        if (ItemName != "Branch")
            _animation = GetComponentInChildren<Animation>();
        Camera = InGameMgr.Inst.Camera;
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameMgr.Inst != null)
        {
            if (InGameMgr.Inst.TargetItem == null)
            {
                ItemInfoTxt.gameObject.SetActive(false);
                LoadingObj.SetActive(false);
            }
            else if (InGameMgr.Inst.TargetItem == this.gameObject)
            {
                CanvasObj.transform.LookAt(Camera.transform);
                ItemInfoTxt.gameObject.SetActive(true);
                if (InGameMgr.Inst.Channeling)
                {
                    ItemInfoTxt.gameObject.SetActive(false);
                    LoadingObj.SetActive(true);
                    timer += Time.deltaTime;
                    LoadingImg.fillAmount = timer / gaintime;
                    if (timer >= gaintime && !Destroyed)
                    {
                        AddItem(ItemName);
                        Destroyed = true;
                        if (ItemName != "Branch(Clone)")
                        {
                            _animation.CrossFade(anim.death.name, 0.1f);
                            CanvasObj.gameObject.SetActive(false);
                            PV.RPC("DestroyObj", RpcTarget.All, 1.0f);
                        }
                        else
                            PV.RPC("DestroyObj", RpcTarget.All, 0.0f);
                    }
                }
                else
                {
                    ItemInfoTxt.gameObject.SetActive(true);
                    LoadingObj.SetActive(false);
                }
            }
        }        
    }

    void SettingItem(string ITName)
    {
        switch (ITName)
        {
            case "Branch(Clone)":
                gaintime = 2.0f;
                break;

            case "Mushroom1(Clone)":
                gaintime = 15.0f;
                break;

            case "Mushroom2(Clone)":
                gaintime = 5.0f;
                break;

            case "Mushroom3(Clone)":
                gaintime = 5.0f;
                break;

            case "Mushroom4(Clone)":
                gaintime = 5.0f;
                break;
        }
    }

    void AddItem(string ITName)
    {
        switch (ITName)
        {
            case "Branch(Clone)":
                InGameMgr.Inst.Branch++;
                break;

            case "Mushroom1(Clone)":
                InGameMgr.Inst.Mushroom1++;
                break;

            case "Mushroom2(Clone)":
                InGameMgr.Inst.Mushroom2++;
                break;

            case "Mushroom3(Clone)":
                InGameMgr.Inst.Mushroom3++;
                break;

            case "Mushroom4(Clone)":
                InGameMgr.Inst.Mushroom4++;
                break;
        }
    }

    [PunRPC]
    void DestroyObj(float setTimer)
    {
        Destroy(this.gameObject, setTimer);
    }
}
