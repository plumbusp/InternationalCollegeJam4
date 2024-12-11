using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperLoudToy : Interactable, ISoundMaker
{
    public event Action<Vector3> OnLoudSoundMade;
    public event Action<Vector3> OnQuiteSoundMade;
    public event Action<Vector3> OnSuperLoudSoundMade;

    override public void Interact()
    {
        base.Interact();
        OnSuperLoudSoundMade?.Invoke(transform.position);
        Debug.Log("BOOM on " + transform.position);
    }
}
