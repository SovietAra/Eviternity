using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwitch_check : MonoBehaviour {

    private Color ObjectColor = Color.red;

    private Color currentColor;
    private Material materialColored;

    private void Start()
    {
        ObjectColor.a = 0.3f;
    }
    void Update()
    {
        if (ObjectColor != currentColor)
        {
            //helps stop memory leaks
            if (materialColored != null)
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(materialColored));

            //create a new material
            materialColored = new Material(Shader.Find("Transparent/Diffuse"));
            materialColored.color = currentColor = ObjectColor;
            GetComponent<Renderer>().material = materialColored;

        }

    }
    public void ChangeColor()
    {
        ObjectColor = Color.green;
        ObjectColor.a = 0.3f;
    }
}
