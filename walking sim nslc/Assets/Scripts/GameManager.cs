using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using Photon.Pun;

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
    Coroutine sequenceOpen;
    Coroutine CHASE;
    Coroutine moddedChase;
    Coroutine controlChase2Coroutine;

    public int paintingsLeft;

    public CharacterController playerController;
    public Vector3 edgarSpawn;
    public GameObject edgar;
    public Transform Player;

    public AudioSource heartBeatIntensifies;
    public AudioSource music;

    public TextMeshProUGUI paintingsCollected;
    public TextMeshProUGUI testing;
    public TextMeshProUGUI deathSecondsLeft;

    public ShakeMe shake;
    bool theChase;
    public float shakeMultiplier;

    public GameObject edgarJumpScare;
    public GameObject gameOverComponents;
    public GameObject endWall;
    public GameObject deathScreen;

    bool restarting;
    public bool messageInProgress;

    public Transform[] edgarSpawns;

    [SerializeField]PhotonView photonView;
    public int numOfPlayersReady;


    void Start()
    {
         GameObject[] paintings = GameObject.FindGameObjectsWithTag("painting");
         paintingsLeft = paintings.Length - 1;
         sequenceOpen = StartCoroutine(openingSequence());
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
        CHASE = null;
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
        StartCoroutine(RestartSceneCoroutinePart2());
        
    }

    IEnumerator RestartSceneCoroutinePart2()
    {
        deathScreen.SetActive(true);
        deathSecondsLeft.SetText("10");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("9");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("8");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("7");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("6");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("5");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("4");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("3");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("2");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("1");
        yield return new WaitForSeconds(1);
        deathSecondsLeft.SetText("0");
        yield return new WaitForSeconds(1);
        deathScreen.SetActive(false);
        print("e");
        music.Play();
        //photonView.RPC("ReadyUpGuys", RpcTarget.All);
        string[] ouchie = {"OUCH"};
        StartCoroutine(messageToPlayer(ouchie));
        UpdateLightState();
        loadingMenu.SetActive(false);
        mapOpen = false;
        map.SetActive(false);
        playerController.enabled = true;
        StopCoroutine(openMap);
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
        photonView.RPC("ReadyUpGuys", RpcTarget.All);
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

    void UpdateLightState()
    {
            if(!photonView.IsMine)
            {
                photonView.RPC("ChangeLightStateMaster", RpcTarget.All);
            }
            else
            {
                photonView.RPC("ChangeLightStateClient", RpcTarget.All);
            }
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.L))
        //{
            //string[] winner = {"YOU WIN"};
            //StartCoroutine(messageToPlayer(winner));
        //}
        //print(Vector3.Distance(Player.transform.position, edgar.transform.position));
        if(Vector3.Distance(Player.transform.position, edgar.transform.position) < 30)
        {
            if(mapOpen && edgar.activeSelf)
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
                if(sequenceOpen != null)
                {
                    StopCoroutine(sequenceOpen);
                    photonView.RPC("ReadyUpGuys", RpcTarget.All);
                    paintingsCollected.gameObject.SetActive(false);
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
        if(sequenceOpen != null)
                {
                    StopCoroutine(sequenceOpen);
                    photonView.RPC("ReadyUpGuys", RpcTarget.All);
                    paintingsCollected.gameObject.SetActive(false);
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
        //print("started");
        yield return new WaitForSeconds(Random.Range(minTimeBetweenChase, maxTimeBetweenChase));
        float value = 0;
        int index = 0;
        for(int i = 0; i < edgarSpawns.Length; i++)
        {
            if(Vector3.Distance(Player.transform.position, edgarSpawns[i].position) > value)
            {
                index = i;
                value = Vector3.Distance(Player.transform.position, edgarSpawns[i].position);
            }
        }
        edgar.transform.position = edgarSpawns[index].position;  
        flashLight.intensity = 7.94f;
        shake.shakeDuration = 999;
        theChase = true;
        directionalLight.intensity = 0;
        music.pitch = 0.5f;
        edgar.SetActive(true);
        heartBeatIntensifies.Play();
        yield return new WaitForSeconds(Random.Range(minTimeChaseDuration, maxTimeChaseDuration));
        flashLight.intensity = 0f;
        theChase = false;
        shake.shakeDuration = 0;
        music.pitch = 1f;
        heartBeatIntensifies.Stop();
        directionalLight.intensity = 4.15f;
        edgar.SetActive(false);
        StartCoroutine(controlChase());
    }

    IEnumerator controlChaseModded()
    {

        float value = 0;
        int index = 0;
        for(int i = 0; i < edgarSpawns.Length; i++)
        {
            if(Vector3.Distance(Player.transform.position, edgarSpawns[i].position) > value)
            {
                index = i;
                value = Vector3.Distance(Player.transform.position, edgarSpawns[i].position);
            }
        }
        edgar.transform.position = edgarSpawns[index].position;  
        flashLight.intensity = 7.94f;
        shake.shakeDuration = 999;
        theChase = true;
        directionalLight.intensity = 0;
        music.pitch = 0.5f;
        edgar.SetActive(true);
        heartBeatIntensifies.Play();
        yield return new WaitForSecondsRealtime(Random.Range(minTimeChaseDuration, maxTimeChaseDuration));
        UpdateLightState();
    }

    [PunRPC]
    void ChangeLightStateMaster()
    {
        if(photonView.IsMine)
        {
            print("master off");
            moddedChase = null;
            moddedChase = StartCoroutine(controlChaseModded());
        }   
        else
        {
            print("master on");
            StopCoroutine(controlChaseModded());
            flashLight.intensity = 0f;
            theChase = false;
            shake.shakeDuration = 0;
            music.pitch = 1f;
            heartBeatIntensifies.Stop();
            directionalLight.intensity = 4.15f;
            edgar.SetActive(false);
        }
    }

    [PunRPC]
    void ChangeLightStateClient()
    {
        if(!photonView.IsMine)
        {
            print("client off");
            moddedChase = null;
            moddedChase = StartCoroutine(controlChaseModded());
        }   
        else
        {
            print("client on");
            StopCoroutine(controlChaseModded());
            flashLight.intensity = 0f;
            theChase = false;
            shake.shakeDuration = 0;
            music.pitch = 1f;
            heartBeatIntensifies.Stop();
            directionalLight.intensity = 4.15f;
            edgar.SetActive(false);
        }
    }

    IEnumerator controlChase2(float imsorryperson)
    {
        yield return new WaitForSeconds(Random.Range(minTimeBetweenChase, maxTimeBetweenChase));
        if(imsorryperson > 5)
        {
            photonView.RPC("ChangeLightStateMaster", RpcTarget.All);
        }
        else
        {
            photonView.RPC("ChangeLightStateClient", RpcTarget.All);
        }
    }

    [PunRPC]
    void ReadyUpGuys()
    {
            numOfPlayersReady++;
            if(photonView.IsMine)
            {
                var whoGetsChasedFirst = Random.Range(0, 10);
                if(controlChase2Coroutine == null)
                {
                    controlChase2Coroutine = StartCoroutine(controlChase2(whoGetsChasedFirst));
                }
                
            }
    }

    public void GameEnd()
    {
            if(!photonView.IsMine)
            {
                photonView.RPC("MasterWin", RpcTarget.All);
            }
            else
            {
                photonView.RPC("ClientWin", RpcTarget.All);
            }
    }

    IEnumerator actualEnding()
    {
        
        yield return new WaitForSeconds(8f);
        //add map selector later
        PhotonNetwork.LoadLevel("EdgarEmporium");
    }

    [PunRPC]
    void ClientWin()
    {
        if(photonView.IsMine)
        {
            paintingsCollected.gameObject.SetActive(true);
            paintingsCollected.SetText("YOU WIN");
            gameSounds.PlayOneShot(clips[3]);
            music.Stop();
            StartCoroutine(actualEnding());
        }
        else
        {
            paintingsCollected.gameObject.SetActive(true);
            paintingsCollected.SetText("YOU SUCK");
            gameSounds.PlayOneShot(clips[4]);
            music.Stop();
            StartCoroutine(actualEnding());
        }
    }

    
    [PunRPC]
    void MasterWin()
    {
        if(!photonView.IsMine)
        {
            paintingsCollected.gameObject.SetActive(true);
            paintingsCollected.SetText("YOU WIN");
            gameSounds.PlayOneShot(clips[3]);
            music.Stop();
            StartCoroutine(actualEnding());
        }
        else
        {
            paintingsCollected.gameObject.SetActive(true);
            paintingsCollected.SetText("YOU SUCK");
            gameSounds.PlayOneShot(clips[4]);
            music.Stop();
            StartCoroutine(actualEnding());
        }
    }

 
}
