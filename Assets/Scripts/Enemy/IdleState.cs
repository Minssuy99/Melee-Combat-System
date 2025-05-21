using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
    private EnemyController enemy;
    
    // 상태 진입 시 호출
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
    }
    
    // 상태 유지 중 매 프레임 호출
    public override void Execute()
    {
        foreach (var target in enemy.TargetsInRange)
        {
            // 대상까지의 방향 벡터 계산
            var vecToTarget = target.transform.position - transform.position;
            
            // 전방 방향과의 각도 계산
            float angle = Vector3.Angle(transform.forward, vecToTarget);
            
            // 시야각 내에 들어오면 타겟 지정 및 상태 전환
            if (angle <= enemy.Fov / 2)
            {
                enemy.Target = target;
                enemy.ChangeState(EnemyStates.CHASE); // 추적 상태로 전환
                break;
            }
        }
    }
    
    // 상태 종료 시 호출
    public override void Exit()
    {

    }
}