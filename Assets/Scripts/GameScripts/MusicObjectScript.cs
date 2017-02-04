using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MusicObjectScript : MonoBehaviour
{
    public AudioClip[] AudioClips = new AudioClip[5];
    private AudioSource[] audioSources;
    private bool[] MusicTheme ;

    [SerializeField]
    [Range(1f, 100f)]
    private float FadeInDelay=2f;
    // Use this for initialization
    private void Start()
    {
       

        for (var tmp = 0; tmp < AudioClips.Length; tmp++)
        {
            gameObject.AddComponent<AudioSource>();
        }

        audioSources = GetComponents<AudioSource>();

        MusicTheme = new bool[AudioClips.Length];

        for (var tmp = 0; tmp < AudioClips.Length; tmp++)
        {
            audioSources[tmp].clip = AudioClips[tmp];
            audioSources[tmp].Play();
            audioSources[tmp].loop = true;
            audioSources[tmp].volume = 0; //mute all sounds at start
        }
    }

    // Update is called once per frame
    private void Update()
    {
        for (var tmp = 0; tmp < AudioClips.Length; tmp++)
        {
            if ( MusicTheme[tmp] && audioSources[tmp].volume < 1f)
            {
               audioSources[tmp].volume +=Time.deltaTime /FadeInDelay;
            }
        }
    }

    public void PlayMusicThemePicker(int tmp, bool state)

    {
        MusicTheme[tmp] = state;
    }
   

    
}