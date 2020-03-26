using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownCommand : ICommand
{
    public void Excute()
    {
        // throw new System.NotImplementedException();
        GameManager.Instance.ProcessCommand(PoolManager.PoolObject.Down);
    }
}
