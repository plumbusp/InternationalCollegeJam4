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

    private void HandleTargetInShadow(Collider2D collider)
    {
        collider.GetComponent<IEnemyTarget>().InSafeSpot = true;
        Debug.Log($"HandleTargetInShadow {collider.tag}");
    }

    private void HandleTargetInLight(Collider2D collider)
    {
        collider.GetComponent<IEnemyTarget>().InSafeSpot = false;
        Debug.Log($"HandleTargetInLight {collider.tag}");
    }
}
