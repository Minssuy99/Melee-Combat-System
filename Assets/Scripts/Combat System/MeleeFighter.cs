using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStates
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

    public AttackStates AttackState { get; private set; }
    bool doCombo;
    int comboCount = 0;

    public bool InAction { get; private set; } = false;
    
    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
        else if (AttackState == AttackStates.IMPACT || AttackState == AttackStates.COOLDOWN)
        {
            doCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true;
        AttackState = AttackStates.WINDUP;
        
        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;

        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float nomalizedTime = timer / animState.length;

            if (AttackState == AttackStates.WINDUP)
            {
                if (nomalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    AttackState = AttackStates.IMPACT;
                    EnableHitbox(attacks[comboCount]);
                }
            }
            else if (AttackState == AttackStates.IMPACT)
            {
                if (nomalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    AttackState = AttackStates.COOLDOWN;
                    DisableAllHitboxes();
                }
            }
            else if (AttackState == AttackStates.COOLDOWN)
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

        AttackState = AttackStates.IDLE;
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
        if(swordCollider != null)
            swordCollider.enabled = false;  
        
        if(leftHandCollider != null)
            leftHandCollider.enabled = false;
        
        if(rightHandCollider != null)
            rightHandCollider.enabled = false;
        
        if(leftFootCollider != null)
            leftFootCollider.enabled = false;
        
        if(rightFootCollider != null)
            rightFootCollider.enabled = false;
    }
    
    public List<AttackData> Attacks => attacks;
}
