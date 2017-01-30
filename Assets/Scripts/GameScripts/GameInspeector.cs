using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using XInputDotNetPure;
using System;
using UnityEngine.SceneManagement;

public class GameInspeector : MonoBehaviour
{
    public GameObject PlayerPrefab;
    private List<Player> spawnedPlayers;
    private UIScript uiScript;

    public GameObject PauseMenuCanvas;


    [SerializeField] [Range(0, 10000f)] public float maxTeamHealth = 10;

    [SerializeField] [Range(0, 10)] private float healthRegenerationMultiplicator = 1f;

    [SerializeField] [Range(0, 10)] private float healthRegenerationMulitplicatorOnDeath = 2f;

    // Use this for initialization
    void Start()
    {
        //SpawnPlayers();
        //uiScript = GetComponent<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiScript == null)
            uiScript = GetComponent<UIScript>();

        Pause();

        CheckForNewPlayers();
        
        List<GameObject> AvailablePlayer = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        if (AvailablePlayer != null)
        {
            bool AllPlayerDead = true;
            for (int i = 0; i < AvailablePlayer.Count; i++)
            {
                Player player = AvailablePlayer[i].GetComponent<Player>();
                if (player != null)
                    AllPlayerDead = false;
            }
            if (AllPlayerDead)
                SpawnPlayers();
        }
    }

    private void Pause()
    {
        if (GlobalReferences.CurrentGameState == GlobalReferences.GameState.Play)
        {
            Time.timeScale = 1;
            PauseMenuCanvas.SetActive(false);

        }
        else if (GlobalReferences.CurrentGameState == GlobalReferences.GameState.Pause)
        {
            Time.timeScale = 0;
            if (PauseMenuCanvas.activeInHierarchy == false)
            {
                PauseMenuCanvas.SetActive(true);
            }
        }
    }

    public void ResumeGame()
    {
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
    }

    public void CreditsScreen()
    {
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
        SceneManager.LoadScene("Credits");
    }

    public void Restart()
    {
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
        SceneManager.LoadScene("Prototyp");
    }

    public void MainMenu()
    {
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
        SceneManager.LoadScene("MainMenu");
    }

private void SpawnPlayers()
    {
        spawnedPlayers = new List<Player>();
        Player.TeamHealth = maxTeamHealth;
        Player.HealthRegenerationMultiplicator = healthRegenerationMultiplicator;
        Player.HealthRegenerationMulitplicatorOnDeath = healthRegenerationMulitplicatorOnDeath;
        
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            SpawnPlayer(GlobalReferences.PlayerStates[i], Player.LastCheckpointPosition + new Vector3(i * 2, 1, 0));
        }
    }

    private void PlayerScript_OnPlayerExit(object sender, PlayerEventArgs e)
    {
        e.PlayerScript.OnPlayerExit -= PlayerScript_OnPlayerExit;
        spawnedPlayers.Remove(e.PlayerScript);
        RemovePlayerState(e.PlayerScript.Index);
        GamePadManager.Disconnect(e.PlayerScript.Index);
        
        Destroy(e.PlayerObject);
        uiScript.OnExit(e.PlayerScript.Index);
    }

    private void CheckForNewPlayers()
    {
        PlayerIndex[] freePads = GamePadManager.GetFreeControllers();
        for (int i = 0; i < freePads.Length; i++)
        {
            if(!IndexInUse(freePads[i]))
            {
                GamePadState state = GamePad.GetState(freePads[i]);
                if (state.IsConnected)
                {
                    if (state.Buttons.Start == ButtonState.Pressed)
                    {
                        PlayerState newPlayerState = new PlayerState(freePads[i], state, true, 0);
                        GlobalReferences.PlayerStates.Add(newPlayerState);
                        GamePadManager.Connect((int)freePads[i]);
                        SpawnPlayer(newPlayerState, new Vector3(0, 1, 0));
                    }
                }
            }
        }
    }

    private bool IndexInUse(PlayerIndex index)
    {
        for (int i = 0; i < spawnedPlayers.Count; i++)
        {
            if (spawnedPlayers[i].Index == index)
                return true;
        }

        return false;
    }

    private void SpawnPlayer(PlayerState playerState, Vector3 position)
    {
        GameObject newPlayer = Instantiate(PlayerPrefab, position, Quaternion.Euler(0, 0, 0));
        Player playerScript = newPlayer.GetComponent<Player>();
        playerScript.Index = playerState.Index;
        playerScript.OnPlayerExit += PlayerScript_OnPlayerExit;
        spawnedPlayers.Add(playerScript);
        uiScript.OnSpawn(playerState.Index);
    }

    private void RemovePlayerState(PlayerIndex index)
    {
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            if (GlobalReferences.PlayerStates[i].Index == index)
                GlobalReferences.PlayerStates.RemoveAt(i);
        }
    }
}
