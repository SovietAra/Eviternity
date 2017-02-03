using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public int MusicThemeIndex = 45;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null )
            player.PlayMusicThemePicker( MusicThemeIndex, true );

    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
            player.PlayMusicThemePicker(MusicThemeIndex, false);
    }
}
