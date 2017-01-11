using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public List<PlayerController> Controllers;
    void Start()
    {
        Controllers = new List<PlayerController>(FindObjectsOfType<PlayerController>());
    }

    void Update ()
    {
        var maxX = ConvertX(Controllers).Max();
        var maxZ = ConvertZ(Controllers).Max();
        var minX = ConvertX(Controllers).Min();
        var minZ = ConvertZ(Controllers).Min();

        var x = (maxX + minX) / 2;
        var z = (maxZ + minZ) / 2;
        transform.position = new Vector3(x,150,z);
    }

    List<float> ConvertX(List<PlayerController> controllers)
    {
        var xs = new List<float>();
        foreach (var playerController in controllers)
        {
            xs.Add(playerController.transform.position.x);
        }
        return xs;
    }

    List<float> ConvertZ(List<PlayerController> controllers)
    {
        var ys = new List<float>();
        foreach (var playerController in controllers)
        {
            ys.Add(playerController.transform.position.z);
        }
        return ys;
    }
}
