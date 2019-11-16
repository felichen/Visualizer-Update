using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
public class Controller : MonoBehaviour
{
    public Slider progressbar;
    public AudioAnalyzer _audioAnalyzer;
    public Image playlist;
    public Canvas fcp1;
    public Canvas fcp2;
    public Canvas menuItems;
    public AudioVisual _audioVisual;
    public BarsCenter _barsCenter;
    AudioSource _audioSource;
    float audioLength;

    public TMPro.TMP_Dropdown centerOptions;
    public TMPro.TMP_Dropdown freqOptions;
    public Button colorOne;
    public Button colorTwo;
    public Button Menu;


    public GameObject av;
    public GameObject pt;
    public GameObject bc;
    public GameObject phyllotaxisball;
    public GameObject koch;
    public Transform[] children;
    public string a = "AudioVisual";
    public string p = "Phyllotaxis";
    KochLine[,] allkochlines;
    Phyllotaxis[,] alltrails;
    // Start is called before the first frame update
    void Start()
    {
        allkochlines = new KochLine[12,3];
        alltrails = new Phyllotaxis[10, 6];
        //set audio for progress bar
        _audioSource = _audioAnalyzer._audioSource;
        audioLength = _audioSource.clip.length;

        //add center options
        List<string> center_options = new List<string>() { "Circle", "Travel", "Bars", "Koch", "Ball" };
        centerOptions.AddOptions(center_options);
        //add frequency options
        List<string> freq_options = new List<string>() { "Low", "Medium", "High" };
        freqOptions.AddOptions(freq_options);

        //pt = GameObject.Find("Phyllotaxis - Center");
        GameObject phylloParent = pt.transform.GetChild(0).gameObject;
        phylloParent.SetActive(false);

        //bc = GameObject.Find("Bars - Center");
        GameObject barParent = bc.transform.GetChild(0).gameObject;
        barParent.SetActive(false);

        //av = GameObject.Find("AudioVisual - Center");
        GameObject circleParent = av.transform.GetChild(0).gameObject;
        circleParent.SetActive(true);

        GameObject ballParent = phyllotaxisball.transform.GetChild(0).gameObject;
        ballParent.SetActive(false);

        GameObject kochparent = koch.transform.GetChild(0).gameObject;
        kochparent.SetActive(false);

        children = circleParent.GetComponentsInChildren<Transform>();

        fcp1.gameObject.SetActive(false);
        fcp2.gameObject.SetActive(false);
        menuItems.gameObject.SetActive(false);
        playlist.gameObject.SetActive(false);

        //for (int i = 0; i < phyllotaxisball.transform.childCount; i++)
        //{
        //    GameObject go = phyllotaxisball.transform.GetChild(i).gameObject;
        //    go.transform.parent = phyllotaxisball.transform;
        //}

        //get sides of koch
        for (int i = 0; i < 12; i++)
        {
            GameObject side = kochparent.transform.GetChild(i).gameObject;
            for (int j = 0; j < 3; j++)
            {
                GameObject line = side.transform.GetChild(j).gameObject;
                KochLine kochline = line.GetComponent<KochLine>();
                allkochlines[i, j] = kochline;
            }
        }

        //get phyllotaxis ball
        for (int i = 5; i < 10; i++)
        {
            GameObject side = ballParent.transform.GetChild(i).gameObject;
            for (int j = 0; j < 6; j++)
            {
                GameObject t = side.transform.GetChild(j).gameObject;
                Phyllotaxis trail = t.GetComponent<Phyllotaxis>();
                alltrails[i, j] = trail;
            }
        }
    }

    public void centerOption_changed(int index)
    {
        //av = GameObject.Find("AudioVisual - Center");
        GameObject circleParent = av.transform.GetChild(0).gameObject;
        //pt = GameObject.Find("Phyllotaxis - Center");
        GameObject phylloParent = pt.transform.GetChild(0).gameObject;
        //bc = GameObject.Find("Bars - Center");
        GameObject barParent = bc.transform.GetChild(0).gameObject;
        GameObject ballParent = phyllotaxisball.transform.GetChild(0).gameObject;
        GameObject kochparent = koch.transform.GetChild(0).gameObject;

        if (index == 1)
        {
            circleParent.SetActive(false);
            //foreach (Transform child in children)
            //{
            //    var name = child.gameObject.name;
            //    if (name.Substring(0, 2) == "PS")
            //    {
            //        child.gameObject.SetActive(false);
            //        ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            //        ps.enableEmission = false;
            //        ps.Stop(true);
            //    }

            //}
            phylloParent.SetActive(true);
            barParent.SetActive(false);
            ballParent.SetActive(false);
            kochparent.SetActive(false);
        }
        else if (index == 2)
        {
            barParent.SetActive(true);
            phylloParent.SetActive(false);
            circleParent.SetActive(false);
            ballParent.SetActive(false);
            kochparent.SetActive(false);
            //foreach (Transform child in children)
            //{
            //    var name = child.gameObject.name;
            //    if (name.Substring(0, 2) == "PS")
            //    {
            //        child.gameObject.SetActive(false);
            //        ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            //        ps.enableEmission = false;
            //        ps.Stop(true);
            //    }
            //}
        }
        else if (index == 0)
        {
            circleParent.SetActive(true);
            //foreach (Transform child in children)
            //{
            //    var name = child.gameObject.name;
            //    if (name.Substring(0, 2) == "PS")
            //    {
            //        child.gameObject.SetActive(true);
            //        ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            //        ps.enableEmission = false;
            //    }
            //}
            phylloParent.SetActive(false);
            barParent.SetActive(false);
            ballParent.SetActive(false);
            kochparent.SetActive(false);
        }
        else if (index == 4)
        {
            barParent.SetActive(false);
            phylloParent.SetActive(false);
            circleParent.SetActive(false);
            ballParent.SetActive(true);
            kochparent.SetActive(false);
        }
        else if (index == 3)
        {
            barParent.SetActive(false);
            phylloParent.SetActive(false);
            circleParent.SetActive(false);
            ballParent.SetActive(false);
            kochparent.SetActive(true);
        }
        //if ((av.GetComponent(a) as MonoBehaviour).enabled == true) {
        //    name = (av.GetComponent(a) as MonoBehaviour).name;
        //    Debug.Log(string.Format("{0}", name));
        //    (av.GetComponent(a) as MonoBehaviour).enabled = false;
        //    //(pt.GetComponent(p) as MonoBehaviour).enabled = true;
        //} else
        //{
        //    Debug.Log(string.Format("REACHED"));
        //    (av.GetComponent(a) as MonoBehaviour).enabled = true;
        //    //(pt.GetComponent(p) as MonoBehaviour).enabled = false;
        //}

    }

    public void freqOption_changed(int index)
    {
        if (index == 0) //sub bass
        {
            _audioVisual.keep = 0.1f;
            _barsCenter.start = 0;

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (j == 0) {
                            allkochlines[i, j]._audioBand[k] = 0;
                        }
                        if (j == 1)
                        {
                            allkochlines[i, j]._audioBand[k] = 2;
                        }
                        if (j == 2)
                        {
                            allkochlines[i, j]._audioBand[k] = 5;
                        }
                    }
                }
            }

            for (int i = 5; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    alltrails[i, j]._scaleBand = 1;
                }
            }
        }
        else if (index == 1) //bass
        {
            _audioVisual.keep = 0.3f;
            _barsCenter.start = 16;
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (j == 0)
                        {
                            allkochlines[i, j]._audioBand[k] = 10;
                        }
                        if (j == 1)
                        {
                            allkochlines[i, j]._audioBand[k] = 20;
                        }
                        if (j == 2)
                        {
                            allkochlines[i, j]._audioBand[k] = 30;
                        }
                    }
                }
            }


            for (int i = 5; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    alltrails[i, j]._scaleBand = 4;
                }
            }
        }
        else if (index == 2) //upper bass
        {
            _audioVisual.keep = 0.5f;
            _barsCenter.start = 32;

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (j == 0)
                        {
                            allkochlines[i, j]._audioBand[k] = 30;
                        }
                        if (j == 1)
                        {
                            allkochlines[i, j]._audioBand[k] = 45;
                        }
                        if (j == 2)
                        {
                            allkochlines[i, j]._audioBand[k] = 60;
                        }
                    }
                }
            }


            for (int i = 5; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    alltrails[i, j]._scaleBand = 7;
                }
            }
        }
        //else if (index == 3) //midrange
        //{
        //    _audioVisual.keep = 0.4f;
        //} else if (index == 4) // uppder midrange
        //{
        //    _audioVisual.keep = 0.5f;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgress();
        ////rotate ball
        //phyllotaxisball.transform.LookAt(this.transform);
        //Vector3 x = new Vector3(1, 0, 0);
        //Vector3 y = new Vector3(0, 1, 0);
        //Vector3 z = new Vector3(0, 0, 1);
        //phyllotaxisball.transform.Rotate(5 * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
        //7 * Time.deltaTime * _audioAnalyzer._AmplitudeBuffer,
        //0 * 75* Time.deltaTime * _audioAnalyzer._AmplitudeBuffer);

    }

    void UpdateProgress()
    {
        float currTime = _audioSource.time;
        progressbar.value = currTime / audioLength;
    }

    public void colorOne_changed()
    {
        if (fcp1.gameObject.activeSelf == true)
        {
            fcp1.gameObject.SetActive(false);
        } else
        {
            fcp1.gameObject.SetActive(true);
            fcp2.gameObject.SetActive(false);
        }
    }

    public void colorTwo_changed()
    {
        if (fcp2.gameObject.activeSelf == true)
        {
            fcp2.gameObject.SetActive(false);
        }
        else
        {
            fcp2.gameObject.SetActive(true);
            fcp1.gameObject.SetActive(false);
        }
    }

    public void onMenuClick()
    {
        if (menuItems.gameObject.activeSelf == false)
        {
            menuItems.gameObject.SetActive(true);
        } else
        {
            menuItems.gameObject.SetActive(false);
        }
       
    }

    public void onPlaylistClick()
    {
        if (playlist.gameObject.activeSelf == false)
        {
            playlist.gameObject.SetActive(true);
        }
        else
        {
            playlist.gameObject.SetActive(false);
        }
    }

}
