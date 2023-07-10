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
    int players;

    bool yeahIJoinedLetskiss;
    bool gameStarting;

    void Start()
    {
        CreateAndJoinRoomObject.SetActive(true);
    }

    public void CreateRoom()
    {
        PlayerPrefs.SetInt("jumpscare happened", 0);
        CreateAndJoinRoomObject.SetActive(false);
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PlayerPrefs.SetInt("jumpscare happened", 0);
        CreateAndJoinRoomObject.SetActive(false);
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        waiting.SetActive(true);
        yeahIJoinedLetskiss = true;
        
        //PhotonNetwork.LoadLevel("EdgarEmporium");
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
