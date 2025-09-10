using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackState : State<EnemyController>
{
    [SerializeField] private float attackDistance = 1f;

    private bool isAttacking;
    
    private EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = attackDistance;
    }

    public override void Execute()
    {
        if (isAttacking) return;
        
        enemy.NavAgent.SetDestination(enemy.Target.transform.position);

        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= attackDistance + 0.03f)
        {
            StartCoroutine(Attack(Random.Range(0,enemy.Fighter.Attacks.Count + 1)));
        }
    }

    IEnumerator Attack(int comboCount = 1)
    {
        isAttacking = true;
        enemy.Animator.applyRootMotion = true;
        
        enemy.Fighter.TryToAttack();

        for (int i = 0; i < comboCount; i++)
        {
            yield return new WaitUntil(() => enemy.Fighter.AttackState == AttackStates.COOLDOWN);
            enemy.Fighter.TryToAttack();
        }
        yield return new WaitUntil(() => enemy.Fighter.AttackState == AttackStates.IDLE);
        
        enemy.Animator.applyRootMotion = false;
        isAttacking = false;
        
        if (enemy.IsInState(EnemyStates.ATTACK))
            enemy.ChangeState(EnemyStates.RetreatAfterAttack);
    }

    public override void Exit()
    {
        enemy.NavAgent.ResetPath();
    }
}
