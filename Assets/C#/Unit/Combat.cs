using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public int unitHealth;
    public int unitDamage;

    [SerializeField] private Transform searchPos;
    [SerializeField] private float searchRange;
    [SerializeField] private LayerMask unitLayer;

    private Unit unit;
    private bool inCombat;
    private GameObject targetUnit;

    [SerializeField] private float setAttackCooldown;
    private float attackCooldown;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
       

        attackCooldown = setAttackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(targetUnit.name);
        //if(unitHealth <= 0)
        //{
        //    Destroy(gameObject);
        //}

        //inCombat = unit.inCombat;

        //if (inCombat)
        //{
        //    targetUnit = unit.target;
      
        //    Collider2D[] unitCollider = Physics2D.OverlapCircleAll(searchPos.position, searchRange, unitLayer);
        //    foreach(Collider2D unit in unitCollider)
        //    {
        //        Unit _unit = gameObject.GetComponent<Unit>();
        //        if (_unit && _unit.isEnemy && !targetUnit.gameObject)
        //        {
        //            targetUnit = unit.gameObject;
                    
        //        } //handle if a healer for friendly adds
        //    }
              
        //    if(unitCollider.Length <= 0 && !unit.target)
        //    {
        //        targetUnit = null;
        //    }

        //    attackCooldown -= Time.deltaTime;

        //    if (targetUnit && attackCooldown <= 0) //Handle attack 
        //    {
        //        attackCooldown = setAttackCooldown;

        //        targetUnit.GetComponent<Unit>().inCombat = true;
        //        targetUnit.GetComponent<Combat>().unitHealth -= unitDamage;
        //    }


        //}
    }
}
