using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitMgr : MonoBehaviour
{
    public static ExitMgr Inst = null;
    public Text ExitInfoTxt;
    public GameObject LoadingObj;
    public Image LoadingImg;
    [HideInInspector] public float timer;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameMgr.Inst.ExitChannlingAble)
        {
            ExitInfoTxt.gameObject.SetActive(true);
            if (InGameMgr.Inst.Channeling)
            {
                ExitInfoTxt.gameObject.SetActive(false);
                LoadingObj.SetActive(true);
                timer += Time.deltaTime;
                LoadingImg.fillAmount = timer / 5.0f;
                if (timer >= 5.0f)
                {
                    InGameMgr.Inst.isExit = true;
                    InGameMgr.Inst.DeliverMeExit();
                    InGameMgr.Inst.GameOverFunc(true);
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
            ExitInfoTxt.gameObject.SetActive(false);
            LoadingObj.SetActive(false);
        }
    }
}
