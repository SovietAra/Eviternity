using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class UIScript : MonoBehaviour
{
    // GameInspeector: Calls function to Add or Remove the UI

    public GameObject UICanvas;
    public GameObject Player1HealthUI;
    public GameObject Player2HealthUI;
    public GameObject Player3HealthUI;
    public GameObject Player4HealthUI;
    public int playerCount;

    public void OnUIEvent(object source, EventArgs e)
    {
        CreateUI(playerCount, UICanvas.transform);

    }

    public int prevPlayerCount;

    private int activePlayers = 0;
    private Slider P1healthSlider;
    private Slider P2healthSlider;
    private Slider P3healthSlider;
    private Slider P4healthSlider;
    private int playerID;
    private bool[] ConnectedPlayers = new bool[4] { false, false, false, false };
    private List<GameObject> CurrentPlayers;

    // Use this for initialization
    private void Start()
    {
        CurrentPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    // Update is called once per frame
    private void Update()
    {
        // GameObject[] CurrentPlayers = new GameObject[4];

        playerCount = CurrentPlayers.Count;

        if (CurrentPlayers != null)
        {
            //if (playerCount > prevPlayerCount)
            //{
            //    CreateUI(playerCount, UICanvas.transform);
            //}
        }

        foreach (GameObject player in CurrentPlayers)
        {
            UpdateHealth(player.GetComponent<Player>(), player.GetComponent<DamageAbleObject>());
        }

        // TODO if an element of the array is false, destroy that UI
        //if (player.gameObject == null)
        //{
        //    Destroy(Player1HealthUI);
        //}

        prevPlayerCount = playerCount;
    }

    private void UpdateHealth(Player player, DamageAbleObject damageAbleObject)
    {
        //player.Index == PlayerIndex.One;
        switch (player.Index)
        {
            case PlayerIndex.One:
                {
                    P1healthSlider.value = damageAbleObject.Health;
                }
                break;

            case PlayerIndex.Two:
                {
                    P2healthSlider.value = damageAbleObject.Health;
                }
                break;

            case PlayerIndex.Three:
                {
                    P3healthSlider.value = damageAbleObject.Health;
                }
                break;

            case PlayerIndex.Four:
                {
                    P4healthSlider.value = damageAbleObject.Health;
                }
                break;

            default:
                break;
        }
    }

    public void CreateUI(int playerCount, Transform UICanvas)
    {
        int activePlayers = GetActivePlayers();

        switch (activePlayers)
        {
            case 0:
                {
                    GameObject P1 = Instantiate(Player1HealthUI);
                    P1healthSlider = P1.GetComponent<Slider>();
                    P1healthSlider.transform.SetParent(UICanvas, false);
                    AddToActivePlayers();
                }
                break;
            case 1:
                {
                    GameObject P2 = Instantiate(Player2HealthUI);
                    P2healthSlider = P2.GetComponent<Slider>();
                    P2healthSlider.transform.SetParent(UICanvas, false);
                    AddToActivePlayers();
                }
                break;
            case 2:
                {
                    GameObject P3 = Instantiate(Player3HealthUI);
                    P3healthSlider = P3.GetComponent<Slider>();
                    P3healthSlider.transform.SetParent(UICanvas, false);
                    AddToActivePlayers();
                }
                break;
            case 3:
                {
                    GameObject P4 = Instantiate(Player4HealthUI);
                    P4healthSlider = P4.GetComponent<Slider>();
                    P4healthSlider.transform.SetParent(UICanvas, false);
                    AddToActivePlayers(); 
                }
                break;

            default:
                break;
        }
        //switch (playerCount)
        //{
        //    case 1:
        //        {
        //            playerID = 1;
        //            GameObject P1 = Instantiate(Player1HealthUI);
        //            P1healthSlider = P1.GetComponent<Slider>();
        //            P1healthSlider.transform.SetParent(UICanvas, false);
        //        }
        //        break;

        //    case 2:
        //        {
        //            playerID = 2;
        //            GameObject P2 = Instantiate(Player2HealthUI);
        //            P2healthSlider = P2.GetComponent<Slider>();
        //            P2healthSlider.transform.SetParent(UICanvas, false);
        //        }
        //        break;

        //    case 3:
        //        {
        //            playerID = 3;
        //            GameObject P3 = Instantiate(Player3HealthUI);
        //            P3healthSlider = P3.GetComponent<Slider>();
        //            P3healthSlider.transform.SetParent(UICanvas, false);
        //        }
        //        break;

        //    case 4:
        //        {
        //            playerID = 4;
        //            GameObject P4 = Instantiate(Player4HealthUI);
        //            P4healthSlider = P4.GetComponent<Slider>();
        //            P4healthSlider.transform.SetParent(UICanvas, false);
        //        }
        //        break;

        //    default:
        //        break;
        //}
    }

    private void AddToActivePlayers()
    {
        for (int j = 0; j < ConnectedPlayers.Length; j++)
        {
            if (ConnectedPlayers[j] == false)
            {
                ConnectedPlayers[j] = true;
                break;
            }
        }
    }

    private int GetActivePlayers()
    {
        for (int i = 0; i < ConnectedPlayers.Length; i++)
        {
            if (ConnectedPlayers[i] == true)
            {
                activePlayers++;
            }
        }

        return activePlayers;
    }
}