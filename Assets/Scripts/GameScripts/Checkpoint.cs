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
        colorSwitch.ChangeColor();
        Debug.Log("It might have worked");
        Player.LastCheckpointPosition = transform.position;
    }
}
