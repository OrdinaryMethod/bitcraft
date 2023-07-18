using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int health;
    public bool empty;

    public NodeType nodeType;

    public enum NodeType
    {
        Lumber,
        Carrion
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
}
