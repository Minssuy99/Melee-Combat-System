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
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;
    
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
            leftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            leftFootCollider = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();

            DisableAllHitboxes();
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
                    EnableHitbox(attacks[comboCount]);
                }
            }
            else if (attackState == AttackState.IMPACT)
            {
                if (nomalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    attackState = AttackState.COOLDOWN;
                    DisableAllHitboxes();
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

    void EnableHitbox(AttackData attack)
    {
        switch (attack.HitboxToUse)
        {
            case AttackHitbox.LEFTHAND:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RIGHTHAND:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.LEFTFOOT:
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RIGHTFOOT:
                rightFootCollider.enabled = true;
                break;
            case AttackHitbox.SWORD:
                swordCollider.enabled = true;
                break;
            default:
                break;
        }
    }

    void DisableAllHitboxes()
    {
        swordCollider.enabled = false;
        leftHandCollider.enabled = false;
        rightHandCollider.enabled = false;
        leftFootCollider.enabled = false;
        rightFootCollider.enabled = false;
    }
}
