using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SongScript : MonoBehaviour
{
    public AudioAnalyzer audioAnalyzer;
    [SerializeField]
    private Text myText;

    public void SetText(string textStr)
    {
        myText.text = textStr;
    }

    public void OnClick()
    {
        audioAnalyzer.populateList();
        Debug.Log("lcount: " + audioAnalyzer.musicList.Count);
        for (int i = 0; i < audioAnalyzer.musicList.Count; i++) { 
       
            if (audioAnalyzer.musicList[i].name == myText.text)
            {
                Debug.Log("audio :" + audioAnalyzer.musicList[i].name);
                Debug.Log("clicked: " + myText.text);
                audioAnalyzer.setAudioClip(audioAnalyzer.musicList[i]);
            }
        }
    }
}
