using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour
{
    public float range;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            print(hit.collider.tag);
        }
        Debug.DrawRay(transform.position, transform.forward, Color.red, range);
    }
}
