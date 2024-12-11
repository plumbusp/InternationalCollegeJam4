using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHole : MonoBehaviour
{
    [SerializeField] private Mouse _mouse;
    [SerializeField] private float _timeBeforeGo;
    private WaitForSeconds _waitBeforeGo;
    private bool _mouseIfGone = false;
    private Transform _cheeseTransform;

    private void Start()
    {
        _waitBeforeGo = new WaitForSeconds(_timeBeforeGo);
        _mouse.gameObject.SetActive(false);
        _mouse.transform.position = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_mouseIfGone)
            return;

        if(collision.tag == "Cheese")
        {
            _cheeseTransform = collision.transform;
            _mouseIfGone = true;
            StartCoroutine(CountBeforeGo());
        }
    }
    private IEnumerator CountBeforeGo()
    {
        yield return _waitBeforeGo;
        _mouse.gameObject.SetActive(true);
        _mouse.Activate(_cheeseTransform);
    }
}
