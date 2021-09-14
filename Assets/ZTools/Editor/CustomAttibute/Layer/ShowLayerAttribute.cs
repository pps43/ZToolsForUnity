﻿using UnityEngine;
namespace ZTools.Editor
{
    /// <summary>
    /// Select layer in your script
    /// e.g. 
    /// [ShowLayer] public int layer;
    /// gameobject.layer = layer;
    /// </summary>
    public class ShowLayerAttribute : PropertyAttribute
    {
    }
}