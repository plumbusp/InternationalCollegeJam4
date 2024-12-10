using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectsConfigurator : MonoBehaviour
{
    [SerializeField] private List<Interactable> _interactableItems;
    [SerializeField] private Canvas _EToInteract;
   void Start()
    {
        _EToInteract.gameObject.SetActive(false);
        foreach (var item in _interactableItems)
        {
            item.WorldCanvas = _EToInteract;
        }
    }
}
