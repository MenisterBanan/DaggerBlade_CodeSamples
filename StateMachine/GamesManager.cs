using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesManager : StateMachine
{
    public static GamesManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SwitchState<PlayingState>();
    }

    private void Update()
    {
        UpdateStateMachine();
    }
}
