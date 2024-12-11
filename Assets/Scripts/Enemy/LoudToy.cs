using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoudToy : Interactable, ISoundMaker
{
    public event Action<Vector3> OnLoudSoundMade;
    public event Action<Vector3> OnQuiteSoundMade;
    override public void Interact()
    {
        base.Interact();
        OnLoudSoundMade?.Invoke(transform.position);
        Debug.Log("BOOM on " + transform.position);
    }
}
