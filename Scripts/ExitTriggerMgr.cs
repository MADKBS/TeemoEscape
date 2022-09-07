using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTriggerMgr : MonoBehaviour
{
    GameObject TriggeredTeemo = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TEEMO")
        {
            if (other.gameObject.GetComponent<TeemoController>().PV.IsMine)
            {
                InGameMgr.Inst.ExitTriggered = true;
                TriggeredTeemo = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == TriggeredTeemo)
            InGameMgr.Inst.ExitTriggered = false;
    }
}
