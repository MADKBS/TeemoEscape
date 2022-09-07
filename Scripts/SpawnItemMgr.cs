using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnItemMgr : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        if (GlobalValue.HostName == GlobalValue.Unique_ID)
        {
            for (int i = 0; i < 80; i++)
            {
                PhotonNetwork.Instantiate("Branch", new Vector3(Random.Range(-460f, 460f), 0, Random.Range(-460f, 460f)),
                    Quaternion.identity, 0);
                if (i < 40)
                {
                    PhotonNetwork.Instantiate("Mushroom1", new Vector3(Random.Range(-460f, 460f), 0, Random.Range(-460f, 460f)),
                    Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Mushroom2", new Vector3(Random.Range(-460f, 460f), 0, Random.Range(-460f, 460f)),
                    Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Mushroom3", new Vector3(Random.Range(-460f, 460f), 0, Random.Range(-460f, 460f)),
                    Quaternion.identity, 0);
                    PhotonNetwork.Instantiate("Mushroom4", new Vector3(Random.Range(-460f, 460f), 0, Random.Range(-460f, 460f)),
                    Quaternion.identity, 0);
                }
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
