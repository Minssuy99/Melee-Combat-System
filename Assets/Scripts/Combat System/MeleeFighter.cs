using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState
{
    IDLE,
    WINDUP,
    IMPACT,
    COOLDOWN,
}

public class MeleeFighter : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    
    BoxCollider swordCollider;
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (sword != null)
        {
            swordCollider = sword.GetComponent<BoxCollider>();
            swordCollider.enabled = false;
        }
    }

    AttackState attackState;

    public bool InAction { get; private set; } = false;
    
    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        attackState = AttackState.WINDUP;

        float impactStartTime = 0.33f;
        float impactEndTime = 0.55f;
        
        animator.CrossFade("Slash", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;

        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float nomalizedTime = timer / animState.length;

            if (attackState == AttackState.WINDUP)
            {
                if (nomalizedTime >=impactStartTime)
                {
                    attackState = AttackState.IMPACT;
                    swordCollider.enabled = true;
                }
            }
            else if (attackState == AttackState.IMPACT)
            {
                if (nomalizedTime >= impactEndTime)
                {
                    attackState = AttackState.COOLDOWN;
                    swordCollider.enabled = false;
                }
            }
            else if (attackState == AttackState.COOLDOWN)
            {
                // TODO : Handle combos
            }
            
            yield return null;
        }

        attackState = AttackState.IDLE;
        
        InAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox" && !InAction)
        {
            StartCoroutine(PlayerHitReaction());
        }
    }
    
    IEnumerator PlayerHitReaction()
    {
        InAction = true;
        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);
        
        yield return new WaitForSeconds(animState.length);
        
        InAction = false;
    }
}
