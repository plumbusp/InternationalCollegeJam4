using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable: MonoBehaviour
{
    public Action<Transform> OnInteracted;
    [SerializeField] private Transform _canvasSpawnPoint;
    private bool _isWaitingForInput = false;
    public Canvas WorldCanvas { get; set; }

    protected void Update()
    {
        if (!_isWaitingForInput)
            return;

        if (Input.GetKeyUp(KeyCode.E))
        {
            Interact();
            DeactivateInstruction();
        }
        WorldCanvas.transform.position = _canvasSpawnPoint.position;
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _isWaitingForInput = true;
            ActivateInstruction();
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _isWaitingForInput = false;
            DeactivateInstruction();
        }
    }
    virtual protected void ActivateInstruction() 
    {
        WorldCanvas.gameObject.SetActive(true);
    }
    virtual protected void DeactivateInstruction()
    {
        WorldCanvas.gameObject.SetActive(false);
    }
    virtual protected void Interact()
    {
        OnInteracted?.Invoke(transform);
        _isWaitingForInput = false;
    }
}
