using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newVector2GridData", menuName = "Scriptable Objects/Vector2GridData")]
public class Vector2GridData : ScriptableObject
{
    public int width = 5;
    public int height = 5;
    public Vector2[] cells;
}