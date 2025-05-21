using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{  
    public State<T> CurrentState { get; private set; } // 현재 상태 저장
    private T _owner;
    
    public StateMachine(T owner)
    {
        _owner = owner;
    }

    public void ChangeState(State<T> newState) // 상태 변경하는 메서드
    {
        CurrentState?.Exit();  // 현재 상태가 있으면 Exit 호출
        CurrentState = newState;  // 새 상태로 변경
        CurrentState.Enter(_owner);  // 새 상태의 Enter 호출
    }

    public void Execute()
    {
        CurrentState?.Execute();  // 현재 상태의 Execute 호출
    }
}