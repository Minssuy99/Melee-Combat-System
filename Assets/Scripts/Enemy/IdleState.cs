using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController> {
    private EnemyController enemy;
    
    // 상태 진입 시 호출
    public override void Enter(EnemyController owner) {
        enemy = owner;
        Debug.Log("Entered Idle State");
    }
    
    // 상태 유지 중 매 프레임 호출
    public override void Execute() {
        Debug.Log("Executing Idle State");
        // T 키를 누르면 CHASE 상태로 전환
        if (Input.GetKeyDown(KeyCode.T))
            enemy.ChangeState(EnemyStates.CHASE);
    }
    
    // 상태 종료 시 호출
    public override void Exit() {
        Debug.Log("Exiting Idle State");
    }
}