using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : State
{


    public override void UpdateState()
    {
        base.UpdateState();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamesManager.instance.SwitchState<PauseState>();
        }

    }
}
