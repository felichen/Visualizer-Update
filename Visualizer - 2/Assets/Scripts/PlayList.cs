using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayList : MonoBehaviour
{
    public AudioAnalyzer audioAnalyzer;
    [SerializeField]
    private GameObject buttonTemplate;

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < 20; i++)
        //{
        //    GameObject button = Instantiate(buttonTemplate) as GameObject;
        //    button.SetActive(true);

        //    button.GetComponent<SongScript>().SetText("Button " + i);

        //    button.transform.SetParent(buttonTemplate.transform.parent, false);
        //}
        audioAnalyzer.populateList();

        Debug.Log("count: " + audioAnalyzer.musicList.Count);

        for (int i = 0; i < audioAnalyzer.musicList.Count; i++)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            button.GetComponent<SongScript>().SetText(audioAnalyzer.musicList[i].name);

            button.transform.SetParent(buttonTemplate.transform.parent, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateButtons()
    {

    }
}
