using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static float Hotizontal() => Input.GetAxis("Horizontal");
    public static float Vertical() => Input.GetAxis("Vertical");
    public static bool Jump() => Input.GetButtonDown("Jump");
    public static bool AddBlock() => Input.GetButtonDown("AddBlock");
    public static bool RemoveBlock() => Input.GetButtonDown("RemoveBlock");
    public static bool ShareBlock() => Input.GetButtonDown("ShareBlock");
}
