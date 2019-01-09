using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace ZTools.Debug
{
    /// <summary>
    /// print unity log onto screen.
    /// use it on target device to see log immediately
    /// </summary>
    public class LogWindow : MonoBehaviour
    {
        public int maxLogLine = 20;//clear after exceed

        private TextMesh _textArea;
        private StringBuilder _sb = new StringBuilder();
        private Text _textUI;
        private int _curLogLine = 0;

        private void Awake()
        {
            _textArea = GetComponent<TextMesh>();
            _textUI = GetComponent<Text>();
            //DontDestroyOnLoad(gameObject);
            _curLogLine = 0;
        }

        void OnEnable()
        {
            Application.logMessageReceived += LogMessage;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogMessage;
        }

        private void LogMessage(string message, string stackTrace, LogType type)
        {
            _curLogLine++;
            if(_curLogLine > maxLogLine)
            {
                _curLogLine = 0;
                _sb.Length = 0;
            }

            if (type == LogType.Error || type == LogType.Exception)
            {
                _sb.Append("<color=red>").Append(message).Append("</color>").Append("\n");

            }
            else if(type == LogType.Warning)
            {
                _sb.Append("<color=orange>").Append(message).Append("</color>").Append("\n");
            }
            else
            {
                _sb.Append(message).Append("\n");
            }
            if (_textArea)
            {
                _textArea.text = _sb.ToString();
            }
            if(_textUI)
            {
                _textUI.text = _sb.ToString();
            }
            
        }
    }
}