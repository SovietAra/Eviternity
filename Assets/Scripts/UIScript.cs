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
    public GameObject P1GunHeat;
    public GameObject Player2HealthUI_background;
    public GameObject Player2HealthUI_fill;
    public GameObject Player2HealthUI_outline;
    public GameObject P2GunHeat;
    public GameObject Player3HealthUI_background;
    public GameObject Player3HealthUI_fill;
    public GameObject Player3HealthUI_outline;
    public GameObject P3GunHeat;
    public GameObject Player4HealthUI_background;
    public GameObject Player4HealthUI_fill;
    public GameObject Player4HealthUI_outline;
    public GameObject P4GunHeat;
    public GameObject IndicatorPlaneOne;
    public GameObject IndicatorPlaneTwo;
    public GameObject IndicatorPlaneThree;
    public GameObject IndicatorPlaneFour;
    public Image TeamHealthBar;
    public Image TeamHealthBar_Border;
    public Sprite TeamHealthActive;
    public Sprite TeamHealthInactive;

    private GameObject game;
    private GameObject P1IndicatorPlane;
    private GameObject P2IndicatorPlane;
    private GameObject P3IndicatorPlane;
    private GameObject P4IndicatorPlane;
    private GameObject Indicate;
    private GameObject Indicate2;
    private GameObject Indicate3;
    private GameObject Indicate4;
    private float teamhealthAmount;
    private float maxTeamHealth;
    private float maxPlayerHealth;
    private Image player1HealthBar_1;
    private Image player1HealthBar_2;
    private Image player1HealthBar_3;
    private Image player1Heat;
    private Image player2HealthBar_1;
    private Image player2HealthBar_2;
    private Image player2HealthBar_3;
    private Image player2Heat;
    private Image player3HealthBar_1;
    private Image player3HealthBar_2;
    private Image player3HealthBar_3;
    private Image player3Heat;
    private Image player4HealthBar_1;
    private Image player4HealthBar_2;
    private Image player4HealthBar_3;
    private Image player4Heat;
    private bool isHealing = false;
    private List<GameObject> CurrentPlayers;

    // Use this for initialization
    private void Start()
    {
        game = GameObject.Find("Game");
        GameInspector gameInspector = game.GetComponent<GameInspector>();
        maxTeamHealth = gameInspector.MaxTeamHealth;
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

    public void OnSpawn(PlayerIndex index)
    {
        CreateUI(index, UICanvas.transform);
        CurrentPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    public void OnExit(PlayerIndex index)
    {
        CurrentPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        RemoveUI(index);
    }

    public void ActivateTeamBar()
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
                    player1Heat.fillAmount = player.PrimaryHeat() / Weapon.maxHeat;
                    if (Indicate == null)
                    {
                        Indicate = Instantiate(P1IndicatorPlane);
                    }
                    Indicate.transform.SetParent(player.transform, false);
                }
                break;

            case PlayerIndex.Two:
                {
                    player2HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player2Heat.fillAmount = player.PrimaryHeat() / Weapon.maxHeat;
                    if (Indicate2 == null)
                    {
                        Indicate2 = Instantiate(P2IndicatorPlane);
                    }
                    Indicate2.transform.SetParent(player.transform, false);
                }
                break;

            case PlayerIndex.Three:
                {
                    player3HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player3Heat.fillAmount = player.PrimaryHeat() / Weapon.maxHeat;
                    if (Indicate3 == null)
                    {
                        Indicate3 = Instantiate(P3IndicatorPlane);
                    }
                    Indicate3.transform.SetParent(player.transform, false);
                }
                break;

            case PlayerIndex.Four:
                {
                    player4HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player4Heat.fillAmount = player.PrimaryHeat() / Weapon.maxHeat;
                    if (Indicate4 == null)
                    {
                        Indicate4 = Instantiate(P4IndicatorPlane);
                    }
                    Indicate4.transform.SetParent(player.transform, false);
                }
                break;

            default:
                break;
        }
    }

    private void RemoveUI(PlayerIndex index)
    {
        switch (index)
        {
            case PlayerIndex.One:
                {
                    Destroy(player1HealthBar_1.gameObject);
                    Destroy(player1HealthBar_2.gameObject);
                    Destroy(player1HealthBar_3.gameObject);
                    Destroy(player1Heat.gameObject);
                }
                break;
            case PlayerIndex.Two:
                {
                    Destroy(player2HealthBar_1.gameObject);
                    Destroy(player2HealthBar_2.gameObject);
                    Destroy(player2HealthBar_3.gameObject);
                    Destroy(player2Heat.gameObject);
                }
                break;
            case PlayerIndex.Three:
                {
                    Destroy(player3HealthBar_1.gameObject);
                    Destroy(player3HealthBar_2.gameObject);
                    Destroy(player3HealthBar_3.gameObject);
                    Destroy(player3Heat.gameObject);
                }
                break;
            case PlayerIndex.Four:
                {
                    Destroy(player4HealthBar_1.gameObject);
                    Destroy(player4HealthBar_2.gameObject);
                    Destroy(player4HealthBar_3.gameObject);
                    Destroy(player4Heat.gameObject);
                }
                break;

            default:
                break;
        };
    }

    private void CreateUI(PlayerIndex index, Transform UICanvas)
    {

        switch (index)
        {
            case PlayerIndex.One:
                {
                    GameObject P1_1 = Instantiate(Player1HealthUI_background);
                    player1HealthBar_1 = P1_1.GetComponent<Image>();
                    player1HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P1_2 = Instantiate(Player1HealthUI_fill);
                    player1HealthBar_2 = P1_2.GetComponent<Image>();
                    player1HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P1_3 = Instantiate(Player1HealthUI_outline);
                    player1HealthBar_3 = P1_3.GetComponent<Image>();
                    player1HealthBar_3.transform.SetParent(UICanvas, false);
                    P1IndicatorPlane = IndicatorPlaneOne;
                    GameObject Heat1 = Instantiate(P1GunHeat);
                    player1Heat = Heat1.GetComponent<Image>();
                    player1Heat.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Two:
                {
                    GameObject P2_1 = Instantiate(Player2HealthUI_background);
                    player2HealthBar_1 = P2_1.GetComponent<Image>();
                    player2HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P2_2 = Instantiate(Player2HealthUI_fill);
                    player2HealthBar_2 = P2_2.GetComponent<Image>();
                    player2HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P2_3 = Instantiate(Player2HealthUI_outline);
                    player2HealthBar_3 = P2_3.GetComponent<Image>();
                    player2HealthBar_3.transform.SetParent(UICanvas, false);
                    P2IndicatorPlane = IndicatorPlaneTwo;
                    GameObject Heat2 = Instantiate(P2GunHeat);
                    player2Heat = Heat2.GetComponent<Image>();
                    player2Heat.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Three:
                {
                    GameObject P3_1 = Instantiate(Player3HealthUI_background);
                    player3HealthBar_1 = P3_1.GetComponent<Image>();
                    player3HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P3_2 = Instantiate(Player3HealthUI_fill);
                    player3HealthBar_2 = P3_2.GetComponent<Image>();
                    player3HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P3_3 = Instantiate(Player3HealthUI_outline);
                    player3HealthBar_3 = P3_3.GetComponent<Image>();
                    player3HealthBar_3.transform.SetParent(UICanvas, false);
                    P3IndicatorPlane = IndicatorPlaneThree;
                    GameObject Heat3 = Instantiate(P3GunHeat);
                    player3Heat = Heat3.GetComponent<Image>();
                    player3Heat.transform.SetParent(UICanvas, false);
                }
                break;
            case PlayerIndex.Four:
                {
                    GameObject P4_1 = Instantiate(Player4HealthUI_background);
                    player4HealthBar_1 = P4_1.GetComponent<Image>();
                    player4HealthBar_1.transform.SetParent(UICanvas, false);
                    GameObject P4_2 = Instantiate(Player4HealthUI_fill);
                    player4HealthBar_2 = P4_2.GetComponent<Image>();
                    player4HealthBar_2.transform.SetParent(UICanvas, false);
                    GameObject P4_3 = Instantiate(Player4HealthUI_outline);
                    player4HealthBar_3 = P4_3.GetComponent<Image>();
                    player4HealthBar_3.transform.SetParent(UICanvas, false);
                    P4IndicatorPlane = IndicatorPlaneFour;
                    GameObject Heat4 = Instantiate(P4GunHeat);
                    player4Heat = Heat4.GetComponent<Image>();
                    player4Heat.transform.SetParent(UICanvas, false);
                }
                break;

            default:
                break;
        }
    }
}