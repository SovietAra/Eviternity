using UnityEngine;

/// <summary>
/// Scripts needs CharacterController component on an object;
/// </summary>

public class CompleteCameraController : MonoBehaviour
{
    public GameObject player;       // store a reference to the player game object

    private Vector3 offset;         //store the offset distance between the player and camera

    // Use this for initialization
    private void Start()
    {
        //distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame !
    private void LateUpdate()
    {
        // position of the camera's transform  same as the player's, but offset calculated offset distance only
        transform.position = player.transform.position + offset;
    }
}