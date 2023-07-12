using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public GameObject CreateAndJoinRoomObject;

    [SerializeField]GameObject waiting;
    [SerializeField]TextMeshProUGUI friendDisplay;
    [SerializeField]TextMeshProUGUI countDownGameStart;
    [SerializeField]TextMeshProUGUI errorMessage;

    [SerializeField]TextMeshProUGUI lobbyName;
    int players;

    bool yeahIJoinedLetskiss;
    bool gameStarting;

    Coroutine errorTimeOut;

    void Start()
    {
        CreateAndJoinRoomObject.SetActive(true);
    }

    public void CreateRoom()
    {
        errorTimeOut = StartCoroutine(errorTimeOutCoroutine());
        PlayerPrefs.SetInt("jumpscare happened", 0);
        CreateAndJoinRoomObject.SetActive(false);
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        errorTimeOut = StartCoroutine(errorTimeOutCoroutine());
        PlayerPrefs.SetInt("jumpscare happened", 0);
        CreateAndJoinRoomObject.SetActive(false);
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        waiting.SetActive(true);
        yeahIJoinedLetskiss = true;
        StopCoroutine(errorTimeOut);
        lobbyName.SetText("LOBBY NAME: " + PhotonNetwork.CurrentRoom.Name);
        //PhotonNetwork.LoadLevel("EdgarEmporium");
    }

    IEnumerator errorTimeOutCoroutine()
    {
        yield return new WaitForSeconds(7f);
        CreateAndJoinRoomObject.SetActive(true);
        errorMessage.SetText("can't connect for some reason pls try again :(");
    }

    

    void Update()
    {
            if(yeahIJoinedLetskiss)
            {
                if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && gameStarting == false)
                {
                    friendDisplay.SetText("Friend Joined :)");
                    countDownGameStart.gameObject.SetActive(true);
                    StartCoroutine(gameStart());
                }
            }
    }

    IEnumerator gameStart()
    {
        gameStarting = true;
        countDownGameStart.SetText("GAME STARTS IN 5");
        yield return new WaitForSeconds(1);
        countDownGameStart.SetText("GAME STARTS IN 4");
        yield return new WaitForSeconds(1);
        countDownGameStart.SetText("GAME STARTS IN 3");
        yield return new WaitForSeconds(1);
        countDownGameStart.SetText("GAME STARTS IN 2");
        yield return new WaitForSeconds(1);
        countDownGameStart.SetText("GAME STARTS IN 1");
        yield return new WaitForSeconds(1);
        countDownGameStart.SetText("GAME STARTS IN 0");
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("EdgarEmporium");
    }
    
}
