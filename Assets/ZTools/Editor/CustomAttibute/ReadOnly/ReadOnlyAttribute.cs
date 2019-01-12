using UnityEngine;
namespace ZTools.EditorUtil.CustomAttribute
{
    /// <summary>
    /// use this to make field readonly in editor;
    /// e.g. [ReadOnly] [SerializedField] private bool myVar = true;
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public string Info
        {
            get;
            protected set;
        }

        public ReadOnlyAttribute()
        {
            Info = null;
        }

        public ReadOnlyAttribute(string info)
        {
            Info = info;
        }
    }
}