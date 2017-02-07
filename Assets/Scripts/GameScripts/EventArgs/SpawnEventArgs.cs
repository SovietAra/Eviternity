using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpawnEventArgs : EventArgs
    {
        public Vector3 SpawnPosition;
        public Vector3 Forward;
        public float Angle;

        public SpawnEventArgs(Vector3 spawnPosition, Vector3 forward, float angle)
        {
            SpawnPosition = spawnPosition;
            Forward = forward;
            Angle = angle;
        }
    }
}
