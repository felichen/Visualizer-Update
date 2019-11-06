using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public GameObject av;
    public GameObject pt;
    public GameObject bc;
    GameObject avchild;
    GameObject ptchild;
    GameObject bcchild;
    // Start is called before the first frame update
    public AudioAnalyzer _audioAnalyzer;
    public Vector3 _rotateAxis, _rotateSpeed;
    void Start()
    {
        avchild = av.transform.GetChild(0).gameObject;
        ptchild = pt.transform.GetChild(0).gameObject;
        bcchild = bc.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (avchild.activeSelf == false && ptchild.activeSelf == false && bcchild.activeSelf == false)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                //transform.GetChild(i).transform.LookAt(this.transform);
                this.transform.Rotate(_rotateAxis.x * _rotateSpeed.x * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
                    _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
                    _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer);
            }
            //transform.GetChild(0).transform.LookAt(this.transform);
            //this.transform.Rotate(_rotateAxis.x * _rotateSpeed.x * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
            //    _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
            //    _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer);
        } else
        {
            //reset camera transformations to 0
            for (int i = 0; i < transform.childCount; i++)
            {
                this.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

    }
}
