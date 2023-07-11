using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
    public Light flashLight;
    public Light directionalLight;

    public float minTimeBetweenChase, maxTimeBetweenChase;
    public float minTimeChaseDuration, maxTimeChaseDuration;

    bool mapOpen;
    public GameObject map;

    public AudioClip[] clips;
    public AudioSource gameSounds;
    [SerializeField]TextMeshProUGUI loadingPercentage;
    public GameObject loadingMenu;

    Coroutine openMap;
    Coroutine restartSceneCoroutineStop;

    public int paintingsLeft;

    public CharacterController playerController;
    public Vector3 edgarSpawn;
    public GameObject edgar;
    public Transform Player;

    public AudioSource heartBeatIntensifies;
    public AudioSource music;

    public TextMeshProUGUI paintingsCollected;

    public ShakeMe shake;
    bool theChase;
    public float shakeMultiplier;

    public GameObject edgarJumpScare;
    public GameObject gameOverComponents;
    public GameObject endWall;

    bool restarting;
    public bool messageInProgress;


    void Start()
    {
         GameObject[] paintings = GameObject.FindGameObjectsWithTag("painting");
         paintingsLeft = paintings.Length;
         StartCoroutine(openingSequence());
         StartCoroutine(jumpScare());
    }

    public void RestartScene()
    {
        if(!restarting)
        {
            print("yeah");
            Player.gameObject.GetComponent<FirstPersonController>().isDead = false;
            edgar.SetActive(false);
            restarting = true;
            playerController.enabled = false;
            Player.gameObject.GetComponent<FirstPersonController>().enabled = false;
            StartCoroutine(RestartSceneCoroutine());
        }

    }

    IEnumerator RestartSceneCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Player.transform.position = new Vector3(45.6f, 0.94f, 73.37f);
        yield return new WaitForSeconds(0.2f);
        playerController.enabled = true;
        Player.gameObject.GetComponent<FirstPersonController>().enabled = true;
        Player.transform.rotation = Quaternion.Euler(0, 0, 0);
        StopAllCoroutines();
        flashLight.intensity = 0f;
        theChase = false;
        shake.shakeDuration = 0;
        music.pitch = 1f;
        heartBeatIntensifies.Stop();
        directionalLight.intensity = 4.15f;
        edgar.transform.position = edgarSpawn;
        edgar.SetActive(false);
        edgarJumpScare.SetActive(false);
        gameOverComponents.SetActive(false);
        music.Play();
        StartCoroutine(controlChase());
        StartCoroutine(openingSequence());
        restarting = false;

    }

    public void Retry()
    {
        PlayerPrefs.SetInt("jumpscare happened", 0);
        RestartScene();
        
    }

    IEnumerator jumpScare()
    {
        if(PlayerPrefs.GetInt("jumpscare happened") == 1)
        {
            yield return new WaitForSeconds(10f);
            playerController.enabled = false;
            Player.gameObject.GetComponent<FirstPersonController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            edgarJumpScare.SetActive(true);
            gameSounds.PlayOneShot(clips[2], 1f);
            music.Stop();
            heartBeatIntensifies.Stop();
            yield return new WaitForSeconds(1f);
            gameOverComponents.SetActive(true);
        }
    }

    IEnumerator openingSequence()
    {
        paintingsCollected.SetText("WAKE UP");
        yield return new WaitForSeconds(2);
        paintingsCollected.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        paintingsCollected.gameObject.SetActive(true);
        paintingsCollected.SetText("YOU WILL STEAL PAINTINGS FOR ME");
        yield return new WaitForSeconds(2);
        paintingsCollected.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        paintingsCollected.gameObject.SetActive(true);
        paintingsCollected.SetText("STEAL ALL OF THEM AND YOU CAN ESCPE");
        yield return new WaitForSeconds(2);
        paintingsCollected.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        paintingsCollected.gameObject.SetActive(true);
        paintingsCollected.SetText("'IT' MIGHT BE CHASING YOU");
        yield return new WaitForSeconds(2);
        paintingsCollected.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        paintingsCollected.gameObject.SetActive(true);
        paintingsCollected.SetText("PRESS 'R' TO OPEN YOUR MAP");
        yield return new WaitForSeconds(2);
        paintingsCollected.gameObject.SetActive(false);
        StartCoroutine(controlChase());
    }

    public IEnumerator messageToPlayer(string[] messages)
    {
        messageInProgress = true;
        foreach (string i in messages) 
        {
            paintingsCollected.gameObject.SetActive(true);
            paintingsCollected.SetText(i);
            yield return new WaitForSeconds(1);
            paintingsCollected.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        messageInProgress = false;
    }

    void Update()
    {
        //print(Vector3.Distance(Player.transform.position, edgar.transform.position));
        if(Vector3.Distance(Player.transform.position, edgar.transform.position) < 30)
        {
            if(mapOpen)
            {
                loadingMenu.SetActive(false);
                map.SetActive(false);
                playerController.enabled = true;
                StopCoroutine(openMap);
            }
        }
        if(Player.gameObject.GetComponent<FirstPersonController>().isDead)
        {
            RestartScene();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(mapOpen == false)
            {
                mapOpen = true;
                paintingsCollected.gameObject.SetActive(false);
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
        if(theChase)
        {
            shake.shakeAmount = (1/Vector3.Distance(Player.position, edgar.transform.position) * shakeMultiplier);
        }
    }

    public void PaintingCollected()
    {
        gameSounds.PlayOneShot(clips[1]);
        if(paintingsLeft > 0)
        {
            paintingsCollected.SetText(paintingsLeft.ToString() + " LEFT");
        }
        else
        {
            paintingsCollected.SetText("LEAVE");
            endWall.SetActive(false);

        }
        StartCoroutine(displayPaintingsLeft());
    }

    IEnumerator displayPaintingsLeft()
    {
        paintingsCollected.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        paintingsCollected.gameObject.SetActive(false);
    }

    IEnumerator LoadingSequence()
    {
        playerController.enabled = false;
        loadingMenu.SetActive(true);
        int i = 0;
        while (i<6)
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
        shake.shakeDuration = 999;
        theChase = true;
        directionalLight.intensity = 0;
        music.pitch = 0.5f;
        edgar.SetActive(true);
        heartBeatIntensifies.Play();
        loadingMenu.SetActive(false);
        mapOpen = false;
        map.SetActive(false);
        playerController.enabled = true;
        StopCoroutine(openMap);
        yield return new WaitForSeconds(Random.Range(minTimeChaseDuration, maxTimeChaseDuration));
        flashLight.intensity = 0f;
        theChase = false;
        shake.shakeDuration = 0;
        music.pitch = 1f;
        heartBeatIntensifies.Stop();
        directionalLight.intensity = 4.15f;
        edgar.transform.position = edgarSpawn;
        edgar.SetActive(false);
        StartCoroutine(controlChase());
    }
}
