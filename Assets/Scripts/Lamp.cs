using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    public Action<IEnemyTarget> newTargetFound;
    public Action<IEnemyTarget> targetLost;
    [Header("Time Parameters")]
    [SerializeField] private float _lightSeconds = 2f;
    [SerializeField] private float _darkSeconds = 2f;
    [SerializeField] private bool _startWithLight = true;

    [Header("Visuals")]
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _shadow;

    [Header("Detection Parameters")]
    [SerializeField] private Collider2D _triggerCollider;
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private Transform origin;
    [SerializeField] private HamsterMovement hamster;

    private WaitForSeconds _lightWait;
    private WaitForSeconds _darkWait;
    private bool _turnedOn;
    private bool targetCheck;
    private bool listContainsCheck;

    Collider2D[] overlapedColliders;
    public HashSet<IEnemyTarget> currentTargetsInLamp = new HashSet<IEnemyTarget>();
    HashSet<IEnemyTarget> targetsToDelete;

    private EnemyParameters enemyParameters;

    private void Start()
    {
        _lightWait = new WaitForSeconds(_lightSeconds);
        _darkWait = new WaitForSeconds(_darkSeconds);
        StartCoroutine(LightFlickering());

        if (_triggerCollider != null)
            _triggerCollider.enabled = false;

    }
    public void Initialize(EnemyParameters enemyParameters)
    {
        this.enemyParameters = enemyParameters;
    }

    private void Update()
    {
        if (_turnedOn)
            return;

        overlapedColliders = Physics2D.OverlapCircleAll(origin.position, checkRadius);
        HashSet<IEnemyTarget> detectedTargets = new HashSet<IEnemyTarget>();

        foreach(Collider2D collider in overlapedColliders)
        {
            if (CheckForTargetTag(collider.tag))
            {
                IEnemyTarget newTarget = collider.GetComponent<IEnemyTarget>();
                detectedTargets.Add(newTarget);
                if (!currentTargetsInLamp.Contains(newTarget))
                {
                    currentTargetsInLamp.Add(newTarget);
                    newTargetFound.Invoke(newTarget);
                }
            }
        }

        targetsToDelete = new HashSet<IEnemyTarget>();

        foreach (IEnemyTarget target in currentTargetsInLamp)
        {
            if(!detectedTargets.Contains(target))
            {
                targetsToDelete.Add(target);
            }
        }
        foreach (IEnemyTarget target in targetsToDelete)
        {
            currentTargetsInLamp.Remove(target);
            targetLost?.Invoke(target);
        }
    }

    private IEnumerator LightFlickering()
    {
        while (true)
        {
            SetLightState(_startWithLight);
            yield return _startWithLight ? _lightWait : _darkWait;

            _startWithLight = !_startWithLight;
        }
    }

    private void SetLightState(bool lightOn)
    {
        //AudioManager.instance.PlayAudio(SFXType.LampOn);
        _turnedOn = lightOn;
        _light.SetActive(lightOn);
        _shadow.SetActive(!lightOn);
        if (_triggerCollider == null)
            return;

        _triggerCollider.enabled = !lightOn;
        if (lightOn)
        {
            if (overlapedColliders == null || overlapedColliders.Length == 0)
                return;

            targetsToDelete = new HashSet<IEnemyTarget>(currentTargetsInLamp);

            foreach (IEnemyTarget target in targetsToDelete)
            {
                currentTargetsInLamp.Remove(target);
                targetLost?.Invoke(target);
            }
        }
    }
    private bool CheckForTargetTag(string tagName)
    {
        if (enemyParameters.DetectionTags.Contains(tagName))
            return true;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
