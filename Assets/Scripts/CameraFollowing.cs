/* 
 * Purpose: A camera that follows players
 * Author: Daniel Masly & Maksym Hlaholiev
 * Date: 18.01.2016 
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public List<GameObject> Controllers;
    public float CamMinHeight = 10f;
    public float CamMaxHeight = 30f;
    public float CameraFollowSpeed = 3f;
    public float CameraRotationXAngle = 55f;

    // private float aspectRatio;
    private Vector3 _middlePoint;

    private void Start()
    {
        // aspectRatio = Screen.width / Screen.height;
       transform.rotation = Quaternion.Euler(CameraRotationXAngle, transform.rotation.y, transform.rotation.z);
    }

    private void FixedUpdate()
    {
        Controllers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        if (Controllers == null || Controllers.Count <= 0) return;
        var maxX = ConvertX(Controllers).Max();
        var maxZ = ConvertZ(Controllers).Max();
        var minX = ConvertX(Controllers).Min();
        var minZ = ConvertZ(Controllers).Min();

        var x = minX + (Mathf.Abs(maxX - minX) / 2F); 
        var z = minZ + (Mathf.Abs(maxZ - minZ) / 2F); 

        var y = Mathf.Clamp((CameraRotationXAngle / Camera.main.fieldOfView * (maxZ - z)), CamMinHeight, CamMaxHeight);

        // Position the camera in the center minus offset that is calculated using dirty tricks

        //var a = y / Mathf.Tan(CameraRotationXAngle + Camera.main.fieldOfView / 2);
        //var b = y / Mathf.Tan(CameraRotationXAngle - Camera.main.fieldOfView / 2);
        //var c = (b + a) / 2;


        // _middlePoint = new Vector3(x, y, z - c);

          _middlePoint = new Vector3(x, y, z - (90F - CameraRotationXAngle) * 0.15f); //stupid magic numbers
      //  _middlePoint = new Vector3(x, y, z);

        transform.position = Vector3.Lerp(transform.position, _middlePoint, Time.deltaTime * CameraFollowSpeed);
    }

    private static IEnumerable<float> ConvertX(IEnumerable<GameObject> controllers)
    {
        return controllers.Select(playerController => playerController.transform.position.x).ToList();
    }

    private static IEnumerable<float> ConvertZ(IEnumerable<GameObject> controllers)
    {
        return controllers.Select(playerController => playerController.transform.position.z).ToList();
    }
}