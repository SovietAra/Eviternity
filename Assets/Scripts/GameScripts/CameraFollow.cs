using System;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public float CameraY;
    public byte ActivePlayers;
    private float _xCord, _zCord;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 P1 = GameObject.Find("Player1").transform.position;
        Vector3 P2 = GameObject.Find("Player2").transform.position;
        Vector3 P3 = GameObject.Find("Player3").transform.position;
        Vector3 P4 = GameObject.Find("Player4").transform.position;


        int minX;
        int maxX;
        int minZ;
        int maxZ;

        minX = (int)Math.Min(Math.Min(Math.Min(P1.x, P2.x), P3.x), P4.x);
        maxX = (int)Math.Max(Math.Max(Math.Max(P1.x, P2.x), P3.x), P4.x);
        minZ = (int)Math.Min(Math.Min(Math.Min(P1.z, P2.z), P3.z), P4.z);
        maxZ = (int)Math.Max(Math.Max(Math.Max(P1.z, P2.z), P3.z), P4.z);

        _zCord = (minZ + maxZ) / 2;
        _xCord = (minX + maxX) / 2;

        transform.position = new Vector3(_xCord, CameraY, _zCord);

    }
}