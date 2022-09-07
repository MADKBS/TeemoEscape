using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenItem : MonoBehaviour
{
    public Texture2D[] IconImg;
    [HideInInspector] public string ItemName;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public void InitIcon(string ITName)
    {
        switch (ITName)
        {
            case "Branch":
                ItemName = ITName;
                this.GetComponent<RawImage>().texture = IconImg[0];
                break;

            case "Mushroom1":
                ItemName = ITName;
                this.GetComponent<RawImage>().texture = IconImg[1];
                break;

            case "Mushroom2":
                ItemName = ITName;
                this.GetComponent<RawImage>().texture = IconImg[2];
                break;

            case "Mushroom3":
                ItemName = ITName;
                this.GetComponent<RawImage>().texture = IconImg[3];
                break;

            case "Mushroom4":
                ItemName = ITName;
                this.GetComponent<RawImage>().texture = IconImg[4];
                break;
        }
    }

    void OnClicked()
    {
        if (InventoryMgr.Inst.ProductList.Count < 6)
        {
            this.transform.SetParent(InventoryMgr.Inst.ProductScr.transform, false);
            InventoryMgr.Inst.ProductList.Add(this.gameObject);
        }        
    }
}
