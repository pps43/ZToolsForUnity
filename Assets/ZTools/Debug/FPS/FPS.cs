﻿using UnityEngine;
using UnityEngine.UI;

namespace ZTools.Debug
{
    /// <summary>
    /// To display framerate, attach Text or TextMesh compnent to this script
    /// </summary>
    public class FPS : MonoBehaviour
    {
        public int targetFrameRate = 60;
        public TextMesh textArea;
        public Text text;
        [SerializeField] private float _updateInterval = 0.5f; // seconds

        private float _frameCount = 0f;
        private float _dt = 0.0f;
        private float _fps = 0.0f;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }

        void Update()
        {
            _frameCount++;
            _dt += Time.deltaTime;
            if (_dt > _updateInterval)
            {
                _fps = _frameCount / _dt;
                _frameCount = 0f;

                _dt -= _updateInterval;
                //dt = 0; // this is not accurate
                show();
            }
        }

        void show()
        {
            if (textArea != null)
                textArea.text = _fps.ToString("0.0");

            if (text != null)
                text.text = _fps.ToString("0.0");
        }
    }
}