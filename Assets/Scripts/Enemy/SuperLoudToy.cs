using System;
using UnityEngine;

public class SuperLoudToy : Interactable, ISoundMaker
{
    public event Action<Vector3> OnLoudSoundMade;
    public event Action<Vector3> OnQuiteSoundMade;
    public event Action<Vector3> OnSuperLoudSoundMade;

    override public void Interact()
    {
        base.Interact();
        AudioManager.instance.PlayAudio(SFXType.Toy);
        OnSuperLoudSoundMade?.Invoke(transform.position);
        Debug.Log("BOOM on " + transform.position);
    }
}
