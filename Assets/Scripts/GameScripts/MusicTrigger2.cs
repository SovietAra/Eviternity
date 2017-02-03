using UnityEngine;

public class MusicTrigger2 : MonoBehaviour
{
    [SerializeField]
    [Range(0, 5)]
    private int MusicThemeIndex;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        GameObject.FindGameObjectWithTag("MusicPlayer")
            .GetComponent<MusicObjectScript>()
            .PlayMusicThemePicker(MusicThemeIndex, true);
    }

    private void OnTriggerExit(Collider other)
    {
    }
}