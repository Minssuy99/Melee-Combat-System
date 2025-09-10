using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates // 적의 상태들을 열거형으로 정의
{ 
    IDLE,  // 대기 상태
    CombatMovement,  // 추격 상태
    ATTACK, // 공격 상태
    RetreatAfterAttack,
    Dead,
}

public class EnemyController : MonoBehaviour
{
    // 적이 볼 수 있는 시야각 (기본 180도)
    [field : SerializeField] public float Fov { get; private set; } = 180f;
    
    // 현재 시야 범위 안에 들어온 모든 대상 목록
    public List<MeleeFighter> TargetsInRange { get; set; } = new List<MeleeFighter>();
    
    // 실제로 쫓을 대상 1명을 저장
    public MeleeFighter Target { get; set; }

    public float CombatMovementTimer { get; set; } = 0f;
    
    // 상태 머신 프로퍼티 (EnemyController를 위한 상태 머신)
    public StateMachine<EnemyController> StateMachine { get; private set; }
    
    // 상태들을 저장하는 딕셔너리
    private Dictionary<EnemyStates, State<EnemyController>> stateDict;
    
    public NavMeshAgent NavAgent { get; private set; }

    public CharacterController CharacterController { get; private set; }
    
    public Animator Animator { get; private set; }
    
    public MeleeFighter Fighter { get; private set; }

    public VisionSensor VisionSensor { get; set; }

    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        Fighter = GetComponent<MeleeFighter>();
        
        // 딕셔너리 초기화
        stateDict = new Dictionary<EnemyStates, State<EnemyController>>();
        
        // 컴포넌트로 추가된 상태 클래스들을 가져와 딕셔너리에 저장
        stateDict[EnemyStates.IDLE] = GetComponent<IdleState>();
        stateDict[EnemyStates.CombatMovement] = GetComponent<CombatMovementState>();
        stateDict[EnemyStates.ATTACK] = GetComponent<AttackState>();
        stateDict[EnemyStates.RetreatAfterAttack] = GetComponent<RetreatAfterAttackState>();
        stateDict[EnemyStates.Dead] = GetComponent<DeadState>();

        // 상태 머신 생성 및 초기 상태 설정
        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyStates.IDLE]);
    }
    
    // 상태 변경 메서드
    public void ChangeState(EnemyStates state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }

    public bool IsInState(EnemyStates state)
    {
        return StateMachine.CurrentState == stateDict[state];
    }

    private Vector3 prevPos;
    
    // 매 프레임마다 현재 상태의 Execute 메서드 실행
    private void Update()
    {
        StateMachine.Execute();
        
        // 기존코드 : Vector3 deltaPos = transform.position - prevPos;
        Vector3 deltaPos = Animator.applyRootMotion? Vector3.zero : transform.position - prevPos;
        Vector3 velocity = deltaPos / Time.deltaTime;
        
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        Animator.SetFloat("forwardSpeed", forwardSpeed / NavAgent.speed, 0.2f, Time.deltaTime);
        
        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        Animator.SetFloat("strafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);
        
        prevPos = transform.position;
    }
}