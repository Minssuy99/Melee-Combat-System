using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates {  // 적의 상태들을 열거형으로 정의
    IDLE,  // 대기 상태
    CHASE,  // 추격 상태cr
}

public class EnemyController : MonoBehaviour {
    // 상태 머신 프로퍼티 (EnemyController를 위한 상태 머신)
    public StateMachine<EnemyController> StateMachine { get; private set; }
    
    // 상태들을 저장하는 딕셔너리
    private Dictionary<EnemyStates, State<EnemyController>> stateDict;
    
    private void Start() {
        // 딕셔너리 초기화
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
        
        // 컴포넌트로 추가된 상태 클래스들을 가져와 딕셔너리에 저장
        stateDict[EnemyStates.IDLE] = GetComponent<IdleState>();
        stateDict[EnemyStates.CHASE] = GetComponent<ChaseState>();
        
        // 상태 머신 생성 및 초기 상태 설정
        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyStates.IDLE]);
    }
    
    // 상태 변경 메서드
    public void ChangeState(EnemyStates state) {
        StateMachine.ChangeState(stateDict[state]);
    }
    
    // 매 프레임마다 현재 상태의 Execute 메서드 실행
    private void Update() {
        StateMachine.Execute();
    }
}