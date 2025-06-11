using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundToysConfigurator : MonoBehaviour
{
    [SerializeField] private List<LoudToy> _superLoudToys;
    [SerializeField] private List<EnemyHearingAI> _enemyHearingAIs;
    [SerializeField] private Canvas _EToInteract;
   void Start()
   {
        _EToInteract.gameObject.SetActive(false);
        foreach (var item in _superLoudToys)
        {
            item.WorldCanvas = _EToInteract;
            item.OnInteracted += HandleSuperLoudSound;
        }
   }
    private void HandleSuperLoudSound(Transform transform)
    {
        foreach(var enemyHearingAI in _enemyHearingAIs)
        {
            enemyHearingAI.OnTargetDetected?.Invoke(transform);
        }
    }
}
