
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MinMap : BasePanel
{
    [SerializeField] private RawImage m_Image; // ��ͼ

    [SerializeField] private RectTransform miniSpot;//��ͼ��Ŀ��
    [SerializeField] private Transform player;//ʵ��Ŀ��
    [SerializeField] private Vector3 origin;//ƫ������
    public bool canshow = false;

    public static MinMap _instance;

    public override void Awake()
    {
        base.Awake();
        canshow = false;

        _instance = this;
        Active(false);
    }

    void Start()
    {
        Addressables.LoadAssetAsync<Texture2D>("map").Completed += (obj) => 
        {
            Texture2D tex = obj.Result;
            m_Image.texture = tex;
            m_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
        };
    }

    void Update()
    {
        if (GlobalData.isLoadModel && m_Visible)
        {
            player = CameraControl.target.transform;
            miniSpot.anchoredPosition = new Vector2(origin.x - player.position.x, origin.z - player.position.z);

            Vector3 ro = player.rotation.eulerAngles;
            Vector3 ro1 = new Vector3(0, 0, (ro.y - 180) * -1);
            miniSpot.rotation = Quaternion.Euler(ro1);
        }
    }
}
