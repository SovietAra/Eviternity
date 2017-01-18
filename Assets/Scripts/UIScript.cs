using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

/// <summary>
/// This class manages the UI elements of player characters.
/// Author: Richard Brönnimann
/// </summary>
public class UIScript : MonoBehaviour
{
    public GameObject UICanvas;
    public GameObject Player1HealthUI;
    public GameObject Player2HealthUI;
    public GameObject Player3HealthUI;
    public GameObject Player4HealthUI;
    
    private int activePlayers = 0;
    private Slider P1healthSlider;
    private Slider P2healthSlider;
    private Slider P3healthSlider;
    private Slider P4healthSlider;
    private int playerID;
    private List<GameObject> CurrentPlayers;

    // Use this for initialization
    private void Start()
    {
        CurrentPlayers = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {       
        if (CurrentPlayers != null)
        {
            foreach (GameObject player in CurrentPlayers)
            {
                UpdateHealth(player.GetComponent<Player>(), player.GetComponent<DamageAbleObject>());
            }
        }
    }

    internal void OnSpawn(PlayerIndex index)
    {
        CreateUI(index, UICanvas.transform);
        CurrentPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    internal void OnExit(PlayerIndex index)
    {
        CurrentPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        RemoveUI(index);
    }

    private void UpdateHealth(Player player, DamageAbleObject damageAbleObject)
    {
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

    internal void RemoveUI(PlayerIndex index)
    {
        switch (index)
        {
            case PlayerIndex.One:
                {
                    Destroy(P1healthSlider.gameObject);
                }
                break;
            case PlayerIndex.Two:
                {
                    Destroy(P2healthSlider.gameObject);
                }
                break;
            case PlayerIndex.Three:
                {
                    Destroy(P3healthSlider.gameObject);
                }
                break;
            case PlayerIndex.Four:
                {
                    Destroy(P4healthSlider.gameObject);
                }
                break;

            default:
                break;
        };
    }

    public void CreateUI(PlayerIndex index, Transform UICanvas)
    {

        switch (index)
        {
            case PlayerIndex.One:
                {
                    GameObject P1 = Instantiate(Player1HealthUI);
                    P1healthSlider = P1.GetComponent<Slider>();
                    P1healthSlider.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Two:
                {
                    GameObject P2 = Instantiate(Player2HealthUI);
                    P2healthSlider = P2.GetComponent<Slider>();
                    P2healthSlider.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Three:
                {
                    GameObject P3 = Instantiate(Player3HealthUI);
                    P3healthSlider = P3.GetComponent<Slider>();
                    P3healthSlider.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Four:
                {
                    GameObject P4 = Instantiate(Player4HealthUI);
                    P4healthSlider = P4.GetComponent<Slider>();
                    P4healthSlider.transform.SetParent(UICanvas, false);
                }
                break;

            default:
                break;
        }
    }
}