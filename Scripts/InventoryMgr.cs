using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMgr : MonoBehaviour
{
    public static InventoryMgr Inst = null;

    public Button ExitBtn;
    public Button ProductBtn;
    public Button ReturnBtn;
    public Button DestroyBtn;
    public GameObject PouchScr;
    public GameObject ProductScr;
    public GameObject InvenItem;
    [HideInInspector] public List<GameObject> ProductList = new List<GameObject>();
    public GameObject Camera;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ExitBtn.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });

        ReturnBtn.onClick.AddListener(() =>
        {
            InventoryReset();
        });

        DestroyBtn.onClick.AddListener(DestroyFunc);

        ProductBtn.onClick.AddListener(ProductFunc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InventoryReset()
    {
        ProductList.Clear();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("INVEN_ITEM"))
        {
            Destroy(obj);
        }

        if (InGameMgr.Inst.Branch != 0)
        {
            for(int i = 0; i < InGameMgr.Inst.Branch; i++)
            {
                GameObject Item = (GameObject)Instantiate(InvenItem);
                Item.transform.SetParent(PouchScr.transform, false);
                Item.GetComponent<InvenItem>().InitIcon("Branch");
            }
        }

        if (InGameMgr.Inst.Mushroom1 != 0)
        {
            for (int i = 0; i < InGameMgr.Inst.Mushroom1; i++)
            {
                GameObject Item = (GameObject)Instantiate(InvenItem);
                Item.transform.SetParent(PouchScr.transform, false);
                Item.GetComponent<InvenItem>().InitIcon("Mushroom1");
            }
        }

        if (InGameMgr.Inst.Mushroom2 != 0)
        {
            for (int i = 0; i < InGameMgr.Inst.Mushroom2; i++)
            {
                GameObject Item = (GameObject)Instantiate(InvenItem);
                Item.transform.SetParent(PouchScr.transform, false);
                Item.GetComponent<InvenItem>().InitIcon("Mushroom2");
            }
        }

        if (InGameMgr.Inst.Mushroom3 != 0)
        {
            for (int i = 0; i < InGameMgr.Inst.Mushroom3; i++)
            {
                GameObject Item = (GameObject)Instantiate(InvenItem);
                Item.transform.SetParent(PouchScr.transform, false);
                Item.GetComponent<InvenItem>().InitIcon("Mushroom3");
            }
        }

        if (InGameMgr.Inst.Mushroom4 != 0)
        {
            for (int i = 0; i < InGameMgr.Inst.Mushroom4; i++)
            {
                GameObject Item = (GameObject)Instantiate(InvenItem);
                Item.transform.SetParent(PouchScr.transform, false);
                Item.GetComponent<InvenItem>().InitIcon("Mushroom4");
            }
        }
    }

    void DestroyFunc()
    {
        Debug.Log(ProductList[0].name);

        for (int i = 0; i < ProductList.Count; i++)
        {
            switch (ProductList[i].GetComponent<InvenItem>().ItemName)
            {
                case "Branch":
                    InGameMgr.Inst.Branch--;
                    break;

                case "Mushroom1":
                    InGameMgr.Inst.Mushroom1--;
                    break;

                case "Mushroom2":
                    InGameMgr.Inst.Mushroom2--;
                    break;

                case "Mushroom3":
                    InGameMgr.Inst.Mushroom3--;
                    break;

                case "Mushroom4":
                    InGameMgr.Inst.Mushroom4--;
                    break;
            }
            Destroy(ProductList[i].gameObject);
        }
        ProductList.Clear();
        InventoryReset();
    }

    void ProductFunc()
    {
        int Branch = 0;
        int Mushroom1 = 0;
        int Mushroom2 = 0;
        int Mushroom3 = 0;
        int Mushroom4 = 0;

        for (int i = 0; i < ProductList.Count; i++)
        {
            switch (ProductList[i].GetComponent<InvenItem>().ItemName)
            {
                case "Branch":
                    Branch++;
                    break;

                case "Mushroom1":
                    Mushroom1++;
                    break;

                case "Mushroom2":
                    Mushroom2++;
                    break;

                case "Mushroom3":
                    Mushroom3++;
                    break;

                case "Mushroom4":
                    Mushroom4++;
                    break;
            }
        }

        if (Branch == 1 && Mushroom2 == 1 && ProductList.Count == 2)
        {
            InGameMgr.Inst.FlashLight = true;
            Camera.GetComponent<Light>().enabled = true;
        }
        else if (Branch == 1 && Mushroom1 == 1 && ProductList.Count == 2)
        {
            InGameMgr.Inst.DoorKey++;
        }
        else if (Mushroom3 == 1 && Mushroom4 == 1 && ProductList.Count == 2)
        {
            InGameMgr.Inst.Trap++;
        }
        else if (Branch == 1 && Mushroom4 == 2 && ProductList.Count == 3)
        {
            InGameMgr.Inst.PoisonedDart++;
        }
        else if (Branch == 1 && Mushroom1 == 5 && ProductList.Count == 6)
        {
            InGameMgr.Inst.ExitKey++;
        }
        else
        {
            return;
        }

        DestroyFunc();
        InGameMgr.Inst.RefreshSkillUI();
    }
}
