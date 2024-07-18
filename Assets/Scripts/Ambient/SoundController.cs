using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    // FOG ------------------------
    public float fogAmount = 0.35f;
    public float fogFadeSpeed = 0.0005f;
    public float fogUnfadeSpeed = 0.0005f;

    [System.Serializable]
    public class SoundSettings
    {
        public bool enableSounds = true;
        [Space(5)]
        public AudioSource breathSource;
        public AudioSource heartSource;
        public AudioSource ambientSource;
        [Space(5)]
        public AudioClip[] caveSounds;
        public AudioClip[] breathSound;
        public AudioClip[] heartSound;
        [Space(5)]
        public AudioReverbZone echo;
    }
    public SoundSettings soundSettings = new SoundSettings();

    [System.Serializable]
    public class MusicSettings
    {
        public bool enableMusic = true;
        [Space(5)]
        public AudioSource source_1;
        public AudioSource source_2;
        [Space(5)]
        public AudioClip startRoomMusic;
        public AudioClip lurkingMusic;
        public AudioClip actionMusic;
        public AudioClip afterchaseMusic;
        public AudioClip endMusic;
        [Space(5)]
        public float volume = 0.05f;
    }
    public MusicSettings musicSettings = new MusicSettings();

    private AudioSource caveSource;

    private static SoundSettings soundSettings_static;
    private static MusicSettings musicSettings_static;

    private IEnumerator music_ienum;
    private IEnumerator heartbeat_ienum;

    public static void PlayBreath()
    {
        if (!soundSettings_static.breathSource.isPlaying)
        {
            soundSettings_static.breathSource.clip = soundSettings_static.breathSound[Random.Range(0, soundSettings_static.breathSound.Length)];
            soundSettings_static.breathSource.Play();
        }
    }

    public static void PlaySound(AudioClip clip, float volume)
    {
        soundSettings_static.breathSource.clip = clip;
        soundSettings_static.breathSource.volume = volume;
        soundSettings_static.breathSource.Play();
    }

    private void Start()
    {
        EventManager.current.onGroundTriggerChanged += ManageEcho;
        EventManager.current.onEnemyStateChanged += ReactMusicToEvent;
        EventManager.current.onEnemyStateChanged += ReactToChaseHeartbeat;

        soundSettings_static = soundSettings;
        musicSettings_static = musicSettings;

        if (musicSettings_static.volume > 1) musicSettings_static.volume = 1;
        musicSettings_static.source_1.loop = true;
        musicSettings_static.source_2.loop = true;

        caveSource = new GameObject("Cave Sounds Source").AddComponent<AudioSource>();
        caveSource.volume = 0.05f;
        caveSource.spatialBlend = 1f;
        caveSource.minDistance = 10f;

        StartCoroutine(PlayAmbient());
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(PlayAmbientImmediate());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(SwapTrack(musicSettings_static.lurkingMusic));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(SwapTrack(musicSettings_static.startRoomMusic));
        }
#endif
    }
    private void ReactToChaseHeartbeat(EnemyController.state state)
    {
        if (heartbeat_ienum != null)
            StopCoroutine(heartbeat_ienum);
        StartCoroutine(heartbeat_ienum = HeartBeat());
    }
    private IEnumerator HeartBeat()
    {
        while (EnemyController.currentState == EnemyController.state.CHASE || EnemyController.currentState == EnemyController.state.LOST) 
        {
            soundSettings_static.heartSource.clip = soundSettings_static.heartSound[Random.Range(0, soundSettings_static.heartSound.Length)];
            soundSettings_static.heartSource.Play();
            yield return new WaitWhile(() => soundSettings_static.heartSource.isPlaying);
        }
        yield return null;
    }

    private void ManageEcho(string tag)
    {
        print(tag);
        switch (tag)
        {
            case "Concrete":
                {
                    if (RenderSettings.fogDensity > 0.01f)
                    {
                        StopCoroutine(UnfadeFog());
                        StopCoroutine(FadeFog());
                        StartCoroutine(FadeFog());
                    }

                    soundSettings.echo.reverbPreset = AudioReverbPreset.Livingroom;

                    if (music_ienum != null)
                        StopCoroutine(music_ienum);
                    if (!EnemyController.isChasing) StartCoroutine(music_ienum = SwapTrack(musicSettings_static.startRoomMusic));

                    if (soundSettings.ambientSource)
                    {
                        if (soundSettings.ambientSource.isPlaying) soundSettings.ambientSource.Stop();
                        if (caveSource.isPlaying) caveSource.Stop();
                    }
                    break;
                }
            default:
                {
                    if (RenderSettings.fogDensity < fogAmount)
                    {
                        StopCoroutine(UnfadeFog());
                        StopCoroutine(FadeFog());
                        StartCoroutine(UnfadeFog());
                    }
                    soundSettings.echo.reverbPreset = AudioReverbPreset.Arena;
                    if (music_ienum != null)
                        StopCoroutine(music_ienum);
                    if (!EnemyController.isChasing) StartCoroutine(music_ienum = SwapTrack(musicSettings_static.lurkingMusic));
                    break;
                }
        }
    }

    private void ReactMusicToEvent(EnemyController.state state)
    {
        if (musicSettings_static.enableMusic)
        {
            switch (state)
            {
                case (EnemyController.state.CHASE):
                    {
                        if (music_ienum != null)
                            StopCoroutine(music_ienum);
                        StartCoroutine(music_ienum = SwapTrack(musicSettings_static.actionMusic, 1f));
                        break;
                    }
                case (EnemyController.state.LOST):
                    {
                        if (music_ienum != null)
                            StopCoroutine(music_ienum);
                        StartCoroutine(music_ienum = SwapTrack(musicSettings_static.afterchaseMusic, 3f));
                        break;
                    }
                case (EnemyController.state.SEARCH):
                    {
                        if (musicSettings_static.source_1.clip != musicSettings_static.lurkingMusic
                            || musicSettings_static.source_2.clip != musicSettings_static.lurkingMusic
                            || musicSettings_static.source_1.clip != musicSettings_static.startRoomMusic
                            || musicSettings_static.source_2.clip != musicSettings_static.startRoomMusic)
                        {
                            if (music_ienum != null)
                                StopCoroutine(music_ienum);
                            StartCoroutine(music_ienum = SwapTrack(musicSettings_static.lurkingMusic, 4f));
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
        }
    }

    private IEnumerator SwapTrack(AudioClip toClip, float _timetofade = 0.25f)
    {
        float timetofade = _timetofade;
        float timeelapsed = 0;

        if (musicSettings_static.source_1.isPlaying)
        {
            musicSettings_static.source_2.clip = toClip;
            musicSettings_static.source_2.volume = 0;
            musicSettings_static.source_2.Play();

            while (timeelapsed < timetofade)
            {
                musicSettings_static.source_2.volume = Mathf.Lerp(0, musicSettings_static.volume, timeelapsed/timetofade);
                musicSettings_static.source_1.volume = Mathf.Lerp(musicSettings_static.volume, 0, timeelapsed/timetofade);
                timeelapsed += Time.deltaTime;
                yield return null;
            }

            musicSettings_static.source_1.Stop();

        } else
        {
            musicSettings_static.source_1.clip = toClip;
            musicSettings_static.source_1.volume = 0;
            musicSettings_static.source_1.Play();

            while (timeelapsed < timetofade)
            {
                musicSettings_static.source_1.volume = Mathf.Lerp(0, musicSettings_static.volume, timeelapsed / timetofade);
                musicSettings_static.source_2.volume = Mathf.Lerp(musicSettings_static.volume, 0, timeelapsed / timetofade);
                timeelapsed += Time.deltaTime;
                yield return null;
            }

            musicSettings_static.source_2.Stop();
        }
    }

    private IEnumerator FadeFog()
    {
        while (RenderSettings.fogDensity > 0.01f)
        {
            RenderSettings.fogDensity -= fogFadeSpeed;
            yield return null;
        }
    }

    private IEnumerator UnfadeFog()
    {
        while (RenderSettings.fogDensity < fogAmount)
        {
            RenderSettings.fogDensity += fogUnfadeSpeed;
            yield return null;
        }
    }

    private IEnumerator PlayAmbient()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(60f, 300f));

            if (soundSettings.enableSounds)
            {
                if (PlayerController.groundTag != "Concrete")
                {
                    if (!caveSource.isPlaying)
                    {
                        caveSource.gameObject.transform.position = transform.position + new Vector3(Random.Range(5f, 30f), 0, Random.Range(5f, 30f));

                        caveSource.clip = soundSettings.caveSounds[Random.Range(0, soundSettings.caveSounds.Length)];
                        caveSource.Play();
                    }
                }
            }
        }
    }
#if UNITY_EDITOR
    private IEnumerator PlayAmbientImmediate()
    {
        if (soundSettings.enableSounds)
        {
            if (PlayerController.groundTag != "Concrete")
            {
                if (!caveSource.isPlaying)
                {
                    caveSource.gameObject.transform.position = transform.position + new Vector3(Random.Range(5f, 30f), 0, Random.Range(5f, 30f));

                    caveSource.clip = soundSettings.caveSounds[Random.Range(0, soundSettings.caveSounds.Length)];
                    caveSource.Play();
                }
            }
        }

        yield return null;
    }
#endif
}
