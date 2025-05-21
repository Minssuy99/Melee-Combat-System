using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    [SerializeField] private float distanceToStand = 3f;
    private EnemyController enemy;
    
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
        // 대상 위치로 이동 명령
        enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        
        // 이동 속도에 따른 애니메이션 제어
        enemy.Animator.SetFloat("moveAmount", enemy.NavAgent.velocity.magnitude / enemy.NavAgent.speed);
    }
    
    // 상태 종료 시 호출
    public override void Exit()
    {

    }
}