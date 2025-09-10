using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    MeleeFighter meleeFighter;
    Animator animator;

    private void Awake()
    {
        meleeFighter = GetComponent<MeleeFighter>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            var enemy = EnemyManager.i.GetAttackinEnemy();
            if (enemy != null && enemy.Fighter.IsCounterable && !meleeFighter.InAction)
            {
                StartCoroutine(meleeFighter.PerformCounterAttack(enemy));
            }
            else
            {
                meleeFighter.TryToAttack();
            }
        }
    }

    private void OnAnimatorMove()
    {
        if (!meleeFighter.InCounter)
            transform.position += animator.deltaPosition;

        transform.rotation *= animator.deltaRotation;
    }
}
