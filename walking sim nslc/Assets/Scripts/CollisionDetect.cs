using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    public GameManager gameManager;
    void OnCollisionEnter(Collision other)
        {
            if(other.collider.tag == "Enemy")
            {
                PlayerPrefs.SetInt("jumpscare happened", 1);
                gameManager.RestartScene();

            }
        }
}
