using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController> {
    // 상태 진입 시 호출
    public override void Enter(EnemyController owner) {
        Debug.Log("Entered Chase State");
    }
    
    // 상태 유지 중 매 프레임 호출
    public override void Execute() {
        Debug.Log("Executing Chase State");
    }
    
    // 상태 종료 시 호출
    public override void Exit() {
        Debug.Log("Exiting Chase State");
    }
}