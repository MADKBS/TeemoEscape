using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : G_Singleton<SoundManager>
{
    [HideInInspector] public AudioSource audioSrc = null;
    Dictionary<string, AudioClip> audioClipList = new Dictionary<string, AudioClip>();
    [HideInInspector] public bool soundOnOff = true;

    int effSdCount = 10;
    int iSdCount = 0;
    List<GameObject> sdObjList = new List<GameObject>();
    List<AudioSource> sdSrcList = new List<AudioSource>();
    float MaxDistance = 150f;

    protected override void Init()
    {
        base.Init();

        LoadChildGameObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioClip clip = null;
        object[] temp = Resources.LoadAll("Sound");
        for (int i = 0; i < temp.Length; i++)
        {
            clip = temp[i] as AudioClip;
            if (audioClipList.ContainsKey(clip.name))
                continue;
            audioClipList.Add(clip.name, clip);
        }
    }

    void LoadChildGameObj()
    {
        if (this == null)
            return;

        audioSrc = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < effSdCount; i++)
        {
            GameObject newSdObj = new GameObject();
            newSdObj.transform.SetParent(transform);
            newSdObj.transform.localPosition = Vector3.zero;
            AudioSource audioSource = newSdObj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            newSdObj.name = "SoundEffObj";

            sdSrcList.Add(audioSource);
            sdObjList.Add(newSdObj);
        }
    }

    public void PlayEffSound(string fileName, Vector3 OtherPos)
    {
        if (!soundOnOff)
            return;

        float DistVol = GetEffVolume(OtherPos);
        if (DistVol == 0)
            return;

        if (!audioClipList.ContainsKey(fileName))
            audioClipList.Add(fileName, Resources.Load("Sound/" + fileName) as AudioClip);

        if (audioClipList[fileName] != null && sdSrcList[iSdCount] != null)
        {
            sdSrcList[iSdCount].clip = audioClipList[fileName];
            sdSrcList[iSdCount].loop = false;
            sdSrcList[iSdCount].volume = DistVol * GlobalValue.Volume / 1f;
            sdSrcList[iSdCount].Play();

            iSdCount++;
            if (effSdCount <= iSdCount)
                iSdCount = 0;
        }
    }

    public float GetEffVolume(Vector3 OtherPos)
    {
        if (OtherPos == Vector3.zero)
            return 1f;

        float volume = 1f;

        Vector3 vec = Camera.main.gameObject.transform.position;
        float Distance = (vec - OtherPos).magnitude;

        if (Distance < MaxDistance)
            return volume * (1 - Distance / MaxDistance);
        else
            return 0;
    }
}
