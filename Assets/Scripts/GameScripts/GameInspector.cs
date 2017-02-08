using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

public class GameInspector : MonoBehaviour
{
    public GameObject PlayerPrefabAegis;
    public GameObject PlayerPrefabStalker;
    public GameObject PlayerPrefab;
    public GameObject PauseMenuCanvas;
    public GameObject DefeatCanvas;
    public GameObject WinCanvas;
    public GameObject AskCanvas;

    public static bool Defeat, Win;

    private PlayerChoice playerChoice;
    private int[] choice;

    private List<Player> spawnedPlayers;
    private UIScript uiScript;

    [SerializeField]
    [Range(0, 10000f)]
    private float maxTeamHealth = 10;

    [SerializeField]
    [Range(0, 10)]
    private float healthRegenerationMultiplicator = 1f;

    [SerializeField]
    [Range(0, 10)]
    private float healthRegenerationMulitplicatorOnDeath = 2f;

    public float MaxTeamHealth
    {
        get { return maxTeamHealth; }
    }

    // Use this for initialization
    private void Start()
    {
        playerChoice = GameObject.FindObjectOfType<PlayerChoice>();

        if (playerChoice != null)
        {
            List<int> temp = new List<int>();
            temp.AddRange(playerChoice.choices);

            choice = temp.ToArray();

            Destroy(playerChoice.gameObject);
        }
        spawnedPlayers = new List<Player>();
        uiScript = GetComponent<UIScript>();

        Player.LastCheckpointPosition = Vector3.zero;
        Player.TeamHealth = maxTeamHealth;
        Player.HealthRegenerationMultiplicator = healthRegenerationMultiplicator;
        Player.HealthRegenerationMulitplicatorOnDeath = healthRegenerationMulitplicatorOnDeath;
        Defeat = false;
        Win = false;
        SpawnPlayers(false);
    }

    private void WinAndDefeat()
    {
        if (Win)
        {
            Time.timeScale = 0;
            if (WinCanvas.activeInHierarchy == false)
            {
                WinCanvas.SetActive(true);
            }
            FreezeAllPlayers();
        }

        if (Defeat)
        {
            Time.timeScale = 0;
            if (DefeatCanvas.activeInHierarchy == false)
            {
                DefeatCanvas.SetActive(true);
            }
            FreezeAllPlayers();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Pause();
        CheckForNewPlayers();
        WinAndDefeat();

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
            FreezeAllPlayers();
        }
    }

    public void ResumeGame()
    {
        Win = false;
        Defeat = false;
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
        UnfreezeAllPlayers();
    }

    public void CreditsScreen()
    {
        Win = false;
        Defeat = false;
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
        SceneManager.LoadScene("Credits");
        UnfreezeAllPlayers();
    }

    public void Restart()
    {
        PlayerAssignmentScript.gameStarted = false;
        foreach (PlayerState item in GlobalReferences.PlayerStates)
        {
            GamePadManager.Disconnect(item.Index);
        }
        GlobalReferences.PlayerStates.Clear();
        SceneManager.LoadScene("PlayerAssignment");
    }

    public void MainMenu()
    {
        GlobalReferences.CurrentGameState = GlobalReferences.GameState.Play;
        PlayerAssignmentScript.gameStarted = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void SpawnPlayers(bool useTeamHealth = true)
    {
        if (Player.TeamHealth > 0 || !useTeamHealth)
        {
            float shareHealth = Player.TeamHealth / GlobalReferences.PlayerStates.Count;
            for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
            {
                if (choice != null)
                {
                    if (choice[(int)GlobalReferences.PlayerStates[i].Index] == 0)
                    {
                        PlayerPrefab = PlayerPrefabAegis;
                    }
                    else if (choice[(int)GlobalReferences.PlayerStates[i].Index] == 1)
                    {
                        PlayerPrefab = PlayerPrefabStalker;
                    }
                }
                else
                {
                    PlayerPrefab = PlayerPrefabAegis;
                }

                GameObject gobj = SpawnPlayer(GlobalReferences.PlayerStates[i], Player.LastCheckpointPosition + new Vector3(i * 2, 1, 0));
                if (gobj != null && useTeamHealth)
                {
                    DamageAbleObject healthContainer = gobj.GetComponent<DamageAbleObject>();
                    if (healthContainer != null)
                    {
                        if (Player.TeamHealth > GlobalReferences.PlayerStates.Count * healthContainer.MaxHealth)
                        {
                            healthContainer.Health = healthContainer.MaxHealth;
                            Player.TeamHealth -= healthContainer.MaxHealth;
                        }
                        else
                        {
                            healthContainer.Health = shareHealth;
                            Player.TeamHealth -= shareHealth;
                        }
                    }
                }
            }
        }
        else
        {
            Player.LastCheckpointPosition = Vector3.zero;
            Defeat = true;
        }
    }

    private void PlayerScript_OnPlayerExit(object sender, PlayerEventArgs e)
    {
        if (spawnedPlayers.Count > 1)
        {
            e.PlayerScript.OnPlayerExit -= PlayerScript_OnPlayerExit;
            spawnedPlayers.Remove(e.PlayerScript);
            RemovePlayerState(e.PlayerScript.Index);
            GamePadManager.Disconnect(e.PlayerScript.Index);

            Destroy(e.PlayerObject);
            uiScript.OnExit(e.PlayerScript.Index);
        }
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
                        if (GlobalReferences.PlayerStates.Count > 1)
                        {
                            FollowingCamera cam = GameObject.FindObjectOfType<FollowingCamera>();
                            if (cam != null)
                            {
                                if(GlobalReferences.PlayerStates.Count >= 3)
                                    SpawnPlayer(newPlayerState, cam.transform.position + new Vector3(0, 1, 0));
                                else
                                    SpawnPlayer(newPlayerState, cam.transform.position + new Vector3(1, 1, 1));
                            }
                            else
                                SpawnPlayer(newPlayerState, Player.LastCheckpointPosition + new Vector3(0, 1, 0));
                        }
                        else
                            SpawnPlayer(newPlayerState, new Vector3(0, 1, 0));
                    }
                }
            }
        }
    }

    private bool IndexInUse(PlayerIndex index)
    {
        if (spawnedPlayers != null)
        {
            for (int i = 0; i < spawnedPlayers.Count; i++)
            {
                if (spawnedPlayers[i].Index == index)
                    return true;
            }
        }
        return false;
    }

    private GameObject SpawnPlayer(PlayerState playerState, Vector3 position)
    {
        GameObject newPlayer = Instantiate(PlayerPrefab, position, Quaternion.Euler(0, 0, 0));
        Player playerScript = newPlayer.GetComponent<Player>();
        playerScript.Index = playerState.Index;
        playerScript.OnPlayerExit += PlayerScript_OnPlayerExit;
        spawnedPlayers.Add(playerScript);
        uiScript.OnSpawn(playerState.Index);
        return newPlayer;
    }

    private void RemovePlayerState(PlayerIndex index)
    {
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            if (GlobalReferences.PlayerStates[i].Index == index)
                GlobalReferences.PlayerStates.RemoveAt(i);
        }
    }

    private void UnfreezeAllPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject item in playerObjects)
        {
            Player player = item.GetComponent<Player>();
            if(player != null)
                player.Freeze = false;
        }
    }

    private void FreezeAllPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject item in playerObjects)
        {
            Player player = item.GetComponent<Player>();
            if (player != null)
                player.Freeze = true;
        }
    }
}
