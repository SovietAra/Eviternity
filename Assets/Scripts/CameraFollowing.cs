using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public List<GameObject> Controllers;
    public float _camMinHeight = 10;
    public float _camMaxHeight =30;

    private float aspectRatio;
    private Vector3 middlePoint;
    

    void Start()
    {
        aspectRatio = Screen.width / Screen.height;
    }

    void Update()
    {
        Controllers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        if (Controllers != null && Controllers.Count > 0)
        {
            var maxX = ConvertX(Controllers).Max();
            var maxZ = ConvertZ(Controllers).Max();
            var minX = ConvertX(Controllers).Min();
            var minZ = ConvertZ(Controllers).Min();

            var x = minX + (Mathf.Abs(maxX - minX) / 2F); // (maxX + minX) / 2;
            var z = minZ + (Mathf.Abs(maxZ - minZ) / 2F); //(maxZ + minZ) / 2;

            var y = Mathf.Clamp( (90F/ Camera.main.fieldOfView * (maxZ - z)), _camMinHeight,_camMaxHeight); 

            // Position the camera in the center.
            middlePoint = new Vector3(x, y, z);
            transform.position = Vector3.Lerp(transform.position, middlePoint, Time.deltaTime);
       
        }
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