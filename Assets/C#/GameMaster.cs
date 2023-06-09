using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GameMaster : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private List<Unit> selectedUnitList;
    [SerializeField] private Transform selectedAreaTransform;

    public GameObject target;
    Collider2D targetCollider;

    private void Awake()
    {
        selectedUnitList = new List<Unit>();
        selectedAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        GetTarget();
        SelectUnits();
        CommandUnits();  
    }
    private void GetTarget()
    {
        if(Input.GetMouseButtonUp(1))
        {
            //Detect clicked game world object
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetCollider = Physics2D.OverlapPoint(clickPosition);
            var targetColliderMap = Physics2D.OverlapPoint(clickPosition);

            if (targetCollider != null)
            {
                target = targetCollider.gameObject;
                Debug.Log("clicked"  + targetCollider.gameObject.name);
            }
            else
            {
                target = null;
            }
        }
    }

    private void SelectUnits()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedAreaTransform.gameObject.SetActive(true);
            startPosition = UtilsClass.GetMouseWorldPosition();
        }

        if (Input.GetMouseButton(0))
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

        if (Input.GetMouseButtonUp(0))
        {
            selectedAreaTransform.gameObject.SetActive(false);

            Collider2D[] collider2dArray = Physics2D.OverlapAreaAll(startPosition, UtilsClass.GetMouseWorldPosition());

            selectedUnitList.Clear();

            foreach (Collider2D collider2D in collider2dArray)
            {
                Unit unit = collider2D.GetComponent<Unit>();
                if (unit != null)
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

    private void CommandUnits()
    {
        if (Input.GetMouseButtonUp(1))
        {
            //Basic movement
            Vector3 moveToPosition = UtilsClass.GetMouseWorldPosition();
            List<Vector3> targetPositionList = GetPositionListAround(moveToPosition, new float[] { 0.2f, 0.4f, 0.6f }, new int[] { 5, 10, 20 });
            int targetPositionListIndex = 0;

            Debug.Log(moveToPosition);

            foreach (Unit unit in selectedUnitList)
            {
                unit.MoveTo(targetPositionList[targetPositionListIndex]);
                targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;

                if(target != null)
                {
                    unit.target = target;
                }
                else
                {
                    unit.target = null;
                }
       
                
            }
        }
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
