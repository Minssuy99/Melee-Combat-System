using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);
    public static EnemyManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private List<EnemyController> enemiesInRange = new List<EnemyController>();
    private float notAttackingTimer = 2;

    public void AddEnemyInRange(EnemyController enemy)
    {
        // 리스트에 enemy 가 존재하면 true
        if (!enemiesInRange.Contains(enemy)) 
            enemiesInRange.Add(enemy);
    }

    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    private void Update()
    {
        if (enemiesInRange.Count == 0) return;
        
        if (!enemiesInRange.Any(e => e.IsInState(EnemyStates.ATTACK)))
        {
            if (notAttackingTimer > 0)
                notAttackingTimer -= Time.deltaTime;

            if (notAttackingTimer <= 0)
            {
                var attackingEnemy = SelectEnemyForAttack();
                attackingEnemy.ChangeState(EnemyStates.ATTACK);
                notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x, timeRangeBetweenAttacks.y);
            }
        }
    }

    EnemyController SelectEnemyForAttack()
    {
        return enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
    }
}
