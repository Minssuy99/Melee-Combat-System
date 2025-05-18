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
    [SerializeField] private List<AttackData> attacks;
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
    bool doCombo;
    int comboCount = 0;

    public bool InAction { get; private set; } = false;
    
    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
        else if (attackState == AttackState.IMPACT || attackState == AttackState.COOLDOWN)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        attackState = AttackState.WINDUP;
        
        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;

        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float nomalizedTime = timer / animState.length;

            if (attackState == AttackState.WINDUP)
            {
                if (nomalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    attackState = AttackState.IMPACT;
                    swordCollider.enabled = true;
                }
            }
            else if (attackState == AttackState.IMPACT)
            {
                if (nomalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    attackState = AttackState.COOLDOWN;
                    swordCollider.enabled = false;
                }
            }
            else if (attackState == AttackState.COOLDOWN)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;

                    StartCoroutine(Attack());
                    yield break;
                }
            }
            
            yield return null;
        }

        attackState = AttackState.IDLE;
        comboCount = 0;
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
        
        yield return new WaitForSeconds(animState.length * 0.8f);
        
        InAction = false;
    }
}
