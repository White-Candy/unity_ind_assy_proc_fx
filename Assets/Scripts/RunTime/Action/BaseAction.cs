using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BaseAction
{
    public CancellationTokenSource m_Token;

    public virtual async UniTask AsyncShow(string name) { await UniTask.Yield(PlayerLoopTiming.Update); }

    public virtual void UpdateData() { }

    public virtual void Exit() { }
}
