using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private ColorSwitchCheck colorSwitch;

    private void Start()
    {
        colorSwitch = GetComponent<ColorSwitchCheck>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            Player.LastCheckpointPosition = transform.position;
    }
}
