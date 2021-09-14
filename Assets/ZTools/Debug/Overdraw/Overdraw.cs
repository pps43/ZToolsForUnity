using UnityEngine;
using UnityEngine.UI;

namespace ZTools.Debug
{
    /// <summary>
    /// Analysis overdraw when play.
    /// Attach to an empty gameobject, and drag Camera and resultText to this script.
    /// This will give accumulatedAverageOverdraw and maxOverdraw.
    /// TODO : due to performance issue, this only runs in Editor Mode.
    /// </summary>
    public class Overdraw : MonoBehaviour
    {
        [Header("Camera Gameobject Name")]
        public string cameraToAnalysisName;
        public float sampleInterval = 1;
        public bool isOpen = true;
        public Text resultText;

        //Max overdraw ratio
        public float maxOverdraw { get; private set; }
        //Moving average overdraw ratio
        public float accumulatedAverageOverdraw { get { return _accumulatedIntervalOverdraw / _intervalFrames; } }

        private Camera _cameraToAnalysis;
        private Camera _overdrawCamera;
        private RenderTexture _overdrawTexture;
        private ComputeShader _computeShader;

        private const int DATA_SIZE = 128 * 128;
        private int[] _inputData = new int[DATA_SIZE];
        private int[] _resultData = new int[DATA_SIZE];
        private ComputeBuffer _resultBuffer;
        private Shader _replacementShader;

        // Last measurement
        /// <summary> The number of shaded fragments in the last frame. </summary>
        public long totalShadedFragments { get; private set; }
        /// <summary> The overdraw ration in the last frame. </summary>
        public float overdrawRatio { get; private set; }

        // Sampled measurement
        /// <summary> Number of shaded fragments in the measured time span. </summary>
        public long intervalShadedFragments { get; private set; }
        /// <summary> The average number of shaded fragments in the measured time span. </summary>
        public float intervalAverageShadedFragments { get; private set; }
        /// <summary> The average overdraw in the measured time span. </summary>
        public float intervalAverageOverdraw { get; private set; }


        private long _accumulatedIntervalFragments;
        private float _accumulatedIntervalOverdraw;
        private long _intervalFrames;
        private float _intervalTime = 0;

        /// <summary>
        /// switch on/off overdraw analysis
        /// </summary>
        /// <param name="isOn"></param>
        public void setOpenClose(bool isOn)
        {
            isOpen = isOn;
            resultText.gameObject.SetActive(isOn);
        }

        private void Awake()
        {
#if !UNITY_EDITOR
        return;          //TODO too slow on mobile
#else
            UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
#endif
            var camobj = GameObject.Find(cameraToAnalysisName);
            if (camobj != null)
            {
                _cameraToAnalysis = camobj.GetComponent<Camera>();
            }
            if (_cameraToAnalysis == null)
            {
                UnityEngine.Debug.LogWarning("Overdraw Analysis. no camera selected, use camera.main");
                _cameraToAnalysis = Camera.main;
            }

            _replacementShader = Shader.Find("Debug/OverdrawInt");

            _overdrawCamera = gameObject.AddComponent<Camera>();//
            _overdrawCamera.CopyFrom(_cameraToAnalysis);
            _overdrawCamera.SetReplacementShader(_replacementShader, null);

            RecreateTexture(_cameraToAnalysis, _overdrawCamera);
            RecreateComputeBuffer();

            _computeShader = Resources.Load<ComputeShader>("OverdrawParallelReduction");

            for (int i = 0; i < _inputData.Length; i++) { _inputData[i] = 0; }
        }

        private void LateUpdate()
        {
            if (!isOpen) { return; }

            _overdrawCamera.CopyFrom(_cameraToAnalysis);
            _overdrawCamera.clearFlags = CameraClearFlags.SolidColor;
            _overdrawCamera.backgroundColor = Color.black;
            _overdrawCamera.targetTexture = _overdrawTexture;
            _overdrawCamera.SetReplacementShader(_replacementShader, null);

            transform.position = _cameraToAnalysis.transform.position;
            transform.rotation = _cameraToAnalysis.transform.rotation;

            RecreateTexture(_cameraToAnalysis, _overdrawCamera);


            _intervalTime += Time.deltaTime;
            if (_intervalTime > sampleInterval)
            {
                intervalShadedFragments = _accumulatedIntervalFragments;
                intervalAverageShadedFragments = (float)_accumulatedIntervalFragments / _intervalFrames;
                intervalAverageOverdraw = (float)_accumulatedIntervalOverdraw / _intervalFrames;

                displayResult();

                _intervalTime -= sampleInterval;

                _accumulatedIntervalFragments = 0;
                _accumulatedIntervalOverdraw = 0;
                _intervalFrames = 0;

            }
        }

        private void OnPostRender()
        {
            if (!isOpen) { return; }

            int kernel = _computeShader.FindKernel("CSMain");

            RecreateComputeBuffer();

            // Setting up the data
            _resultBuffer.SetData(_inputData);
            _computeShader.SetTexture(kernel, "Overdraw", _overdrawTexture);
            _computeShader.SetBuffer(kernel, "Output", _resultBuffer);

            int xGroups = (_overdrawTexture.width / 32);
            int yGroups = (_overdrawTexture.height / 32);

            // Summing up the fragments
            _computeShader.Dispatch(kernel, xGroups, yGroups, 1);
            _resultBuffer.GetData(_resultData);

            // Getting the results
            totalShadedFragments = 0;
            for (int i = 0; i < _resultData.Length; i++)
            {
                totalShadedFragments += _resultData[i];
            }

            overdrawRatio = (float)totalShadedFragments / (xGroups * 32 * yGroups * 32);

            _accumulatedIntervalFragments += totalShadedFragments;
            _accumulatedIntervalOverdraw += overdrawRatio;
            _intervalFrames++;

            //Debug.Log("_accumulatedIntervalOverdraw: " + _accumulatedIntervalOverdraw + "/"+ _intervalFrames);

            if (overdrawRatio > maxOverdraw) maxOverdraw = overdrawRatio;
        }

        public void OnDestroy()
        {
            if (_overdrawCamera != null)
            {
                _overdrawCamera.targetTexture = null;
            }
            if (_resultBuffer != null) _resultBuffer.Release();
        }

        /// <summary> Checks if the overdraw texture should be updated. This needs to happen if the main camera
        /// configuration changes. </summary>
        private void RecreateTexture(Camera cameraToAnalysis, Camera myOverdrawCamera)
        {
            if (_overdrawTexture == null)
            {
                _overdrawTexture = new RenderTexture(myOverdrawCamera.pixelWidth, myOverdrawCamera.pixelHeight, 24, RenderTextureFormat.RFloat);
                _overdrawTexture.enableRandomWrite = true;
                myOverdrawCamera.targetTexture = _overdrawTexture;
            }

            if (cameraToAnalysis.pixelWidth != _overdrawTexture.width || cameraToAnalysis.pixelHeight != _overdrawTexture.height)
            {
                _overdrawTexture.Release();
                _overdrawTexture.width = cameraToAnalysis.pixelWidth;
                _overdrawTexture.height = cameraToAnalysis.pixelHeight;
            }
        }

        private void RecreateComputeBuffer()
        {
            if (_resultBuffer != null) return;
            _resultBuffer = new ComputeBuffer(_resultData.Length, 4);
        }
        private void displayResult()
        {
            resultText.text = "Overdraw(Cur/Max):" + accumulatedAverageOverdraw.ToString("0.00") + " / " + maxOverdraw.ToString("0.00");
        }
    }
}