using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private GameMaster gameMaster;

    private IMovePosition movePosition;
    private Rigidbody2D rigidbody;

    private SpriteRenderer spriteRenderer;
    private NavMeshAgent agent;

    [SerializeField] private Transform searchPos;
    [SerializeField] private float searchRange;
    [SerializeField] private LayerMask nodeLayer;

    public bool isEnemy;
    public bool inCombat; //handle attacks
    public bool isMelee;
    public bool isRange;
    public int damage;

    [SerializeField] private List<GameObject> nearbyNodes;

    public GameObject homeBase;
    public GameObject target;
    public Vector3 defaultPosition;
    private bool canFlip;
    public bool canMove;

    public GameObject currentNode;
    public bool hasResource;
    public bool isHarvestingNode = false;
    private bool seekNewNode = false;

    //Report status
    [SerializeField] private GameObject statusBubble;


    private void Awake()
    {
        movePosition = GetComponent<IMovePosition>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        nearbyNodes = new List<GameObject>();

        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        canFlip = true;
        isHarvestingNode = false;
        canMove = true;
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

        try
        {
       
            ReportStatus();
            AutoHarvestNode();
            StopMovement();   
        }
        catch(System.Exception e)
        {
           
        }
           
    }

    private class newNode
    {
        public float distance { get; set; }
        public GameObject node { get; set; }
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

            //FindClosestNode();
        }
        else if(!target && isHarvestingNode) //Move on to next node if available
        {
            Debug.Log("triggered");

            Collider2D[] nodeCollider = Physics2D.OverlapCircleAll(searchPos.position, searchRange, nodeLayer);

            //keept checking nodes
            if(nodeCollider.Length > 0)
            {

                newNode _newNode = new newNode();
                

                List<newNode> nodeList = new List<newNode>();

                for (int i = 0; i < nodeCollider.Length; i++)
                {
                    float dist = Vector3.Distance(nodeCollider[i].transform.position, transform.position);

                    newNode node = new newNode();
                    node.distance = dist;
                    node.node = nodeCollider[i].gameObject;



                    nodeList.Add(node);



                        //newNode = i;


                     
                }

                Debug.Log(nodeList.Count());


                newNode closestNode = new newNode();
                float previousDist = 0;

                foreach(newNode n in nodeList)
                {
                    if(n.distance < closestNode.distance || closestNode.distance == 0)
                    {


                        closestNode = n;
                        Debug.Log("considered: " + closestNode.node + " dist: " + closestNode.distance);
                    }
                }



                target = closestNode.node;
                currentNode = closestNode.node;

                Debug.Log("selected: " + target.name);

                nodeList.Clear();


            }        
        }
    }

    private void ReportStatus()
    {
        if(hasResource)
        {
            statusBubble.SetActive(true);
        }
        else
        {
            statusBubble.SetActive(false);
        }
    }

    private void StopMovement()
    {
        //Always move towards target

        if (target.GetComponent<Unit>())
        {
            MoveTo(target.transform.position);
            inCombat = true;
        }

        //check to make sure target is not enemy and reset can move
        if (!target.GetComponent<Unit>() || (isHarvestingNode || hasResource))
        {
            canMove = true;
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
    }

    IEnumerator HarvestResource(GameObject node)
    {
        canFlip = false;
        yield return new WaitForSeconds(1.5f);
        canFlip = true;
        if(node)
        {
            if(target == node)
            {
                node.GetComponent<Node>().health -= 1;
                hasResource = true;
            }           
        }   
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(searchPos.position, searchRange);
    }
}
