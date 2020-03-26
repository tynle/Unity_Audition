using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftCommand : ICommand
{
    public void Excute()
    {
        GameManager.Instance.ProcessCommand(PoolManager.PoolObject.Left);
        // throw new System.NotImplementedException();
    }
}
