using UnityEngine;
namespace ZTools.Editor.CustomAttribute
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