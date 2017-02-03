using System;
using UnityEngine;

public class IcicleSpawner : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1000)]
    private float count = 10;

    [SerializeField]
    [Range(0, 100)]
    private float duration = 5f;

    [SerializeField]
    [Range(0, 100)]
    private float radius = 10;

    [SerializeField]
    [Range(0, 100)]
    private float spawnHeight = 10f;

    public GameObject IcicleProjectile;

    private float elapsedTime = 0;
    private int spawned = 0;
    private float spawnsPerSecond = 0f;
    
	private void Start ()
    {
        spawnsPerSecond = count / duration;	
	}
	
	// Update is called once per frame
	private void Update ()
    {
        elapsedTime += Time.deltaTime;
        int toSpawn = Convert.ToInt32(Math.Floor(elapsedTime * spawnsPerSecond));
        if(toSpawn >= 1)
        {
            for (int i = 0; i < toSpawn; i++)
            {
                elapsedTime -= 1 / spawnsPerSecond;
                SpawnIcicle();
            }
        }
	}

    private void SpawnIcicle()
    {
        if (IcicleProjectile != null)
        {
            spawned++;
            GameObject gobj = Instantiate(IcicleProjectile, GetSpawnPosition(), Quaternion.identity);
            Projectile projectile = gobj.GetComponent<Projectile>();
            if (spawned == count)
                Destroy(gameObject);
        }
    }
    
    private Vector3 GetSpawnPosition()
    {
        float angle = UnityEngine.Random.Range(0, 359);
        float distance = UnityEngine.Random.Range(0, radius);

        return new Vector3(transform.position.x + distance * (float)Math.Cos((Math.PI / 180) * angle), spawnHeight, transform.position.z + distance * (float)Math.Sin((Math.PI / 180) * angle));
    }
}
