using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lamp : MonoBehaviour
{
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
    [SerializeField] private HamsterMovement hamster;

    private WaitForSeconds _lightWait;
    private WaitForSeconds _darkWait;
    private bool _turnedOn;

    private void Start()
    {
        _lightWait = new WaitForSeconds(_lightSeconds);
        _darkWait = new WaitForSeconds(_darkSeconds);
        _triggerCollider.enabled = false;

        StartCoroutine(LightFlickering());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            hamster.InSafeSpot = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            hamster.InSafeSpot = false;
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
        _turnedOn = lightOn;
        _light.SetActive(lightOn);
        _shadow.SetActive(!lightOn);
        _triggerCollider.enabled = !lightOn;

        if (_turnedOn && Vector2.Distance(transform.position, hamster.transform.position) <= checkRadius)
        {
            Debug.Log("Hamster VON");
            hamster.InSafeSpot = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
