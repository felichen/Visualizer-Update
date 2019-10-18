using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioAnalyzer _audioAnalyzer;
    public Vector3 _rotateAxis, _rotateSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).transform.LookAt(this.transform);
        this.transform.Rotate(_rotateAxis.x * _rotateSpeed.x * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
            _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
            _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer);
    }
}
