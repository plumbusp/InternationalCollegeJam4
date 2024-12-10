using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundMaker
{
    public event Action<Vector3> OnLoudSoundMade;
    public event Action<Vector3> OnQuiteSoundMade;

}
