using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MusicObject
{
    [field: SerializeField] public MusicType musicType { get; private set; }
    [field: SerializeField] public AudioClip clip { get; private set; }
}
