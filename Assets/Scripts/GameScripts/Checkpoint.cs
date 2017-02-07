using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private ColorSwitchCheck colorSwitch;
    private GameInspector gameInspector;
    private bool Triggered = false;
    private BoxCollider boxCollider;

    [SerializeField]
    [Range(0, 10000)]
    private float TeamHealthRestore = 100;

    private void Start()
    {
        colorSwitch = GetComponent<ColorSwitchCheck>();
        gameInspector = GameObject.FindObjectOfType<GameInspector>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Triggered && other.gameObject.CompareTag("Player"))
        {
            Triggered = true;
            
            Player.LastCheckpointPosition = transform.position + boxCollider.center;

            colorSwitch.ChangeColor();

            Player.TeamHealth += TeamHealthRestore;
            if (Player.TeamHealth > gameInspector.MaxTeamHealth)
            {
                Player.TeamHealth = gameInspector.MaxTeamHealth;
            }
        }
    }
}
