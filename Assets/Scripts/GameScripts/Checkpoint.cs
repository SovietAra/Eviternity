using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private ColorSwitchCheck colorSwitch;
    private GameInspector gameInspector;
    private bool Triggered = false;

    [SerializeField]
    [Range(0, 10000)]
    private float TeamHealthRestore = 100;

    private void Start()
    {
        colorSwitch = GetComponent<ColorSwitchCheck>();
        gameInspector = GameObject.FindObjectOfType<GameInspector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Triggered)
        {
            Triggered = true;
            if (other.gameObject.tag == "Player")
                Player.LastCheckpointPosition = transform.position;

            colorSwitch.ChangeColor();

            Player.TeamHealth += TeamHealthRestore;
            if (Player.TeamHealth > gameInspector.MaxTeamHealth)
            {
                Player.TeamHealth = gameInspector.MaxTeamHealth;
            }
        }
    }
}
