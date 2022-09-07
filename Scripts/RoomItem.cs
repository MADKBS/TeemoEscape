using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text RoomName;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            FindObjectOfType<LobbyMgr>().OnClickedRoomItem(RoomName.text);
        });
    }

    public void DispRoomData(string RN)
    {
        RoomName.text = RN;
    }
}
