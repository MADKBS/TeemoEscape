using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

enum TitleCondition
{
    notready,
    startable,
    changing,
    login,
    register
}

public class TitleManager : MonoBehaviour
{
    public Text GoMainUITxt;
    public Text ErrorTxt;

    public GameObject MainUIObj;
    RectTransform MainUIRect;

    [Header("-- Login Panel --")]
    public GameObject LoginObj;
    public InputField Login_IDIFd;
    public InputField Login_PWIFd;
    public Button RegisterBtn;
    public Button LoginBtn;

    [Header("-- Register Panel --")]
    public GameObject RegisterObj;
    public InputField Reg_IDIFd;
    public InputField Reg_PWIFd;
    public InputField Reg_PWConIFd;
    public Button BackLoginBtn;
    public Button RegisterConfirmBtn;

    float delta1 = 0;
    float delta2 = 0;
    float delta3 = 0;
    TitleCondition titlecond;
    bool isLoginPanel;
    bool expand;
    float PanelShrinkingSpd;
    bool isErrorMsgOn;

    // Start is called before the first frame update
    void Start()
    {
        titlecond = TitleCondition.notready;
        isLoginPanel = true;
        expand = true;
        MainUIRect = MainUIObj.GetComponent<RectTransform>();
        PanelShrinkingSpd = 1500;
        isErrorMsgOn = false;

        RegisterBtn.onClick.AddListener(() =>
        {
            titlecond = TitleCondition.changing;
            isLoginPanel = false;
            LoginObj.SetActive(false);
            Login_IDIFd.text = "";
            Login_PWIFd.text = "";
        });

        BackLoginBtn.onClick.AddListener(() =>
        {
            titlecond = TitleCondition.changing;
            isLoginPanel = true;
            RegisterObj.SetActive(false);
            Reg_IDIFd.text = "";
            Reg_PWIFd.text = "";
            Reg_PWConIFd.text = "";
        });

        LoginBtn.onClick.AddListener(LoginBtnFunc);

        RegisterConfirmBtn.onClick.AddListener(RegisterConfirmBtnFunc);
    }

    // Update is called once per frame
    void Update()
    {
        delta1 += Time.deltaTime;

        if (delta1 >= 4.0f && titlecond == TitleCondition.notready)
            titlecond = TitleCondition.startable;

        if (titlecond == TitleCondition.startable)
        {
            if ((int)delta1 % 2 == 1)
                GoMainUITxt.gameObject.SetActive(true);
            else
                GoMainUITxt.gameObject.SetActive(false);

            if (Input.GetMouseButtonDown(0))
            {
                GoMainUITxt.gameObject.SetActive(false);
                MainUIObj.SetActive(true);
                titlecond = TitleCondition.changing;
            }
        }
        else if (titlecond == TitleCondition.changing)
        {
            delta2 += Time.deltaTime;

            if (!expand)
            {
                MainUIRect.sizeDelta = new Vector2(600, 400 - delta2 * PanelShrinkingSpd);
                if (delta2 * PanelShrinkingSpd >= 400)
                {
                    expand = !expand;
                    MainUIRect.sizeDelta = new Vector2(600, 0);
                    delta2 = 0;
                    if (isLoginPanel)
                        LoginObj.SetActive(true);
                    else
                        RegisterObj.SetActive(true);
                }
            }
            else if (expand)
            {
                MainUIRect.sizeDelta = new Vector2(600, delta2 * PanelShrinkingSpd);
                if (delta2 * PanelShrinkingSpd >= 400)
                {
                    if (isLoginPanel)
                        titlecond = TitleCondition.login;
                    else
                        titlecond = TitleCondition.register;
                    expand = !expand;
                    MainUIRect.sizeDelta = new Vector2(600, 400);
                    delta2 = 0;                    
                }
            }
        }

        if (Login_IDIFd.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            Login_PWIFd.Select();
        }
        else if (Reg_IDIFd.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            Reg_PWIFd.Select();
        }
        else if(Reg_PWIFd.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            Reg_PWConIFd.Select();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (titlecond == TitleCondition.login)
                LoginBtnFunc();
            else if (titlecond == TitleCondition.register)
                RegisterConfirmBtnFunc();
        }

        if (isErrorMsgOn)
        {
            delta3 += Time.deltaTime;
            if (delta3 >= 3.0f)
            {
                isErrorMsgOn = false;
                ErrorTxt.gameObject.SetActive(false);
            }
        }
    }

    void ErrorMsgFunc(string Msg)
    {
        delta3 = 0;
        isErrorMsgOn = true;
        ErrorTxt.text = Msg;
        ErrorTxt.gameObject.SetActive(true);
    }

    void LoginBtnFunc()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = Login_IDIFd.text.Trim() + "@aaa.aaa",
            Password = Login_PWIFd.text.Trim()
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    void RegisterConfirmBtnFunc()
    {
        if (Reg_PWIFd.text.Trim() != Reg_PWConIFd.text.Trim())
        {
            ErrorMsgFunc("비밀번호가 서로 다릅니다.");
            Reg_PWIFd.text = "";
            Reg_PWConIFd.text = "";
        }
        else
        {
            var request = new RegisterPlayFabUserRequest
            {
                Email = Reg_IDIFd.text.Trim() + "@aaa.aaa",
                Password = Reg_PWIFd.text.Trim(),
                Username = Reg_IDIFd.text.Trim()
            };
            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        }
    }

    void OnLoginSuccess(LoginResult result)
    {
        GlobalValue.Unique_ID = Login_IDIFd.text.Trim();
        SceneManager.LoadScene("LobbyScene");
    }

    void OnLoginFailure(PlayFabError error)
    {
        ErrorMsgFunc("아이디와 비밀번호를 확인해주세요.");
        Debug.Log(error);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        ErrorMsgFunc("회원가입에 성공하였습니다.");
        Debug.Log(result);
    }

    void OnRegisterFailure(PlayFabError error)
    {
        ErrorMsgFunc("회원가입에 실패하였습니다.");
        Debug.Log(error);
    }
}
