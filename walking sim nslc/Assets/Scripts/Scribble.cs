using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scribble : MonoBehaviour
{
    public Sprite[] scribbles;
    SpriteRenderer self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<SpriteRenderer>();
        self.sprite = scribbles[Random.Range(0, scribbles.Length - 1)];   
    }
}
