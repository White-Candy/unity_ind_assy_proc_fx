using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class PictureAction : BaseAction
{
    private bool m_Init = false;

    private PicturePanel m_Panel;

    public PictureAction()
    {
        m_Token = new CancellationTokenSource();
        m_panelToken = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        if (!m_Init)
        {
            List<Sprite> sprites = await LoadPictureAsync(name);

            m_Panel = UIConsole.FindAssetPanel<PicturePanel>();
            m_Panel.Init(sprites);
            m_Init = true;
        }

        await UniTask.WaitUntil(() => m_Init == true, PlayerLoopTiming.Update, m_Token.Token);
        //m_Panel.transform.SetAsFirstSibling();
        m_Panel.Active(true);

        try
        { 
            await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false);
        }
        catch 
        {

        }
    }

    private async UniTask<List<Sprite>> LoadPictureAsync(string name)
    {
        List<string> paths = new List<string>();
#if UNITY_STANDALONE_WIN
        var paths = NetworkManager._Instance.DownLoadAaset(name, "png");
        await NetHelper.RsCkAndDLReq(paths, name);
        paths = NetworkManager._Instance.DownLoadAaset(name, "png");
#elif UNITY_WEBGL
            string configPath = FPath.AssetRootPath + GlobalData.ProjGroupName + Tools.GetModulePath(name);
            Debug.Log("tupian an config path: "  + configPath);
            paths = await FileHelper.DownLoadConfig(name, configPath + "\\Config.txt", ".png");
#endif
        List<Sprite> sprites = new List<Sprite>();

        if (paths.Count == 0)
            UITools.ShowMessage("当前模块没有图片资源");

        AsyncResult result = await AssetConsole.Instance.LoadTexObject(paths.ToArray());

        await UniTask.WaitUntil(() => result.isLoad == true);

        foreach (var spo in result.m_Assets)
        {
            sprites.Add(Tools.SpriteConvert((Texture2D)spo.Value));
        }
        return sprites;
    }

    public override void Exit(Action callback)
    {
        base.Exit(callback);
        //Debug.Log("Exit");

        m_Token.Cancel();
        m_Token.Dispose();
        m_Token = new CancellationTokenSource();

        //m_panelToken.Cancel();
        //m_panelToken.Dispose();
        //m_panelToken = new CancellationTokenSource();

        m_Panel.Active(false);
    }  
}
