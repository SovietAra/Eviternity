using UnityEngine;
using UnityEngine.UI;

public class VideoPlayer : MonoBehaviour {

    public MovieTexture movie;
    public bool loop;
    public bool stoppable;
    public float pauseTime;
    public float movieTime;
    public float elapsedTime;
    public RawImage Movie;

	// Use this for initialization
	void Start ()
    {
        if (movie != null)
        {
            GetComponent<RawImage>().texture = movie as MovieTexture;
            movie.Play();

            if (!loop)
                movie.loop = false;

            if (loop)
                movie.loop = true;
        }
        pauseTime = 3.0f;
        elapsedTime = 0.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (movie != null)
        {
            pauseTime -= Time.deltaTime;

            if (stoppable)
            {
                if (pauseTime <= 0)
                {
                    movie.Pause();
                }
            }
        }
	}
}
