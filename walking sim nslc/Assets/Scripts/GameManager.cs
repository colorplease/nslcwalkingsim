using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Light flashLight;
    public Light directionalLight;

    public float minTimeBetweenChase, maxTimeBetweenChase;
    public float minTimeChaseDuration, maxTimeChaseDuration;
    public float minPowerRespawnTime, maxPowerRespawnTime;

    bool mapOpen;
    public GameObject map;

    public AudioClip[] clips;
    public AudioSource gameSounds;
    [SerializeField]TextMeshProUGUI loadingPercentage;
    public GameObject loadingMenu;

    Coroutine openMap;
    Coroutine restartSceneCoroutineStop;
    Coroutine sequenceOpen;
    Coroutine moddedChase;
    Coroutine controlChase2Coroutine;
    Coroutine beSpooked;

    public int paintingsLeft;

    public CharacterController playerController;
    public Vector3 edgarSpawn;
    public GameObject edgar;
    public Transform Player;

    public AudioSource heartBeatIntensifies;
    public AudioSource music;

    public TextMeshProUGUI paintingsCollected;
    public TextMeshProUGUI paintingsCollectedHUD;
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
    public Transform[] powerSpawns;

    public PhotonView photonView;
    public int numOfPlayersReady;
    public int numOfPlayersAlive;

    public GameObject scribbles;
    public GameObject chair;

    public int frameRate = 100;
    public int powerUpID;
    public bool powerUpUse;

    public Sprite lagAbility;
    public Sprite lightAbility;
    public Sprite scareAbility;
    public Image abilityImage;
    public TextMeshProUGUI abilityTitle;
    public GameObject abilityObject;
    bool lightsCut = false;


    void Start()
    {
         GameObject[] paintings = GameObject.FindGameObjectsWithTag("painting");
         paintingsLeft = paintings.Length;
         sequenceOpen = StartCoroutine(openingSequence());
         paintingsCollectedHUD.SetText(paintingsLeft.ToString());
         numOfPlayersAlive = 2;
        QualitySettings.vSyncCount = 0;
         Application.targetFrameRate =  500;
    }

    public void RestartScene()
    {
        if(!restarting)
        {
            photonView.RPC("LosePlayerAlive", RpcTarget.All);
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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate =  500;
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
        paintingsCollectedHUD.gameObject.SetActive(false);
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
        paintingsCollectedHUD.gameObject.SetActive(true);
        photonView.RPC("AddPlayerAlive", RpcTarget.All);
        StartCoroutine(powerUpSpawn());
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
        RestartScene();
        
    }

    IEnumerator jumpScare()
    {
            yield return new WaitForSeconds(2f);
            edgarJumpScare.SetActive(true);
            gameSounds.PlayOneShot(clips[2], 1f);
            yield return new WaitForSeconds(1f);
            edgarJumpScare.SetActive(false);
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
            else if(photonView.IsMine)
            {
                photonView.RPC("ChangeLightStateClient", RpcTarget.All);
            }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && powerUpUse)
        {
                switch(powerUpID)
            {
                case 0:
                if(photonView.IsMine)
                {
                    photonView.RPC("FrameRateDebuffMaster", RpcTarget.All);
                }
                else
                {
                    photonView.RPC("FrameRateDebuffClient", RpcTarget.All);
                }
                break;
                case 1:
                lightsCut = true;
                UpdateLightState();
                break;
                case 2:
                if(photonView.IsMine)
                {
                    photonView.RPC("jumpScareMaster", RpcTarget.All);
                }
                else
                {
                    photonView.RPC("jumpScareClient", RpcTarget.All);
                }
                break;
            }
            gameSounds.PlayOneShot(clips[6]);
            powerUpUse = false;
            abilityObject.SetActive(false);
            photonView.RPC("AddPlayerAlive", RpcTarget.All);
        }
        //print(Vector3.Distance(Player.transform.position, edgar.transform.position));
        if(Vector3.Distance(Player.transform.position, edgar.transform.position) < 30)
        {
            if(mapOpen && edgar.activeSelf)
            {
                if(beSpooked != null)
                {
                    StopCoroutine(beSpooked);
                }
                paintingsCollected.gameObject.SetActive(false);
                string[] beScared = {"HE IS HERE"};
                beSpooked = StartCoroutine(messageToPlayer(beScared));
                loadingMenu.SetActive(false);
                map.SetActive(false);
                playerController.enabled = true;
                StopCoroutine(openMap);
                paintingsCollectedHUD.gameObject.SetActive(true);
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
                paintingsCollectedHUD.gameObject.SetActive(false);
            }
            else
            {
                loadingMenu.SetActive(false);
                mapOpen = false;
                map.SetActive(false);
                playerController.enabled = true;
                StopCoroutine(openMap);
                paintingsCollectedHUD.gameObject.SetActive(true);
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
            paintingsCollectedHUD.SetText(paintingsLeft.ToString());
        }
        else
        {
            paintingsCollected.SetText("LEAVE");
            paintingsCollectedHUD.SetText("0");
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
        paintingsCollectedHUD.gameObject.SetActive(false);
        paintingsCollected.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        paintingsCollected.gameObject.SetActive(false);
        paintingsCollectedHUD.gameObject.SetActive(true);
    }

    IEnumerator LoadingSequence()
    {
        playerController.enabled = false;
        loadingMenu.SetActive(true);
        int i = 0;
        while (i<5)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
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
            scribbles.SetActive(true);
            print("master off");
            moddedChase = null;
            moddedChase = StartCoroutine(controlChaseModded());
        }  
        else
        {
            scribbles.SetActive(false);
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
        if(scribbles.activeSelf && lightsCut)
                {
                    string[] lightDebuff = {"LIGHTS OFF"};
                    StartCoroutine(messageToPlayer(lightDebuff));
                    lightsCut = false;
                }
        if(!scribbles.activeSelf && lightsCut)
        {
                    string[] lightDebuff = {"LIGHTS ON"};
                    StartCoroutine(messageToPlayer(lightDebuff));
                    lightsCut = false;
        }
    }

    public void GeneratePowerUp()
    {
        photonView.RPC("LosePlayerAlive", RpcTarget.All);
        var powerUpChance = Random.Range(0,3);
        switch(powerUpChance)
        {
            case 0:
            powerUpID = 0;
            abilityImage.sprite = lagAbility;
            abilityTitle.SetText("LAG EXTREME");
            powerUpUse = true;
            abilityObject.SetActive(true);
            break;

            case 1:
            powerUpID = 1;
            abilityImage.sprite = lightAbility;
            abilityTitle.SetText("CIRCUIT BREAKER");
            powerUpUse = true;
            abilityObject.SetActive(true);
            break;

            case 2:
            powerUpID = 2;
            abilityImage.sprite = scareAbility;
            abilityTitle.SetText("JUMPSCARE");
            powerUpUse = true;
            abilityObject.SetActive(true);
            break;
        }
    }

    IEnumerator frameDebuffTimer()
    {
        photonView.RPC("LosePlayerAlive", RpcTarget.All);
        yield return new WaitForSecondsRealtime(30f);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate =  500;
        photonView.RPC("AddPlayerAlive", RpcTarget.All);
    }

    [PunRPC]
    void FrameRateDebuffClient()
    {
        if(photonView.IsMine)
        {
            StartCoroutine(frameDebuffTimer());
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate =  15;
            string[] frameDebuff = {"NO FRAMES?"};
            StartCoroutine(messageToPlayer(frameDebuff));
        }
        if(!photonView.IsMine)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate =  500;
            string[] frameDebuff = {"OPPONENT FPS LOWERED"};
            StartCoroutine(messageToPlayer(frameDebuff));
        }
    }

    [PunRPC]
    void FrameRateDebuffMaster()
    {
        if(!photonView.IsMine)
        {
            StartCoroutine(frameDebuffTimer());
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate =  15;
            Application.targetFrameRate = frameRate;
            string[] frameDebuff = {"NO FRAMES?"};
            StartCoroutine(messageToPlayer(frameDebuff));
        }
        if(photonView.IsMine)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate =  500;
            string[] frameDebuff = {"OPPONENT FPS LOWERED"};
            StartCoroutine(messageToPlayer(frameDebuff));
        }
    }

    [PunRPC]
    void jumpScareClient()
    {
        if(photonView.IsMine)
        {
            StartCoroutine(jumpScare());
        }
        else
        {
            string[] jumpDebuff = {"GET READY FOR THE SCREAM"};
            StartCoroutine(messageToPlayer(jumpDebuff));
        }
    }

    [PunRPC]
    void jumpScareMaster()
    {
        if(!photonView.IsMine)
        {
            StartCoroutine(jumpScare());
        }
        else
        {
            string[] jumpDebuff = {"GET READY FOR THE SCREAM"};
            StartCoroutine(messageToPlayer(jumpDebuff));
        }
    }

    [PunRPC]
    void ChangeLightStateClient()
    {
        if(!photonView.IsMine)
        {
            scribbles.SetActive(true);
            print("client off");
            moddedChase = null;
            moddedChase = StartCoroutine(controlChaseModded());
        }   
        else
        {
            scribbles.SetActive(false);
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
        if(scribbles.activeSelf && lightsCut)
                {
                    string[] lightDebuff = {"LIGHTS OFF"};
                    StartCoroutine(messageToPlayer(lightDebuff));
                    lightsCut = false;
                }
        if(!scribbles.activeSelf && lightsCut)
                {
                    string[] lightDebuff = {"LIGHTS ON"};
                    StartCoroutine(messageToPlayer(lightDebuff));
                    lightsCut = false;
                }
    }

    IEnumerator controlChase2(float imsorryperson)
    {
        StartCoroutine(powerUpSpawn());
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

    IEnumerator powerUpSpawn()
    {
        print("spawned");
            yield return new WaitForSeconds(Random.Range(minPowerRespawnTime, maxPowerRespawnTime));
            if(numOfPlayersAlive == 2 && !chair.activeSelf)
            {
                photonView.RPC("StartPowerUpSpawn", RpcTarget.All);
            }
        StartCoroutine(powerUpSpawn());
        
    }

    [PunRPC]
    void StartPowerUpSpawn()
    {
        chair.SetActive(true);
        chair.transform.position = powerSpawns[Random.Range(0, powerSpawns.Length - 1)].position;
        string [] powerMessage = {"CHECK YOUR MAP AND GET THE CHAIR"};
        StartCoroutine(messageToPlayer(powerMessage));
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

    [PunRPC]
    void AddPlayerAlive()
    {
        numOfPlayersAlive++;
    }

    [PunRPC]
    void LosePlayerAlive()
    {
        numOfPlayersAlive--;
    }

    [PunRPC]
    public void chairCollected()
    {
        chair.SetActive(false);
        string [] powerMessageCollect = {"SOMEONE HAS THE CHAIR"};
        StartCoroutine(messageToPlayer(powerMessageCollect));
    }

    public void chairCollectVoid()
    {
        photonView.RPC("chairCollected", RpcTarget.All);
    }


    [PunRPC]
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
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
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
            GameObject.FindGameObjectWithTag("winner").SetActive(false);
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
            GameObject.FindGameObjectWithTag("winner").SetActive(false);
            paintingsCollected.gameObject.SetActive(true);
            paintingsCollected.SetText("YOU SUCK");
            gameSounds.PlayOneShot(clips[4]);
            music.Stop();
            StartCoroutine(actualEnding());
        }
    }

 
}
