using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseState : State
{

    public override void UpdateState()
    {
        base.UpdateState();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamesManager.instance.SwitchState<PlayingState>();

        }

    }

}
