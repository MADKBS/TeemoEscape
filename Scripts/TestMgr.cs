using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestMgr : MonoBehaviour
{
    public Button[] PlayerName;

    // Start is called before the first frame update
    void Start()
    {
        PlayerName[0].onClick.AddListener(() =>
        {
            GlobalValue.Unique_ID = "Player1";
            SceneManager.LoadScene("LobbyScene");
        });

        PlayerName[1].onClick.AddListener(() =>
        {
            GlobalValue.Unique_ID = "Player2";
            SceneManager.LoadScene("LobbyScene");
        });

        PlayerName[2].onClick.AddListener(() =>
        {
            GlobalValue.Unique_ID = "Player3";
            SceneManager.LoadScene("LobbyScene");
        });

        PlayerName[3].onClick.AddListener(() =>
        {
            GlobalValue.Unique_ID = "Player4";
            SceneManager.LoadScene("LobbyScene");
        });

        PlayerName[4].onClick.AddListener(() =>
        {
            GlobalValue.Unique_ID = "Player5";
            SceneManager.LoadScene("LobbyScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
