using System;
using UnityEngine;

public class LoudToy : Interactable, ISoundMaker
{
    override protected void Interact()
    {
        base.Interact();
        //AudioManager.instance.PlayAudio(SFXType.Toy);
        Debug.Log("BOOM on " + transform.position);
    }
}
