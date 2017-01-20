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
    public Image TeamHealthBar;
    public Image TeamHealthBar_Border;
    public Sprite TeamHealthActive;
    public Sprite TeamHealthInactive;

    private float teamhealthAmount;
    private Image player1HealthBar;
    private Image player2HealthBar;
    private Image player3HealthBar;
    private Image player4HealthBar;
    private bool isHealing = false;
    private List<GameObject> CurrentPlayers;

    // Use this for initialization
    private void Start()
    {
        CurrentPlayers = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(CurrentPlayers.Count);
        if (CurrentPlayers != null)
        {
            foreach (GameObject player in CurrentPlayers)
            {
                if (player != null)
                    UpdateHealth(player.GetComponent<Player>(), player.GetComponent<DamageAbleObject>());
            }

            if (CurrentPlayers.Count == 0)
            {
                UICanvas.SetActive(false);
            }
            else if (CurrentPlayers.Count >= 1)
            {
                if(UICanvas.activeInHierarchy == false)
                {
                    UICanvas.SetActive(true);
                }

                if (isHealing)
                {
                    teamhealthAmount = Player.TeamHealth / 30;
                    TeamHealthBar.fillAmount = teamhealthAmount;
                    TeamHealthBar_Border.GetComponent<Image>().sprite = TeamHealthActive;
                }
                else
                {
                    TeamHealthBar_Border.GetComponent<Image>().sprite = TeamHealthInactive;
                }
                isHealing = false;
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

    internal void ActivateTeamBar()
    {
        isHealing = true;
    }

    private void UpdateHealth(Player player, DamageAbleObject damageAbleObject)
    {
        switch (player.Index)
        {
            case PlayerIndex.One:
                {
                    player1HealthBar.fillAmount = damageAbleObject.Health / 10;
                }
                break;

            case PlayerIndex.Two:
                {
                    player2HealthBar.fillAmount = damageAbleObject.Health / 10;
                }
                break;

            case PlayerIndex.Three:
                {
                    player3HealthBar.fillAmount = damageAbleObject.Health / 10;
                }
                break;

            case PlayerIndex.Four:
                {
                    player4HealthBar.fillAmount = damageAbleObject.Health / 10;
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
                    Destroy(player1HealthBar.gameObject);
                }
                break;
            case PlayerIndex.Two:
                {
                    Destroy(player2HealthBar.gameObject);
                }
                break;
            case PlayerIndex.Three:
                {
                    Destroy(player3HealthBar.gameObject);
                }
                break;
            case PlayerIndex.Four:
                {
                    Destroy(player4HealthBar.gameObject);
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
                    player1HealthBar = P1.GetComponent<Image>();
                    player1HealthBar.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Two:
                {
                    GameObject P2 = Instantiate(Player2HealthUI);
                    player2HealthBar = P2.GetComponent<Image>();
                    player2HealthBar.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Three:
                {
                    GameObject P3 = Instantiate(Player3HealthUI);
                    player3HealthBar = P3.GetComponent<Image>();
                    player3HealthBar.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Four:
                {
                    GameObject P4 = Instantiate(Player4HealthUI);
                    player4HealthBar = P4.GetComponent<Image>();
                    player4HealthBar.transform.SetParent(UICanvas, false);
                }
                break;

            default:
                break;
        }
    }
}