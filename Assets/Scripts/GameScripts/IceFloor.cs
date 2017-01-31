using UnityEngine;

public class IceFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
            player.OnIce = true;

        Debug.Log("Ice");
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
            player.OnIce = false;
    }
}
