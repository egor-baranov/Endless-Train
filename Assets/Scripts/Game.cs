using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public delegate void ToDo();
    public ToDo OnPauseAction, OnResumeAction;

    public bool paused = false;

    public bool Paused { 
        get => paused; 
        set {
            paused = value;
            if (value) OnResumeAction?.Invoke();
            else OnPauseAction?.Invoke();
        } 
    }
}
