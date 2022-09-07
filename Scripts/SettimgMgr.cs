using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettimgMgr : MonoBehaviour
{
    public Slider VolumeSlider;
    public Text VolumeGageTxt;
    public Button GoLobbyBtn;
    public Button ExitBtn;
    public Button ExitProgramBtn;

    // Start is called before the first frame update
    void Start()
    {
        VolumeSlider.value = GlobalValue.Volume;
        if (InGameMgr.Inst != null)
            InGameMgr.Inst.UIMode = true;

        ExitBtn.onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
        });

        GoLobbyBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LobbyScene");
        });

        ExitProgramBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    // Update is called once per frame
    void Update()
    {
        GlobalValue.Volume = VolumeSlider.value;
        VolumeGageTxt.text = ((int)(VolumeSlider.value * 100)).ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitBtnFunc();
    }

    void ExitBtnFunc()
    {
        Destroy(this.gameObject);
        if (InGameMgr.Inst != null)
            InGameMgr.Inst.SettingPanelActive = false;
        else if (LobbyMgr.Inst != null)
            LobbyMgr.Inst.SettingPanelActive = false;

        if (InGameMgr.Inst != null && !InGameMgr.Inst.InventoryPanel.activeSelf)
            InGameMgr.Inst.UIMode = false;            
    }
}
