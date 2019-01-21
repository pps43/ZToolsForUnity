using UnityEngine;
using UnityEngine.UI;
namespace ZTools.UIUtility
{
    /// <summary>
    /// Multiple buttons share one handler with different data passed into it.
    /// </summary>
    public class ButtonGroupClick : MonoBehaviour
    {
        public Button[] buttons;

        private void Awake()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int _i = i;//must do this, or it will always pass buttons.Length into handler(i)
                buttons[i].onClick.AddListener(delegate () { hander(_i); });
            }
        }

        private void hander(int idx)
        {
            Debug.Log("idx = " + idx.ToString());
            Debug.Log("You clicked button:" + buttons[idx].name);
        }
    }
}