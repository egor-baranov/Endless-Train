using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public GameObject LeftArrow, RightArrow;
    public delegate void ToDo();
    public ToDo TODO;
    void Update() {
        if ((!LeftArrow.GetComponent<GUIButton>().Pressed && !RightArrow.GetComponent<GUIButton>().Pressed) ||
          (LeftArrow.GetComponent<GUIButton>().Pressed && RightArrow.GetComponent<GUIButton>().Pressed)) TODO.Invoke();
        else if(LeftArrow.GetComponent<GUIButton>().Pressed) LeftArrow.GetComponent<GUIButton>().TODOPressed.Invoke();
        else if(RightArrow.GetComponent<GUIButton>().Pressed) RightArrow.GetComponent<GUIButton>().TODOPressed.Invoke(); 
    } 
}
