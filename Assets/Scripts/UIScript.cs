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
    public GameObject Player1HealthUI_background;
    public GameObject Player1HealthUI_fill;
    public GameObject Player1HealthUI_outline;
    public GameObject Player2HealthUI_background;
    public GameObject Player2HealthUI_fill;
    public GameObject Player2HealthUI_outline;
    public GameObject Player3HealthUI_background;
    public GameObject Player3HealthUI_fill;
    public GameObject Player3HealthUI_outline;
    public GameObject Player4HealthUI_background;
    public GameObject Player4HealthUI_fill;
    public GameObject Player4HealthUI_outline;
    public Image TeamHealthBar;
    public Image TeamHealthBar_Border;
    public Sprite TeamHealthActive;
    public Sprite TeamHealthInactive;

    private float teamhealthAmount;
    private float maxTeamHealth;
    private float maxPlayerHealth;
    private Image player1HealthBar_1;
    private Image player1HealthBar_2;
    private Image player1HealthBar_3;
    private Image player2HealthBar_1;
    private Image player2HealthBar_2;
    private Image player2HealthBar_3;
    private Image player3HealthBar_1;
    private Image player3HealthBar_2;
    private Image player3HealthBar_3;
    private Image player4HealthBar_1;
    private Image player4HealthBar_2;
    private Image player4HealthBar_3;
    private bool isHealing = false;
    private List<GameObject> CurrentPlayers;

    // Use this for initialization
    private void Start()
    {
        GameObject game = GameObject.Find("Game");
        GameInspeector gameInspector = game.GetComponent<GameInspeector>();
        maxTeamHealth = gameInspector.maxTeamHealth;
        GameObject playerPrefab = GameObject.Find("Player");
        DamageAbleObject playerDamageable = gameInspector.PlayerPrefab.GetComponent<DamageAbleObject>();
        maxPlayerHealth = playerDamageable.MaxHealth;
    }

    // Update is called once per frame
    private void Update()
    {

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

                teamhealthAmount = Player.TeamHealth / maxTeamHealth;
                TeamHealthBar.fillAmount = teamhealthAmount;

                if (isHealing)
                {
                    if (TeamHealthBar != null && TeamHealthActive != null)
                    {
                        Image image = TeamHealthBar_Border.GetComponent<Image>();
                        image.sprite = TeamHealthActive;
                    }
                }
                else
                {
                    if (TeamHealthBar != null && TeamHealthInactive != null)
                    {
                        Image image = TeamHealthBar_Border.GetComponent<Image>();
                        image.sprite = TeamHealthInactive;
                    }
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
                    player1HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                }
                break;

            case PlayerIndex.Two:
                {
                    player2HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                }
                break;

            case PlayerIndex.Three:
                {
                    player3HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                }
                break;

            case PlayerIndex.Four:
                {
                    player4HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
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
                    Destroy(player1HealthBar_1.gameObject);
                    Destroy(player1HealthBar_2.gameObject);
                    Destroy(player1HealthBar_3.gameObject);
                }
                break;
            case PlayerIndex.Two:
                {
                    Destroy(player2HealthBar_1.gameObject);
                    Destroy(player2HealthBar_2.gameObject);
                    Destroy(player2HealthBar_3.gameObject);
                }
                break;
            case PlayerIndex.Three:
                {
                    Destroy(player3HealthBar_1.gameObject);
                    Destroy(player3HealthBar_2.gameObject);
                    Destroy(player3HealthBar_3.gameObject);
                }
                break;
            case PlayerIndex.Four:
                {
                    Destroy(player4HealthBar_1.gameObject);
                    Destroy(player4HealthBar_2.gameObject);
                    Destroy(player4HealthBar_3.gameObject);
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
                    //GameObject P1 = Instantiate(Player1HealthUI);
                    //player1HealthBar = P1.GetComponent<Image>();
                    //player1HealthBar.transform.SetParent(UICanvas, false);
                    GameObject P1_1 = Instantiate(Player1HealthUI_background);
                    player1HealthBar_1 = P1_1.GetComponent<Image>();
                    player1HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P1_2 = Instantiate(Player1HealthUI_fill);
                    player1HealthBar_2 = P1_2.GetComponent<Image>();
                    player1HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P1_3 = Instantiate(Player1HealthUI_outline);
                    player1HealthBar_3 = P1_3.GetComponent<Image>();
                    player1HealthBar_3.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Two:
                {
                    //GameObject P2 = Instantiate(Player2HealthUI);
                    //player2HealthBar = P2.GetComponent<Image>();
                    //player2HealthBar.transform.SetParent(UICanvas, false);
                    GameObject P2_1 = Instantiate(Player2HealthUI_background);
                    player2HealthBar_1 = P2_1.GetComponent<Image>();
                    player2HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P2_2 = Instantiate(Player2HealthUI_fill);
                    player2HealthBar_2 = P2_2.GetComponent<Image>();
                    player2HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P2_3 = Instantiate(Player2HealthUI_outline);
                    player2HealthBar_3 = P2_3.GetComponent<Image>();
                    player2HealthBar_3.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Three:
                {
                    //GameObject P3 = Instantiate(Player3HealthUI);
                    //player3HealthBar = P3.GetComponent<Image>();
                    //player3HealthBar.transform.SetParent(UICanvas, false);
                    GameObject P3_1 = Instantiate(Player3HealthUI_background);
                    player3HealthBar_1 = P3_1.GetComponent<Image>();
                    player3HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P3_2 = Instantiate(Player3HealthUI_fill);
                    player3HealthBar_2 = P3_2.GetComponent<Image>();
                    player3HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P3_3 = Instantiate(Player3HealthUI_outline);
                    player3HealthBar_3 = P3_3.GetComponent<Image>();
                    player3HealthBar_3.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Four:
                {
                    //GameObject P4 = Instantiate(Player4HealthUI);
                    //player4HealthBar = P4.GetComponent<Image>();
                    //player4HealthBar.transform.SetParent(UICanvas, false);
                    GameObject P4_1 = Instantiate(Player4HealthUI_background);
                    player4HealthBar_1 = P4_1.GetComponent<Image>();
                    player4HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P4_2 = Instantiate(Player4HealthUI_fill);
                    player4HealthBar_2 = P4_2.GetComponent<Image>();
                    player4HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P4_3 = Instantiate(Player4HealthUI_outline);
                    player4HealthBar_3 = P4_3.GetComponent<Image>();
                    player4HealthBar_3.transform.SetParent(UICanvas, false);
                }
                break;

            default:
                break;
        }
    }
}