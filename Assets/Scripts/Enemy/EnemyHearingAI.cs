using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHearingAI : EnemyPerceptionAI
{
    public override Action<Transform> OnTargetDetected { get => _onTargetDetected; set => _onTargetDetected = value; }
    public override Action OnTargetLost { get => _onTargetLost; set => _onTargetLost = value; }
    public override Action<Transform> OnTargetCanBeKilled { get => _onTargetCanBeKilled; set => _onTargetCanBeKilled = value; }

    private bool _isAllowedToDetect;
    public override bool isAllowedToDetect
    {
        get => _isAllowedToDetect;
        set => _isAllowedToDetect = false;
    }

    [SerializeField] private float _closeHearingRadius;
    [SerializeField] private float _timeOnInvestigation;
    private WaitForSeconds wait;

    private int _currentSoundPriority;
    private int _nonePriority = 0;
    private int _lowPriority = 1;
    private int _normalPriority = 2;
    private int _urgentPriority = 3;


    public override void Detect()
    {
        if (_isAllowedToDetect)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _closeHearingRadius);
            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (CheckForTargetTag(collider.tag))
                    {
                        _onTargetCanBeKilled?.Invoke(collider.transform);
                    }
                }
            }
        }
    }
    public void HandleSuperLoudSound(Transform target)
    {
        if (!CheckSoundPriority(_urgentPriority))
            return;

        _currentSoundPriority = _lowPriority; // Reset to low priority after chasing the sound source
        OnTargetDetected?.Invoke(target);
        Debug.Log("Now what was THAT ?!!");
    }

    public void HandleLoudSound(Transform target)
    {
        if (!CheckSoundPriority(_normalPriority))
            return;

        OnTargetDetected?.Invoke(target);
        _currentSoundPriority = _lowPriority; // Reset to low priority after chasing the sound source
        Debug.Log("Going to investigate loud sound ");
    }

    /// <summary>
    /// Checks whether the new sound has a higher priority than the current one.
    /// </summary>
    /// <param name="newSoundPriority"></param>
    /// <returns></returns>
    private bool CheckSoundPriority(int newSoundPriority)
    {
        if (_currentSoundPriority <= newSoundPriority)
        {
            _currentSoundPriority = newSoundPriority;
            return true;
        }
        return false;
    }
}
