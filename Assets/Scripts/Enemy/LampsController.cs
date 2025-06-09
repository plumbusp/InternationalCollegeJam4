using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampsController : MonoBehaviour
{
    [SerializeField] private List<Lamp> lamps;
    [SerializeField] private EnemyParameters enemyParameters;

    private void Start()
    {
        foreach (var lamp in lamps)
        {
            lamp.Initialize(enemyParameters);
            lamp.detectedTargetInShadow += HandleTargetInShadow;
            lamp.detectedTargetInLight += HandleTargetInLight;
        }
    }

    private void HandleTargetInShadow(IEnemyTarget enemyTarget)
    {
        enemyTarget.InSafeSpot = true;
    }

    private void HandleTargetInLight(IEnemyTarget collider)
    {
        collider.InSafeSpot = false;
    }
}
