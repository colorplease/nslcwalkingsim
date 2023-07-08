using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Light flashLight;
    public Light directionalLight;

    public float minTimeBetweenChase, maxTimeBetweenChase;
    public float minTimeChaseDuration, maxTimeChaseDuration;

    bool mapOpen;
    [SerializeField]GameObject map;

    public AudioClip[] clips;
    public AudioSource gameSounds;
    [SerializeField]TextMeshProUGUI loadingPercentage;
    [SerializeField]GameObject loadingMenu;
    Coroutine openMap;

    public CharacterController playerController;


    void Start()
    {
         StartCoroutine(controlChase());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(mapOpen == false)
            {
                mapOpen = true;
                if(openMap != null)
                {
                    StopCoroutine(openMap);
                }
                openMap = StartCoroutine(LoadingSequence());
            }
            else
            {
                loadingMenu.SetActive(false);
                mapOpen = false;
                map.SetActive(false);
                playerController.enabled = true;
                StopCoroutine(openMap);
            }
            
        }
    }

    IEnumerator LoadingSequence()
    {
        playerController.enabled = false;
        loadingMenu.SetActive(true);
        int i = 0;
        while (i<10)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            gameSounds.pitch = Random.Range(0.75f, 1.25f);
            gameSounds.PlayOneShot(clips[0], 0.25f);
            loadingPercentage.text = Random.Range(0, 99.99f).ToString() + "%";
            i++;
        }
        loadingMenu.SetActive(false);
        map.SetActive(true);
    }

    IEnumerator controlChase()
    {
        yield return new WaitForSeconds(Random.Range(minTimeBetweenChase, maxTimeBetweenChase));
        flashLight.intensity = 7.94f;
        directionalLight.intensity = 0;
        loadingMenu.SetActive(false);
        mapOpen = false;
        map.SetActive(false);
        playerController.enabled = true;
        StopCoroutine(openMap);
        yield return new WaitForSeconds(Random.Range(minTimeChaseDuration, maxTimeChaseDuration));
        flashLight.intensity = 0f;
        directionalLight.intensity = 4.15f;
        StartCoroutine(controlChase());
    }
}
