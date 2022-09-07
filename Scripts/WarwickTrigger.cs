using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarwickTrigger : MonoBehaviour
{
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
        if (other.tag == "TEEMO")
            WarwickController.Inst.TriggeredTeemo.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (WarwickController.Inst.TriggeredTeemo.Contains(other.gameObject))
            WarwickController.Inst.TriggeredTeemo.Remove(other.gameObject);
    }
}
