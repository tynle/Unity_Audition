using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpCommand : ICommand
{
    // Start is called before the first frame update
    public void Excute()
    {
        GameManager.Instance.ProcessCommand(PoolManager.PoolObject.Up);
        // throw new System.NotImplementedException();
    }
}
