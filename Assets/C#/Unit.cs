using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private IMovePosition movePosition;
    private Rigidbody2D rigidbody;

    private SpriteRenderer spriteRenderer;
    private NavMeshAgent agent;

    public bool inCombat; //handle attacks

    public bool isMelee;
    public bool isRange;

    public int damage;

    //Base variables
    [SerializeField] private GameObject homeBase;
    public GameObject target;
    public Vector3 defaultPosition;

    //What this specific unit can do
    [SerializeField] private GameObject currentNode;
    [SerializeField] private bool hasResource;

    private void Awake()
    {
        movePosition = GetComponent<IMovePosition>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        //Find base at all times
        if(homeBase == null)
        {
            homeBase = GameObject.FindGameObjectWithTag("Home");
        }

        Flip();
        AutoHarvest();

    
      
    }

    public void MoveTo(Vector2 targetPositon)
    {
        agent.SetDestination(targetPositon);
    }

    private void AutoHarvest()
    {
        if (target)
        {
            //Auto Gather if target still a node
            if (hasResource && target == currentNode)
            {
                MoveTo(homeBase.transform.position);
            }
            else if (!hasResource && target == currentNode)
            {
                MoveTo(target.transform.position);
            }
        }
    }

    private void Flip()
    {
        // Check the sign of the velocity to determine the direction
        float velocityX = GetComponent<Rigidbody2D>().velocity.x;
        float direction = Mathf.Sign(velocityX);

        // Flip the sprite based on the direction
        if (direction > 0)
        {
            spriteRenderer.flipX = false; // Not flipped
        }
        else if (direction < 0)
        {
            spriteRenderer.flipX = true; // Flipped
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        //Ignore other units
        if (collision.gameObject.tag == "Unit")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        }

        //Lumber Harvesting
        if (collision.gameObject.tag == "Lumber" && target == collision.gameObject)
        {
            currentNode = collision.gameObject; //Set active node
          
            if (!hasResource)
            {
                StartCoroutine(HarvestResource());
            }
        }

        if (collision.gameObject.tag == "Home")
        {

            if (hasResource)
            {
                StartCoroutine(StoreResource());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        //if(collision.gameObject.tag == "Lumber")
        //{
        //    StopCoroutine(HarvestLumber());
        //}

        //if(collision.gameObject.tag == "Home")
        //{
        //    StopCoroutine(StoreLumber());
        //}
    }

    private void OnCollisionStay2D(Collision2D collision) //Account for already touching object
    {
        //Lumber Harvesting
        if (collision.gameObject.tag == "Lumber")
        {
            if (target == collision.gameObject)
            {
                currentNode = collision.gameObject; //Set active node
                StartCoroutine(HarvestResource());
            }
        }
    }


    IEnumerator HarvestResource()
    {
        Debug.Log("harvesting");
        yield return new WaitForSeconds(1.5f);
        
        hasResource = true;
    }

    IEnumerator StoreResource()
    {
        Debug.Log("storing");

        yield return new WaitForSeconds(1.5f);
       
        hasResource = false;
    }

}
