using UnityEngine;

public class ColorSwitchCheck : MonoBehaviour
{
    private Color objectColor = Color.red;

    private Color currentColor;
    private Material materialColored;

    private void Start()
    {
        objectColor.a = 0.3f;
    }

    private void Update()
    {
        if (objectColor != currentColor)
        {
            //helps stop memory leaks
            if (materialColored != null)
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(materialColored));

            //create a new material
            materialColored = new Material(Shader.Find("Transparent/Diffuse"));
            materialColored.color = currentColor = objectColor;
            GetComponent<Renderer>().material = materialColored;

        }

    }

    public void ChangeColor()
    {
        objectColor = Color.green;
        objectColor.a = 0.3f;
    }
}
