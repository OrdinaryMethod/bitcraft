using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.EventSystems;

public class GameMaster : MonoBehaviour
{
    public GameMasterData gameMasterData;
    private CameraController camera;

    private Vector3 startPosition;
    public List<Unit> selectedUnitList;
    [SerializeField] private Transform selectedAreaTransform;

    public GameObject target;
    Collider2D targetCollider;

    public int lumberTotal;

    public TextAlignment phaseDisplayText;
    private Touch theTouch;
    private float timeTouchEnded;

    private float previousTouchDistance;

    //Base 
    public bool BuildingSelected;

    //Interface

    private void Awake()
    {
        selectedUnitList = new List<Unit>();
        selectedAreaTransform.gameObject.SetActive(false);
        camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Update()
    {
        //SelectUnits_PC();
        SelectUnits_Mobile();
        CommandUnits_Mobile();
      

    }



    private void SelectUnits_PC()
    {
        if (Input.GetMouseButtonDown(1))
        {
            selectedAreaTransform.gameObject.SetActive(true);
            startPosition = UtilsClass.GetMouseWorldPosition();
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 currentMousePos = UtilsClass.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector3(
                    Mathf.Min(startPosition.x, currentMousePos.x),
                    Mathf.Min(startPosition.y, currentMousePos.y)
                );

            Vector3 upperRight = new Vector3(
                    Mathf.Max(startPosition.x, currentMousePos.x),
                    Mathf.Max(startPosition.y, currentMousePos.y)
                );

            selectedAreaTransform.position = lowerLeft;
            selectedAreaTransform.localScale = upperRight - lowerLeft;
        }

        if (Input.GetMouseButtonUp(1))
        {
            selectedAreaTransform.gameObject.SetActive(false);

            Collider2D[] collider2dArray = Physics2D.OverlapAreaAll(startPosition, UtilsClass.GetMouseWorldPosition());

            selectedUnitList.Clear();

            foreach (Collider2D collider2D in collider2dArray)
            {
                Unit unit = collider2D.GetComponent<Unit>();
                if (unit != null && !unit.isEnemy)
                {
                    selectedUnitList.Add(unit);


                }
            }
        }

        foreach (Unit unit in selectedUnitList)
        {
            unit.GetComponentInChildren<SelectionArea>().isSelected = true;
        }
    }
    private void SelectUnits_Mobile()
    {

        if (Input.touchCount == 2)
        {
            camera.canMove = false;
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

           float scaleSpeed = 1f;

            if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began)
            {
                selectedAreaTransform.gameObject.SetActive(true);
            }
 
            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                float getDistance = Vector2.Distance(touch1.position, touch2.position);

                if (getDistance > previousTouchDistance)
                {
                    if(selectedAreaTransform.localScale.x < 20)
                    {
                        selectedAreaTransform.localScale = new Vector2(
                             selectedAreaTransform.localScale.x + 1.5f,
                             selectedAreaTransform.localScale.y + 1.5f
                         );
                    }              
                }
                else if (getDistance < previousTouchDistance)
                {
                    if (selectedAreaTransform.localScale.x > 0)
                    {
                        selectedAreaTransform.localScale = new Vector2(
                             selectedAreaTransform.localScale.x - 1.5f,
                             selectedAreaTransform.localScale.y - 1.5f
                         );
                    }
                }

                previousTouchDistance = getDistance;



                Vector2 middlePosition = (touch1.position + touch2.position) / 2f;

                // Convert the screen position to world position
                Vector3 middlePositionWorld = Camera.main.ScreenToWorldPoint(new Vector3(middlePosition.x, middlePosition.y, 10f));

                // Set the position of the selected area transform
                selectedAreaTransform.position = new Vector3(middlePositionWorld.x, middlePositionWorld.y, 0f);

            }

            if (touch1.phase == TouchPhase.Ended && touch2.phase == TouchPhase.Ended)
            {             
                Debug.Log("Both fingers released");
                // Check for units within the area
                Collider2D[] colliders = Physics2D.OverlapAreaAll(selectedAreaTransform.position - selectedAreaTransform.localScale / 20f, selectedAreaTransform.position + selectedAreaTransform.localScale / 20f);
                
                foreach(Unit o in selectedUnitList)
                {
                    o.isSelected = false;
                }
                
                
                selectedUnitList.Clear();

                foreach (Collider2D collider in colliders)
                {
                    Unit unit = collider.GetComponent<Unit>();

                    //Units
                    if (unit != null && !unit.isEnemy)
                    {
                        if(selectedAreaTransform.gameObject.active)
                        {
                            selectedUnitList.Add(unit);
                            unit.GetComponent<Unit>().isSelected = true;
                        }
                    }
                }

                selectedAreaTransform.gameObject.SetActive(false);
                selectedAreaTransform.localScale = new Vector2(0, 0);
            }
        }
        else
        {
            selectedAreaTransform.gameObject.SetActive(false);
            selectedAreaTransform.localScale = new Vector3(1, 1);

            camera.canMove = true;
        }
    }
    private void CommandUnits_Mobile()
    {
        if (Input.touchCount == 1)
        {
            Touch touch1 = Input.GetTouch(0);

            if(!IsTouchOverUITag(touch1.position, "Interface"))
            {
                //Detect clicked game world object
                Vector2 clickPosition = Camera.main.ScreenToWorldPoint(touch1.position);
                targetCollider = Physics2D.OverlapPoint(clickPosition);
                var targetColliderMap = Physics2D.OverlapPoint(clickPosition);

                //Set target
                if (targetCollider != null)
                {
                    target = targetCollider.gameObject;
                    Debug.Log("You click on: " + targetCollider.gameObject.name);
                }
                else
                {
                    target = null;
                }

                //Basic movement
                Vector3 moveToPosition = UtilsClass.GetMouseWorldPosition();
                List<Vector3> targetPositionList = GetPositionListAround(moveToPosition, new float[] { 0.2f, 0.4f, 0.6f }, new int[] { 5, 10, 20 });
                int targetPositionListIndex = 0;

                foreach (Unit unit in selectedUnitList)
                {
                    if (target) //no formation
                    {
                        unit.MoveTo(moveToPosition);
                    }
                    else //formation
                    {
                        unit.MoveTo(targetPositionList[targetPositionListIndex]);
                        targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
                    }


                    if (target)
                    {
                        unit.target = target;

                        if (target.tag == "Node")
                        {
                            unit.isHarvestingNode = true;
                            //unit.isSelected = false;

                        }
                    }
                    else
                    {
                        unit.target = null;
                        unit.isHarvestingNode = false;
                    }
                }
            }
        }
    }

    private bool IsTouchOverUITag(Vector2 touchPosition, string tag)
    {
        // Create a PointerEventData to represent the touch input
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        // Create a list to store the results of the raycast
        List<RaycastResult> results = new List<RaycastResult>(); // Define and initialize the results list

        // Perform a raycast into the screen from the touch position
        EventSystem.current.RaycastAll(eventData, results);

        // Check if any UI objects were hit and have the specified tag
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }


    //Unit coordination
    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }

}
