using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class UITools
{
    public static void ShowMessage(string mess, float duration = 3.0f)
    {
        MessagePanel panel = FindAssetPanel<MessagePanel>();
        //panel.Show(mess, duration);
    }

    public static T FindAssetPanel<T>() where T : BasePanel
    {
        T t = UIConsole.Instance.FindPanel<T>();
        if (t == null)
        {
            string path = "Prefabs/UI/Panel/" + typeof(T).ToString();
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(path));
            if (go != null)
            {
                t = go.GetComponent<T>();
                go.name = typeof(T).Name;
                if (t is IGlobalPanel)
                {
                    // TODO..
                }
                else
                {
                    go.transform.SetParent(GameObject.Find("Canvas/").transform);
                }
                go.transform.localScale = Vector3.one;
                RectTransform rect = go.transform as RectTransform;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
            }
        }
        return t;
    }
}

