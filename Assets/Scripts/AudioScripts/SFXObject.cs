using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SFXObject
{
    [field: SerializeField] public SFXType sfxType { get; private set; }
    [field: SerializeField] public AudioClip clip { get; private set; }
}
