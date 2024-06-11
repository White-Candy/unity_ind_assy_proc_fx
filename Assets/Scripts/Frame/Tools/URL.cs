using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URL
{
    public static string ROOT_API
    {
        get { return GlobalData.Url + "/prod-api/application/u3dApp/"; }
    }

    /// <summary>
    /// µÇÂ¼½Ó¿Ú
    /// </summary>
    public static string URL_LOGIN
    {
        get { return ROOT_API + "login"; }
    }
}
