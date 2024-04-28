using UnityEngine;
using System;

[Serializable]
public class PerlinPropGenerator
{
    public MapProp[] prop;
    public float density;
    [Range(0.04f, 0.06f)] public float modifier;
    public bool useRandomModifier;
    public float[,] PerlinMap;
}