using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightCommand : ICommand
{
    public void Excute()
    {
        GameManager.Instance.ProcessCommand(PoolManager.PoolObject.Right);
        // throw new System.NotImplementedException();
    }
}
