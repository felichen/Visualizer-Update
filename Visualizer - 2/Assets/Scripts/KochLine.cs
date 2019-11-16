using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    LineRenderer lineRenderer;
    //[Range(0,1)]
    //public float _lerpAmount;
    Vector3[] lerpPosition;
    public float multiplier;
    private float[] lerpAudio;

    [Header("Audio")]
    public AudioAnalyzer _audioAnalyzer;
    public int[] _audioBand;
    public Material _material;
    public Color _color;
    private Material _matInstance;
    public int _audioBandMaterial;
    public float _emissionMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        lerpAudio = new float[_initiatorPointAmount];
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = _position.Length;
        lineRenderer.SetPositions(_position);
        lerpPosition = new Vector3[_position.Length];

        //apply material
        _matInstance = new Material(_material);
        lineRenderer.material = _matInstance;
    }

    // Update is called once per frame
    void Update()
    {
        _matInstance.SetColor("_EmissionColor", _color * _audioAnalyzer._audioBandBuffer64[_audioBandMaterial] * _emissionMultiplier);
        if (_generationCount != 0)
        {
            int count = 0;
            for (int i = 0; i < _initiatorPointAmount; i++)
            {
                lerpAudio[i] = _audioAnalyzer._audioBandBuffer64[_audioBand[i]];
                for (int j = 0; j < (_position.Length - 1) / _initiatorPointAmount; j++)
                {
                    lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], lerpAudio[i]);
                    count++;
                }
            }
            lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], lerpAudio[_initiatorPointAmount - 1]);

            if (_useBezierCurves)
            {
                _bezierPosition = BezierCurve(lerpPosition, _bezierVertexCount);
                lineRenderer.positionCount = _bezierPosition.Length;
                lineRenderer.SetPositions(_bezierPosition);
            } else
            {
                lineRenderer.positionCount = lerpPosition.Length;
                lineRenderer.SetPositions(lerpPosition);
            }
        }
    }
}
