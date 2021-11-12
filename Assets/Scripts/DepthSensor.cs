using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using System.Linq;

public class DepthSensor : MonoBehaviour
{
    [SerializeField]
    private NNModel _monoDepthONNX;

    [SerializeField]
    private RawImage _sourceImage;

    [SerializeField]
    private RawImage _depthImage;

    private Model _runtimeModel;
    private IWorker _worker;

    private Texture2D _inputTexture;
    private Texture2D _depthTexture;
    private Rect _region;

    private const int _modelWidth = 224;
    private const int _modelHeight = 224;
    private const int _numChannels = 3;

    private Vector3[] _vertices;
    private Color[] _colors;

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// Initialize all member variables
    /// </summary>
    private void Init()
    {
        // start up barracuda model
        _runtimeModel = ModelLoader.Load(_monoDepthONNX);
        _worker = WorkerFactory.CreateComputeWorker(_runtimeModel);

        // initialize textures
        _region = new Rect(0, 0, _modelWidth, _modelHeight);
        _inputTexture = new Texture2D(_modelWidth, _modelHeight, TextureFormat.RGB24, false);
        _depthTexture = new Texture2D(_modelWidth, _modelHeight, TextureFormat.RGB24, false);

        // initialize array for points
        _vertices = new Vector3[_modelWidth * _modelHeight];
        _colors = new Color[_modelWidth * _modelHeight];
    }

    /// <summary>
    /// Load input image, update model, and update depth map
    /// </summary>
    /// <param name="filename"></param>
    public void LoadInputImage(string filename)
    {
        Init();

        // read input image
        Texture2D img = new Texture2D(_modelWidth, _modelHeight, TextureFormat.RGB24, false);
        byte[] data = System.IO.File.ReadAllBytes(filename);
        img.LoadImage(data);

        // resize input image
        RenderTexture tex = new RenderTexture(_modelWidth, _modelHeight, 0);
        Graphics.Blit(img, tex);
        RenderTexture.active = tex;
        _inputTexture.ReadPixels(_region, 0, 0);
        RenderTexture.active = null;
        _inputTexture.Apply();

        _sourceImage.texture = _inputTexture;

        UpdateModel();
    }

    /// <summary>
    /// Update model
    /// </summary>
    private void UpdateModel()
    {
        Color[] pixels = _inputTexture.GetPixels();

        if (pixels.Length >= (_modelWidth * _modelHeight))
        {
            var tensor = new Tensor(_inputTexture);
            var output = _worker.Execute(tensor).PeekOutput();
            float[] depth = output.AsFloats();
            UpdateDepthTexture(depth);
            _depthImage.texture = _depthTexture;

            tensor.Dispose();
        }
    }

    /// <summary>
    /// Update depth map texture coloring
    /// </summary>
    /// <param name="depth"></param>
    private void UpdateDepthTexture(float[] depth)
    {
        var min = Mathf.Min(depth);
        var max = Mathf.Max(depth);
        foreach (var pix in depth.Select((v, i) => new { v, i }))
        {
            var x = pix.i % _modelWidth;
            var y = pix.i / _modelWidth;
            var invY = _modelHeight - y - 1;

            // normalize depth value
            var val = (pix.v - min) / (max - min);
            _depthTexture.SetPixel(x, y, new Color(val, val, val));
            var worldPos = new Vector3(x / (_modelWidth / 0.9f), y / (_modelHeight / 0.9f), val);
            _vertices[y * _modelWidth + x] = worldPos;
            _colors[y * _modelWidth + x] = _inputTexture.GetPixel(x, invY);
        }

        _depthTexture.Apply();
    }

    /// <summary>
    /// Get depth map points
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetPoints()
    {
        return _vertices;
    }

    /// <summary>
    /// Get depth map points scaled by input scale
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    public Vector3[] GetPointsScaled(float scale)
    {
        Vector3[] outVert = _vertices;
        for (int i = 0; i < _vertices.Length; i++)
        {
            outVert[i] = scale * _vertices[i];
        }
        return outVert;
    }
}
