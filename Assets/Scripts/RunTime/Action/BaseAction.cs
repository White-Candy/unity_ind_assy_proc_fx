using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAction
{
    public virtual async UniTask AsyncShow() { await UniTask.Yield(PlayerLoopTiming.Update); }

    public virtual void UpdateData() { }

    public virtual void Exit() { }
}
