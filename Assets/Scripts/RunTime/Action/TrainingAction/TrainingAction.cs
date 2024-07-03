using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrainingAction : BaseAction
{
    public TrainingAction()
    {
        m_Token = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {

    }
}
