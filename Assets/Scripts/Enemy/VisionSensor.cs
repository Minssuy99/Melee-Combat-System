using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;

    // 트리거 콜라이더에 어떤 오브젝트가 들어왔을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        var fighter = other.GetComponent<MeleeFighter>();

        if (fighter != null)
            enemy.TargetsInRange.Add(fighter); // 적 범위에 들어온 대상 추가
    }

    // 트리거 콜라이더에서 오브젝트가 나갔을 때 호출
    private void OnTriggerExit(Collider other)
    {
        var fighter = other.GetComponent<MeleeFighter>();

        if (fighter != null)
            enemy.TargetsInRange.Remove(fighter); // 적 범위에서 벗어난 대상 제거
    }
}