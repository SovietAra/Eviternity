using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // public Material Checkpoint1;
    public Material CheckpointNewMaterial;
    public AudioClip ActivationSound;

    private GameInspector gameInspector;
    private bool Triggered = false;
    private BoxCollider boxCollider;
    private AudioSource source;

    [SerializeField]
    [Range(0, 10000)]
    private float TeamHealthRestore = 100;

    

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = true;

        gameInspector = GameObject.FindObjectOfType<GameInspector>();
        boxCollider = GetComponent<BoxCollider>();
        source = GetComponent<AudioSource>();
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
        if(source != null && ActivationSound != null)
        {
            source.clip = ActivationSound;
            source.Play();
        }
        var   intMaterials = new Material[2];
        for (int i = 0; i < intMaterials.Length; i++)
        {
            intMaterials[i] = CheckpointNewMaterial;
        }
        GetComponent<Renderer>().materials = intMaterials;
    }
}