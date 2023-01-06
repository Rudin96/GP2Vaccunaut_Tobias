using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Team14;
using UnityEngine.SceneManagement;

public enum AudioType
{
    SFX,
    Background,
    Music
}

public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;
    public AudioClipList GameSounds;

    public static AudioManager Instance;

    [Header("Music Vol")]
    [Range(0f, 1f)] public float Music;

    [Header("Master Vol")]
    [Range(0f, 1f)] public float Master;

    [Header("SFX Vol")]
    [Range(0f, 1f)] public float SFX;

    [Header("Background Vol")]
    [Range(0f, 1f)] public float Background;

    [Header("Ambience")]
    public bool ambFade = true;
    public AudioMixerGroup AmbienceGroup;
    [Range(0f, 6f)] public float ambFadeDuration = 5f;
    [Range(0f, 1f)] public float ambFadeInSpeed = .5f;
    [Range(0f, 1f)] public float ambFadeOutSpeed = .5f;

    private bool isPlayingNoOverlap = false;

    private void Start()
    {
        InitSoundVolumes();
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        InitAmbientSounds();
    }

    private void InitAmbientSounds()
    {
        List<AudioClip> sounds = GameSounds.AmbientSounds;
        int rng = Random.Range(0, sounds.Count);
        if (sounds.Count == 0) return;
        PlayAudioWithFade(sounds[rng], AudioType.Background);
    }

    private IEnumerator FadeCheckSongInList(AudioSource audioSrc, AudioClip clipFadeTo)
    {
        while (audioSrc.time < audioSrc.clip.length - ambFadeDuration / 2f)
        {
            yield return new WaitForFixedUpdate();
        }
        BeginCrossfade(audioSrc, clipFadeTo);
    }

    private void CheckFade(AudioSource src, AudioClip clip)
    {
        StartCoroutine(FadeCheckSongInList(src, clip));
    }

    public void BeginCrossfade(AudioSource audioSrc, AudioClip clipFadeTo)
    {
        StartCoroutine(FadeOut(audioSrc));
        StartCoroutine(FadeIn(clipFadeTo));
    }

    private AudioSource CreateAudioSource()
    {
        return gameObject.AddComponent<AudioSource>();
    }

    private AudioSource CreateAudioSource(GameObject attachObject)
    {
        return attachObject.AddComponent<AudioSource>();
    }

    private IEnumerator CheckIfClipFinishedAndDestroySrc(AudioSource src)
    {
        while (src.time < src.clip.length)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(src);
        yield break;
    }

    public AudioSource PlayAudio(AudioClip audioClip)
    {
        AudioSource audioSource = CreateAudioSource();
        audioSource.clip = audioClip;
        audioSource.Play();
        StartCoroutine(CheckIfClipFinishedAndDestroySrc(audioSource));
        return audioSource;
    }

    public AudioSource PlayAudio(AudioClip audioClip, AudioType type)
    {
        AudioSource audioSrc = CreateAudioSource();
        audioSrc.clip = audioClip;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();
        StartCoroutine(CheckIfClipFinishedAndDestroySrc(audioSrc));

        return audioSrc;
    }

    public AudioSource PlayAudio(AudioClip audioClip, AudioType type, bool loop)
    {
        AudioSource audioSrc = CreateAudioSource();
        audioSrc.clip = audioClip;
        audioSrc.loop = loop;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();

        if (!loop)
            StartCoroutine(CheckIfClipFinishedAndDestroySrc(audioSrc));

        return audioSrc;
    }

    public AudioSource PlayAudioNoOverlap(AudioClip audioClip, AudioType type)
    {
        AudioSource retval = null;
        if (!isPlayingNoOverlap)
        {
            StartCoroutine(WaitTilSoundOver(audioClip.length));
            retval = PlayAudio(audioClip, type);
        }
        return retval;
    }

    public GameObject PlayAudioWithFade(AudioClip audioClip, AudioType type)
    {
        AudioSource audioSrc = CreateAudioSource();
        audioSrc.clip = audioClip;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();
        CheckFade(audioSrc, audioClip);
        return audioSrc.gameObject;
    }

    public GameObject PlayAudioWithFade(AudioClip audioClip, AudioType type, GameObject attachTo)
    {
        AudioSource audioSrc = CreateAudioSource(attachTo);
        audioSrc.clip = audioClip;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();
        CheckFade(audioSrc, audioClip);
        return audioSrc.gameObject;
    }

    public AudioSource PlayAudio(AudioClip audioClip, AudioType type, bool loop, float vol)
    {
        AudioSource audioSrc = CreateAudioSource();
        audioSrc.clip = audioClip;
        audioSrc.volume = vol;
        audioSrc.loop = loop;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();

        if (!loop)
            StartCoroutine(CheckIfClipFinishedAndDestroySrc(audioSrc));

        return audioSrc;
    }

    public AudioSource PlayAudio(AudioClip audioClip, AudioType type, Vector3 pos)
    {
        GameObject gameObject = new GameObject("New Audio Source");
        gameObject.transform.position = pos;
        var destroyer = gameObject.AddComponent<AudioDestroyer>();
        AudioSource audioSrc = CreateAudioSource(gameObject);
        audioSrc.clip = audioClip;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();
        destroyer?.DestroyAfterSeconds(audioClip.length);
        return audioSrc;
    }

    public AudioSource PlayAudio(AudioClip audioClip, AudioType type, Vector3 pos, float vol)
    {
        GameObject gameObject = new GameObject("New Vol Audio Source");
        gameObject.transform.position = pos;
        var destroyer = gameObject.AddComponent<AudioDestroyer>();
        AudioSource audioSrc = CreateAudioSource(gameObject);
        audioSrc.clip = audioClip;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.volume = vol;
        audioSrc.Play();
        destroyer?.DestroyAfterSeconds(audioClip.length);
        return audioSrc;
    }

    public AudioSource PlayAudio(AudioClip audioClip, AudioType type, Vector3 pos, float vol, float spatialBlend, float maxDistance, bool loop)
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = pos;
        var destroyer = gameObject.AddComponent<AudioDestroyer>();
        AudioSource audioSrc = CreateAudioSource(gameObject);
        audioSrc.clip = audioClip;
        audioSrc.loop = loop;
        audioSrc.spatialBlend = spatialBlend;
        audioSrc.maxDistance = maxDistance;
        audioSrc.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(type))[0];
        audioSrc.Play();

        if (!loop)
            destroyer?.DestroyAfterSeconds(audioClip.length);

        return audioSrc;
    }

    private string GetGroupName(AudioType type)
    {
        switch (type)
        {
            case AudioType.SFX:
                return $"Master/{nameof(AudioType.SFX)}";
            case AudioType.Background:
                return $"Master/{nameof(AudioType.Background)}";
            case AudioType.Music:
                return $"Master/{nameof(AudioType.Music)}";
            default:
                return string.Empty;
        }
    }

    public void DestroyFadingAudio(GameObject audioSrcObject)
    {
        StartCoroutine(FadeOutAndDestroy(audioSrcObject));
    }

    [System.Obsolete("Use PlayAudio() instead!!")]
    public void PlayAudioAtPosition(AudioClip audioClip, Vector3 pos)
    {
        //AudioSource.PlayClipAtPoint(audioClip, pos);
        PlayAudio(audioClip, AudioType.SFX, pos);
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        while (source.volume > .2f)
        {
            yield return new WaitForFixedUpdate();
            source.volume -= ambFadeOutSpeed * Time.deltaTime;
        }
        Destroy(source);
        yield break;
    }

    private IEnumerator FadeOutAndDestroy(GameObject objToDestroy)
    {
        var source = objToDestroy.GetComponent<AudioSource>();
        while (source.volume > .2f)
        {
            yield return new WaitForFixedUpdate();
            source.volume -= ambFadeOutSpeed * Time.deltaTime;
        }
        Destroy(objToDestroy);
        yield break;
    }

    private IEnumerator FadeIn(AudioClip fadeTo)
    {
        var newSource = CreateAudioSource();
        newSource.clip = fadeTo;
        newSource.volume = .5f;
        newSource.outputAudioMixerGroup = Mixer.FindMatchingGroups(GetGroupName(AudioType.Background))[0];
        newSource.Play();
        while (newSource.volume < 1f)
        {
            yield return new WaitForFixedUpdate();
            newSource.volume += ambFadeInSpeed * Time.deltaTime;
        }
        StartCoroutine(FadeCheckSongInList(newSource, GameSounds.AmbientSounds[Random.Range(0, GameSounds.AmbientSounds.Count)]));
        yield break;
    }

    private IEnumerator WaitTilSoundOver(float soundLength)
    {
        isPlayingNoOverlap = true;
        yield return new WaitForSeconds(soundLength);
        isPlayingNoOverlap = false;
    }

    private void SwitchSong(AudioSource audioSource, AudioClip newClip)
    {
        audioSource.clip = newClip;
    }

    //private void OnValidate()
    //{
    //    if (Mixer == null) return;
    //    GameSettings audioSettings = Settings.GetSettings();
    //    audioSettings.MasterVolume = Master;
    //    audioSettings.BackgroundVolume = Background;
    //    audioSettings.MusicVolume = Music;
    //    audioSettings.SFXVolume = SFX;
    //    Settings.SetSettings(audioSettings);
    //    InitSoundVolumes();
    //}

    private void InitSoundVolumes()
    {
        Mixer.SetFloat("Master", Mathf.Lerp(-80f, 0f, Settings.GetSettings().MasterVolume));
        Mixer.SetFloat("SFX", Mathf.Lerp(-80f, 0f, Settings.GetSettings().SFXVolume));
        Mixer.SetFloat("Background", Mathf.Lerp(-80f, 0f, Settings.GetSettings().BackgroundVolume));
        Mixer.SetFloat("Music", Mathf.Lerp(-80f, 0f, Settings.GetSettings().MusicVolume));
    }

    public void ChangeVolume(float val, string groupName)
    {
        groupName = groupName.Replace("VolumeSlider", "");
        GameSettings settings = Settings.GetSettings();
        float lerpVal = Mathf.Lerp(-80f, 0f, val);
        switch (groupName)
        {
            case "Master":
                Mixer.SetFloat(groupName, lerpVal);
                settings.MasterVolume = val;
                break;
            case "Music":
                Mixer.SetFloat(groupName, lerpVal);
                settings.MusicVolume = val;
                break;
            case "Background":
                Mixer.SetFloat(groupName, lerpVal);
                settings.BackgroundVolume = val;
                break;
            case "SFX":
                Mixer.SetFloat(groupName, lerpVal);
                settings.SFXVolume = val;
                break;
            default:
                break;
        }
        Settings.SetSettings(settings);
    }
}
