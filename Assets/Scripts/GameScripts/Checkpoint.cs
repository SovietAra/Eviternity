using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // public Material Checkpoint1;
    public Material CheckpointNewMaterial;

    private GameInspector gameInspector;
    private bool Triggered = false;
    private BoxCollider boxCollider;

    [SerializeField]
    [Range(0, 10000)]
    private float TeamHealthRestore = 100;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = true;

        gameInspector = GameObject.FindObjectOfType<GameInspector>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Triggered && other.gameObject.CompareTag("Player"))
        {
            Triggered = true;

            Player.LastCheckpointPosition = transform.position + boxCollider.center;

            ChangeColor();

            Player.TeamHealth += (gameInspector.MaxTeamHealth - Player.TeamHealth) * TeamHealthRestore;
            if (Player.TeamHealth > gameInspector.MaxTeamHealth)
            {
                Player.TeamHealth = gameInspector.MaxTeamHealth;
            }
        }
    }

    public void ChangeColor()
    {
     var   intMaterials = new Material[2];
        for (int i = 0; i < intMaterials.Length; i++)
        {
            intMaterials[i] = CheckpointNewMaterial;
        }
        GetComponent<Renderer>().materials = intMaterials;
    }
}