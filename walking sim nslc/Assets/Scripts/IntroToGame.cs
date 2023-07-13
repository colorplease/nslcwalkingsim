using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroToGame : MonoBehaviour
{
    public GameObject[] testThing;
    public AudioSource intro; //menu music no drums
    public GameObject otherThing; //menu music with drums
    public GameObject loadingText;
    public GameObject networkMAnager;

    void Start()
    {
        StartCoroutine(startIntro());
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            networkMAnager.SetActive(true);
        }
    }
    IEnumerator startIntro()
    {
        intro.enabled = true;
        int i = 0;
        while (i < 8) 
        {
            testThing[i].SetActive(true);
            yield return new WaitForSecondsRealtime(1.195f);
            testThing[i].SetActive(false);
            yield return new WaitForSecondsRealtime(1.195f);
            i++;
        }
        intro.Stop();
        testThing[8].SetActive(true);
        yield return new WaitForSecondsRealtime(2.2f);
        otherThing.SetActive(true);
        testThing[8].SetActive(false);
        testThing[9].SetActive(true);

    }
    public void StartGame()
    {
        testThing[9].SetActive(false);
         networkMAnager.SetActive(true);
         loadingText.SetActive(true);
    }
}
