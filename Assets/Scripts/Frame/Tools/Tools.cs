using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static bool CheckMessageSuccess(int code)
    {
        return code == GlobalData.SuccessCode;
    }
}
