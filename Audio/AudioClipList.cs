using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AudioClipList", fileName = "new AudioClipList", order = 0)]
public class AudioClipList : ScriptableObject
{
    public List<AudioClip> EnvironmentSounds;
    public List<AudioClip> ImpactSounds;
    public List<AudioClip> VaccumSounds;
    public List<AudioClip> GeneratorSounds;
    public List<AudioClip> PlayerActionSounds;
    public List<AudioClip> CharacterLandingSounds;
    public List<AudioClip> FootstepSounds;
    public List<AudioClip> AmbientSounds;
    public List<AudioClip> MenuSounds;
}
