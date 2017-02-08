using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    [SerializeField]
    [Range(0, 10f)]
    private float loadingDelay = 10f;
    private float elapsedTime = 0f;
    private AsyncOperation async;
	// Use this for initialization
	void Start ()
    {
        StartLoading();
	}
	
	// Update is called once per frame
	void Update ()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > loadingDelay)
            ActivateScene();
    }
    
    public void StartLoading()
    {
        StartCoroutine("Load");
    }

    private IEnumerator Load()
    {
        async = SceneManager.LoadSceneAsync("LevelZero");
        async.allowSceneActivation = false;
        yield return async;
    }

    public void ActivateScene()
    {
        async.allowSceneActivation = true;
    }
}
