using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject Player = null;
    private float CamRotSpeed = 3.0f;
    public GameObject MainCamera;
    Light LightSetting = null;

    public void InitCamera(GameObject _Player)
    {
        Player = _Player;
    }

    // Start is called before the first frame update
    void Start()
    {
        LightSetting = MainCamera.GetComponent<Light>();
        if (GlobalValue.IamTeemo == false)
        {
            LightSetting.enabled = true;
            LightSetting.color = Color.red;
        }            
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Player != null)
        {
            Vector3 PlayerAngle = Player.transform.rotation.eulerAngles;
            Vector3 CamAngle = this.transform.rotation.eulerAngles;
            float X_Change = CamAngle.x - Input.GetAxis("Mouse Y") * CamRotSpeed;
            float Y_Change = CamAngle.y + Input.GetAxis("Mouse X") * CamRotSpeed;

            if (X_Change < 180f)
                X_Change = Mathf.Clamp(X_Change, -1f, 35f);
            else
                X_Change = Mathf.Clamp(X_Change, 310f, 361f);

            transform.position = Player.transform.position;

            if (Input.GetKey(KeyCode.LeftAlt))
                transform.rotation = Quaternion.Euler(X_Change, Y_Change, PlayerAngle.z);
            else
                transform.rotation = Quaternion.Euler(X_Change, PlayerAngle.y, PlayerAngle.z);
        }
    }
}
