using UnityEngine;

/// <summary>
/// Select sortinglayer in your script
/// e.g. 
/// [ShowSortingLayer] [SerializeField] private string renderLayer = "";
/// gameobject.layer = layer;
/// </summary>
public class ShowSortingLayerAttribute : PropertyAttribute
{
}
