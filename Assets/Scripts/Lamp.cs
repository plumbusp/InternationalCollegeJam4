using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    public Action<Collider2D> detectedTargetInShadow;
    public Action<Collider2D> detectedTargetInLight;
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
    List<string> detectedTargets = new List<string>();

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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "Player")
    //    {
    //        hamster.InSafeSpot = true;
    //        Debug.Log("Player in safe spot");
    //    }

    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.tag == "Player")
    //    {
    //        Debug.Log("Player NOT  safe spot");
    //        hamster.InSafeSpot = false;
    //    }

    //}

    private void Update()
    {
        if (_turnedOn)
            return;

        overlapedColliders = Physics2D.OverlapCircleAll(origin.position, checkRadius);
        foreach(Collider2D collider in overlapedColliders)
        {
            targetCheck = CheckForTargetTag(collider.tag);
            listContainsCheck = detectedTargets.Contains(collider.tag);
            if (targetCheck && !listContainsCheck)
            {
                detectedTargets.Add(collider.tag);
                detectedTargetInShadow?.Invoke(collider);
            }
            else if(!targetCheck && listContainsCheck)
            {
                detectedTargets.Remove(collider.tag);
                detectedTargetInLight?.Invoke(collider);
            }
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
            if (overlapedColliders == null || overlapedColliders.Length ==0)
                return;

            foreach (Collider2D collider in overlapedColliders)
            {
                if (CheckForTargetTag(collider.tag))
                {
                    detectedTargetInLight?.Invoke(collider);
                }
            }
        }
        //if (_turnedOn && Vector2.Distance(transform.position, hamster.transform.position) <= checkRadius)
        //{
        //    Debug.Log("Hamster VON");
        //    hamster.InSafeSpot = false;
        //}
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
