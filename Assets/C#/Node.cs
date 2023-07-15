using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int health;
    public bool empty;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    private void Update()
    {
        if(health <= 0)
        {
            empty = true;
        }

        if(empty)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
