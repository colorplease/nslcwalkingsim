using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField]Transform player;
    public Transform jumpScareBox;
    void OnCollisionEnter(Collision other)
    {
        print("wsg");
        if(other.collider.tag == "Enemy")
        {
            player.position = jumpScareBox.position;
        }
    }
}
