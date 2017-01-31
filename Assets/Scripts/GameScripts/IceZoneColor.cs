using UnityEngine;

public class IceZoneColor : MonoBehaviour {

    private Color ObjectColor = Color.blue;
    private Material materialColored;

    private void Start()
    {
        ObjectColor.a = 0.2f;
    }

    private void Update()
    {
            materialColored = new Material(Shader.Find("Transparent/Diffuse"));
            materialColored.color = ObjectColor;
            GetComponent<Renderer>().material = materialColored;
    }
}
