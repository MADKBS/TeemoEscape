using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeemoTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!transform.GetComponentInParent<TeemoController>().PV.IsMine)
            this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ITEM")
            InGameMgr.Inst.TriggeredItem.Add(other.gameObject);
        else if (other.tag == "JAILDOOR")
            InGameMgr.Inst.DoorTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (InGameMgr.Inst.TriggeredItem.Contains(other.gameObject))
            InGameMgr.Inst.TriggeredItem.Remove(other.gameObject);
        else if (other.tag == "JAILDOOR")
            InGameMgr.Inst.DoorTriggered = false;
    }
}
