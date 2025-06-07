using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeafCat : MonoBehaviour
{
    // Detect()
    // Ondetected get and set distanation to it
    // if lost patrol
    // if patrolled enought return
    // 

    [SerializeField] private IEnemyPerceptionAI enemyVisionAI;
    [SerializeField] private EnemyParameters deafCatParameters;
    //Enemy movement script 
    private void Start()
    {
        enemyVisionAI.Initialize(deafCatParameters);
        enemyVisionAI.OnTargetDetected += HandleTargetDetection;
        //Subsribe on enemy catched player
    }

    private void Update()
    {
        //Move enemy according to detection reesults
    }
    private void LateUpdate()
    {
        enemyVisionAI.Detect();
    }

    private void HandleTargetDetection()
    {
        //Switch to chasing
        //
    }
}
