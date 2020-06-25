﻿using System;
 using Core.State;

 public class StateTransition
{
    public IState From { get; private set; }
    public IState To { get; private set; }
    public Func<bool> Condition { get; private set; }

    public StateTransition(IState from, IState to, Func<bool> condition)
    {
        From = from;
        To = to;
        Condition = condition;
    }
}