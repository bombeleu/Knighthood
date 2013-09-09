// Custom serializable class

using UnityEngine;


[System.Serializable]
public class ScaledCurve
{
    public float scale = 1;
    public string hello = "Hello World";
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
}