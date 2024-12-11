using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private bool _isActivated = false;
    private Transform _cheeseTransform;
    private void Update()
    {
        if (!_isActivated)
            return;
        transform.position = Vector2.MoveTowards(transform.position, _cheeseTransform.position, Time.deltaTime * _moveSpeed);
        transform.up = _cheeseTransform.position - transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Cheese")
        {
            _isActivated = false;
        }
    }
    public void Activate(Transform cheeseTransform)
    {
        _cheeseTransform = cheeseTransform;
        _isActivated = true;
    }
}
