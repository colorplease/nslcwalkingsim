using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        print("1");
        PhotonNetwork.ConnectUsingSettings();
        StartCoroutine(connectTimeOut());
    }

    public override void OnConnectedToMaster()
    {
        print("2");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("3");
        SceneManager.LoadScene("Lobby");
    }

    IEnumerator connectTimeOut()
    {
        yield return new WaitForSeconds(7f);
        PhotonNetwork.ConnectUsingSettings();
    }
}
