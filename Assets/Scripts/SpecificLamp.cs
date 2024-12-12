using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificLamp : MonoBehaviour
{
    [Header("Time Parameters")]
    [SerializeField] private float _lightSeconds = 2f;
    [SerializeField] private float _darkSeconds = 2f;
    [SerializeField] private bool _startWithLight = true;

    [Header("Visuals")]
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _shadow;

    [Header("Detection Parameters")]
    [SerializeField] private List<Collider2D> _triggerColliders;
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private HamsterMovement hamster;
    [SerializeField] private Transform _additionalAreaCheck;

    private WaitForSeconds _lightWait;
    private WaitForSeconds _darkWait;
    private bool _turnedOn;

    private void Start()
    {
        _lightWait = new WaitForSeconds(_lightSeconds);
        _darkWait = new WaitForSeconds(_darkSeconds);
        StartCoroutine(LightFlickering());

        foreach (var col in _triggerColliders)
            col.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            hamster.InSafeSpot = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !hamster.InSafeSpot)
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
        foreach (var col in _triggerColliders)
            col.enabled = !lightOn;

        if (_turnedOn && Vector2.Distance(transform.position, hamster.transform.position) <= checkRadius)
        {
            Debug.Log("Hamster VON");
            hamster.InSafeSpot = false;
        }
        else if(_turnedOn && Vector2.Distance(_additionalAreaCheck.position, hamster.transform.position) <= checkRadius)
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
