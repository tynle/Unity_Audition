using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMono<InputManager>
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.CanPlay())
            return;

        ICommand command = null;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            command = new UpCommand();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            command = new DownCommand();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            command = new LeftCommand();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            command = new RightCommand();
        }

        if (command != null)
            command.Excute();
    }
}
