using UnityEngine;

public class Collapse : MonoBehaviour
{

    [SerializeField]
    [Range(0.5f, 30)]
    private float timeToCollapse;

    private float elapsedGameTime;
    
	// Update is called once per frame
	void Update ()
    {
        elapsedGameTime += Time.deltaTime;

        if (elapsedGameTime > timeToCollapse)
        {
            Destroy(gameObject);
        }
	}
}
