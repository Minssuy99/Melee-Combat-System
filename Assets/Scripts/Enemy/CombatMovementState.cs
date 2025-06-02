using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum AICombatStates
{
    IDLE,
    CHASE,
    Circling,
}


public class CombatMovementState : State<EnemyController>
{
    [SerializeField] private float circlingSpeed = 20f;
    [SerializeField] private float distanceToStand = 3f;
    [SerializeField] private float adjustDistanceThreshold = 1f;
    [SerializeField] Vector2 idleTimeRange = new Vector2(2, 5);
    [SerializeField] Vector2 circlingTimeRange = new Vector2(3, 6);

    private AICombatStates state;
    private EnemyController enemy;

    private float timer = 0f;
    private int circlingDir = 1;
    
    // 상태 진입 시 호출
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        
        // 추적 멈출 거리 설정
        enemy.NavAgent.stoppingDistance = distanceToStand;
    }
    
    // 상태 유지 중 매 프레임 호출
    public override void Execute()
    {
        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > distanceToStand + adjustDistanceThreshold)
        {
            StartChase();
        }
        
        if (state == AICombatStates.IDLE)
        {
            if (timer <= 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    StartIdle();
                }
                else
                {
                    StartCircling();
                }
            }
        }
        else if (state == AICombatStates.CHASE)
        {
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStand + 0.03f)
            {
                StartIdle();
                return;
            }
                
            // 대상 위치로 이동 명령
            enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        else if (state == AICombatStates.Circling)
        {
            if (timer <= 0)
            {
                StartIdle();
                return;
            }
            Vector3 vecToTarget = enemy.transform.position - enemy.Target.transform.position;
            Vector3 rotatedPos = Quaternion.Euler(0, circlingSpeed * circlingDir * Time.deltaTime, 0) * vecToTarget;
            enemy.NavAgent.Move(rotatedPos - vecToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    void StartIdle()
    {
        state = AICombatStates.IDLE;
        timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
        
        enemy.Animator.SetBool("combatMode", true);
    }
    
    void StartChase()
    {
        state = AICombatStates.CHASE;
        enemy.Animator.SetBool("combatMode", false);
    }

    void StartCircling()
    {
        state = AICombatStates.Circling;

        enemy.NavAgent.ResetPath();
        timer = Random.Range(circlingTimeRange.x, circlingTimeRange.y);

        circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
    }
    
    // 상태 종료 시 호출
    public override void Exit()
    {

    }
}