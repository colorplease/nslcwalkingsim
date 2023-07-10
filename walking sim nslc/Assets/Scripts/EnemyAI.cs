using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]Transform movePosition;
    [SerializeField]UnityEngine.AI.NavMeshAgent agent;
    [SerializeField]AudioSource scaryMan;
    [SerializeField]AudioClip synthesis;

    void OnEnable()
    {
        StartCoroutine(scaryMansource());
    }
    // Update is called once per frame
    void Update()
    {
        agent.destination = movePosition.position;
    }

    IEnumerator scaryMansource()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        scaryMan.pitch = Random.Range(0.25f, 2f);
        scaryMan.PlayOneShot(synthesis);
        StartCoroutine(scaryMansource());
    }
}
