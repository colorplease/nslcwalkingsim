using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractScript : MonoBehaviour
{
    public float range;
    public float backRange;
    public float backRad;
    public GameObject pickUpText;
    public GameManager gameManage;
    public Transform player;

    public string[] behindYOU;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if(hit.collider.tag == "painting")
            {
                if(!pickUpText.activeSelf && !gameManage.map.activeSelf && !gameManage.loadingMenu.activeSelf)
                {
                    pickUpText.SetActive(true);
                    pickUpText.GetComponent<TextMeshProUGUI>().SetText("Press E to pick up");
                }
                if(Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.gameObject.SetActive(false);
                        gameManage.paintingsLeft--;
                        gameManage.PaintingCollected();
                    }
            }
            else if(hit.collider.tag == "must")
            {
                if(!pickUpText.activeSelf && !gameManage.map.activeSelf && !gameManage.loadingMenu.activeSelf)
                {
                    pickUpText.SetActive(true);
                    pickUpText.GetComponent<TextMeshProUGUI>().SetText("collect all paintings before you leave");
                }
            }
            else
            {
                pickUpText.SetActive(false);
            }
        }
        else
        {
            pickUpText.SetActive(false);
        }
        RaycastHit hit2;
        if(Physics.Raycast(player.position, -player.transform.forward, out hit2, backRange))
        {
            //print(hit2.collider.tag);
            if(hit2.collider.tag == "Enemy" && !gameManage.messageInProgress)
            {
                StartCoroutine(gameManage.messageToPlayer(behindYOU));
            }
        }
        Debug.DrawRay(player.position, -player.transform.forward, Color.red, backRange);
    }

    
}
