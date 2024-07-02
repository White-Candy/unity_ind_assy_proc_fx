using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BaseAction
{
    public CancellationTokenSource m_Token;

    public CancellationTokenSource m_panelToken;

    public virtual async UniTask AsyncShow(string name) 
    {
        await UniTask.Yield();
    }

    public virtual void UpdateData() { }

    public virtual void Exit(Action callback) { callback(); }
}
