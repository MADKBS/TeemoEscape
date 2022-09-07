using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailTriggerMgr : MonoBehaviour
{
    int Count = 0;

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
            Count++;
            if (Count == InGameMgr.Inst.TeemoCount)
            {
                InGameMgr.Inst.AllJailFunc();
            }
        }            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TEEMO")
            Count--;
    }
}
