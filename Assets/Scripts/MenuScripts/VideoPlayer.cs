using System.Collections;
using System.Collections.Generic;
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

    public static bool StartGamePressed;
    public static bool AnimationPlayed;

	// Use this for initialization
	void Start () {
        movieTime = movie.duration;

        GetComponent<RawImage>().texture = movie as MovieTexture;
        movie.Play();

        if(loop == false)
        movie.loop = false;

        if (loop == true)
            movie.loop = true;

        pauseTime = 3.0f;
        elapsedTime = 0.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        pauseTime -= Time.deltaTime;

        if (stoppable == true)
        {
            if(pauseTime <= 0)
            {
                movie.Pause();
            }

            if (StartGamePressed == true)
            {
                movie.Play();
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 2.5f)
                    movie.Pause();
            }
            if (elapsedTime >= 4.0)
            {
                AnimationPlayed = true;
            }
        }
	}
}
