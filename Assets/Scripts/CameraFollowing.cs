using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public List<GameObject> Controllers;

    void Start()
    {

    }

    void Update()
    {
        Controllers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        var maxX = ConvertX(Controllers).Max();
        var maxZ = ConvertZ(Controllers).Max();
        var minX = ConvertX(Controllers).Min();
        var minZ = ConvertZ(Controllers).Min();

        var x = (maxX + minX) / 2;
        var z = (maxZ + minZ) / 2;
        transform.position = new Vector3(x, transform .position .y, z);
    }

    List<float> ConvertX(List<GameObject> controllers)
    {
        var xs = new List<float>();
        foreach (var playerController in controllers)
        {
            xs.Add(playerController.transform.position.x);
        }
        return xs;
    }

    List<float> ConvertZ(List<GameObject> controllers)
    {
        var ys = new List<float>();
        foreach (var playerController in controllers)
        {
            ys.Add(playerController.transform.position.z);
        }
        return ys;
    }
}