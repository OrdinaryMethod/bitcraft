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

    [SerializeField] private Transform searchPos;
    [SerializeField] private float searchRange;
    [SerializeField] private LayerMask nodeLayer;

    public bool inCombat; //handle attacks
    public bool isMelee;
    public bool isRange;
    public int damage;

    [SerializeField] private List<GameObject> nearbyNodes;

    //Base variables
    public GameObject homeBase;
    public GameObject target;
    public Vector3 defaultPosition;
    private bool canFlip;

    //What this specific unit can do
    public GameObject currentNode;
    public bool hasResource;
    public bool isHarvestingNode = false;

    private void Awake()
    {
        movePosition = GetComponent<IMovePosition>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        nearbyNodes = new List<GameObject>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        canFlip = true;
        isHarvestingNode = false;
    }

    private void Update()
    {
        //Find base at all times
        if(homeBase == null)
        {
            homeBase = GameObject.FindGameObjectWithTag("Home"); //make this refresh to nearest home base
        }

        if(canFlip)
        {
            Flip();
        }
        
        AutoHarvestNode();
    }

    public void MoveTo(Vector2 targetPositon)
    {
        agent.SetDestination(targetPositon);
    }

    private void AutoHarvestNode()
    {
        if (target && isHarvestingNode)
        {
            if (hasResource && target == currentNode)
            {
                MoveTo(homeBase.transform.position);
            }
            else if (!hasResource && target == currentNode)
            {
                MoveTo(target.transform.position);
            }
        }
        else if(!target && isHarvestingNode)
        {
            Collider2D[] nodeCollider = Physics2D.OverlapCircleAll(searchPos.position, searchRange, nodeLayer);

            int newNode = 0;

            if(nodeCollider.Length > 0)
            {
                float dist = Vector3.Distance(nodeCollider[newNode].transform.position, transform.position);


                for (int i = 0; i < nodeCollider.Length; i++)
                {
                    if (dist >= Vector3.Distance(nodeCollider[i].transform.position, transform.position))
                    {
                        newNode = i;
                    }
                }

                target = nodeCollider[newNode].gameObject;
                currentNode = nodeCollider[newNode].gameObject;
            }        
        }
    }

    private void Flip()
    {
        // Check the sign of the velocity to determine the direction
        float velocityX = agent.velocity.x;
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
        //Ignore other units
        if (collision.gameObject.tag == "Unit")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        }

        //Harvesting
        if (collision.gameObject.tag == "Node" && target == collision.gameObject)
        {
            currentNode = collision.gameObject; //Set active node
          
            if (!hasResource)
            {
                StartCoroutine(HarvestResource(collision.gameObject));
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
        //Harvesting
        if (collision.gameObject.tag == "Node")
        {
            if (target == collision.gameObject)
            {
                currentNode = collision.gameObject; //Set active node
                StartCoroutine(HarvestResource(collision.gameObject));
            }
        }

        //Harvesting
        if (collision.gameObject.tag == "Home")
        {
            if (hasResource)
            {
                StartCoroutine(StoreResource());
            }
        }
    }

    IEnumerator HarvestResource(GameObject node)
    {
        canFlip = false;
        yield return new WaitForSeconds(1.5f);
        canFlip = true;
        node.GetComponent<Node>().health -= 1;
        hasResource = true;
    }

    IEnumerator StoreResource()
    {
        yield return new WaitForSeconds(1.5f);
       
        hasResource = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(searchPos.position, searchRange);
    }

}
