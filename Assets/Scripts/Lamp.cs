using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [Header("Time Parameters")]
    [SerializeField] private float _lightSeconds;
    private WaitForSeconds _lightWait;
    [SerializeField] private float _darkSeconds;
    private WaitForSeconds _darkWait;
    [SerializeField] private bool _startWithLight;

    [Space(8f)]
    [Header("Visuals")]
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _shadow;

    [Space(8f)]
    [Header("Shadow parameters")]
    [SerializeField] private Transform _middlePoint;
    [SerializeField] private float radius;
    [SerializeField] private HamsterMovement hamster;


    private bool _turnedOn;
    private void Start()
    {
        _lightWait = new WaitForSeconds(_lightSeconds);
        _darkWait = new WaitForSeconds(_darkSeconds);

        StartCoroutine(LightFlickering());
    }

    private void Update()
    {
        if (_turnedOn)
        {
            if (hamster.InSafeSpot)
            {
                hamster.InSafeSpot = false;
            }
            return;
        }

        bool playerDetected = Physics2D.OverlapCircleAll(_middlePoint.position, radius)
                              .Any(collider => collider.tag =="Player");
        hamster.InSafeSpot = playerDetected;
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

    private void SetLightState(bool lightsOn)
    {
        _turnedOn = lightsOn;
        _light.SetActive(lightsOn);
        _shadow.SetActive(!lightsOn);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_middlePoint.position, radius);
        Gizmos.color = Color.green;
    }
}
