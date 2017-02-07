using UnityEngine;

public class ColorSwitchCheck : MonoBehaviour
{
    public Material Checkpoint;
    public Material Checkpoint1;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = true;
    }

    public void ChangeColor()
    {
        GetComponent<Renderer>().material = Checkpoint1;
    }
}
