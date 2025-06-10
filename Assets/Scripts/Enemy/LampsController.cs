using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampsController : MonoBehaviour
{
    [SerializeField] private List<Lamp> lamps;
    [SerializeField] private EnemyParameters enemyParameters;
    public HashSet<IEnemyTarget> currentEnemyTargets = new HashSet<IEnemyTarget>();
    private HashSet<IEnemyTarget> targetsToDelete = new HashSet<IEnemyTarget>();

    private void Start()
    {
        foreach (var lamp in lamps)
        {
            lamp.Initialize(enemyParameters);
            lamp.newTargetFound += HandleTargetFound;
            lamp.targetLost += HandleTargetLost;
        }
    }

    private void HandleTargetFound(IEnemyTarget enemyTarget)
    {
        if(!currentEnemyTargets.Contains(enemyTarget))
        {
            currentEnemyTargets.Add(enemyTarget);
        }

        enemyTarget.InSafeSpot = true;
    }

    private void HandleTargetLost(IEnemyTarget enemyTarget)
    {
        int count = 0;
        foreach(var lamp in lamps)
        {
            if(lamp.currentTargetsInLamp.Contains(enemyTarget))
            {
                count++;
            }
        }
        if(count == 0)
            enemyTarget.InSafeSpot = false;
    }
}
