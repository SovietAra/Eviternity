﻿using System;
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

    // Health UI
    public GameObject Player1HealthUI_background;
    public GameObject Player1HealthUI_fill;
    public GameObject Player1HealthUI_outline;
    public GameObject Player1Icon;
    public GameObject Player2HealthUI_background;
    public GameObject Player2HealthUI_fill;
    public GameObject Player2HealthUI_outline;
    public GameObject Player2Icon;
    public GameObject Player3HealthUI_background;
    public GameObject Player3HealthUI_fill;
    public GameObject Player3HealthUI_outline;
    public GameObject Player3Icon;
    public GameObject Player4HealthUI_background;
    public GameObject Player4HealthUI_fill;
    public GameObject Player4HealthUI_outline;
    public GameObject Player4Icon;
    public Image TeamHealthBar;
    public Image TeamHealthBar_Border;
    public Sprite TeamHealthActive;
    public Sprite TeamHealthInactive;
    public Sprite[] PlayerIcons;

    // Weapon UI
    public GameObject P1Weapon_background;
    public GameObject P1Weapon_border;
    public GameObject P1Weapon_icon;
    public GameObject P1WeaponHeat;
    public GameObject P1SecWeapon_background;
    public GameObject P1SecWeapon_border;
    public GameObject P1SecWeapon_icon;
    public GameObject P1SecWeaponHeat;
    public GameObject P2Weapon_background;
    public GameObject P2Weapon_border;
    public GameObject P2Weapon_icon;
    public GameObject P2WeaponHeat;
    public GameObject P2SecWeapon_background;
    public GameObject P2SecWeapon_border;
    public GameObject P2SecWeapon_icon;
    public GameObject P2SecWeaponHeat;
    public GameObject P3Weapon_background;
    public GameObject P3Weapon_border;
    public GameObject P3Weapon_icon;
    public GameObject P3WeaponHeat;
    public GameObject P3SecWeapon_background;
    public GameObject P3SecWeapon_border;
    public GameObject P3SecWeapon_icon;
    public GameObject P3SecWeaponHeat;
    public GameObject P4Weapon_background;
    public GameObject P4Weapon_border;
    public GameObject P4Weapon_icon;
    public GameObject P4WeaponHeat;
    public GameObject P4SecWeapon_background;
    public GameObject P4SecWeapon_border;
    public GameObject P4SecWeapon_icon;
    public GameObject P4SecWeaponHeat;
    public Sprite[] WeaponIcons;

    // Abilities
    public GameObject[] EnergyBars;

    // Other
    public GameObject IndicatorPlaneOne;
    public GameObject IndicatorPlaneTwo;
    public GameObject IndicatorPlaneThree;
    public GameObject IndicatorPlaneFour;

    #region private variables
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
    private Image player1Heat2;
    private Image p1icon;
    //private Image p1weaponIcon;
    //private Image p1secWeaponIcon;
    private Image player1Weapon_1;
    private Image player1Weapon_2;
    private Image player1Weapon_3;
    private Image player1Weapon2_1;
    private Image player1Weapon2_2;
    private Image player1Weapon2_3;
    private Image player2HealthBar_1;
    private Image player2HealthBar_2;
    private Image player2HealthBar_3;
    private Image player2Heat;
    private Image player2Heat2;
    private Image p2icon;
    //private Image p2weaponIcon;
    //private Image p2secWeaponIcon;
    private Image player2Weapon_1;
    private Image player2Weapon_2;
    private Image player2Weapon_3;
    private Image player2Weapon2_1;
    private Image player2Weapon2_2;
    private Image player2Weapon2_3;
    private Image player3HealthBar_1;
    private Image player3HealthBar_2;
    private Image player3HealthBar_3;
    private Image player3Heat;
    private Image player3Heat2;
    private Image p3icon;
    //private Image p3weaponIcon;
    //private Image p3secWeaponIcon;
    private Image player3Weapon_1;
    private Image player3Weapon_2;
    private Image player3Weapon_3;
    private Image player3Weapon2_1;
    private Image player3Weapon2_2;
    private Image player3Weapon2_3;
    private Image player4HealthBar_1;
    private Image player4HealthBar_2;
    private Image player4HealthBar_3;
    private Image player4Heat;
    private Image player4Heat2;
    private Image p4icon;
    //private Image p4weaponIcon;
    //private Image p4secWeaponIcon;
    private Image player4Weapon_1;
    private Image player4Weapon_2;
    private Image player4Weapon_3;
    private Image player4Weapon2_1;
    private Image player4Weapon2_2;
    private Image player4Weapon2_3;
    private bool isHealing = false;
    private bool P1iconsSet = false;
    private bool P2iconsSet = false;
    private bool P3iconsSet = false;
    private bool P4iconsSet = false;
    private List<GameObject> CurrentPlayers;
    private Image[] AbilityBars = new Image[12];
    #endregion

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
                    UpdateHealth(player.GetComponent<Player>(), player.GetComponent<DamageAbleObject>(), player);
            }

            if (UICanvas.activeInHierarchy == false)
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
        else
        {
            UICanvas.SetActive(false);
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

    private void UpdateHealth(Player playerScript, DamageAbleObject damageAbleObject, GameObject player)
    {
        switch (playerScript.Index)
        {
            case PlayerIndex.One:
                {
                    player1HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player1Heat.fillAmount = playerScript.PrimaryHeat / playerScript.PrimaryMaxHeat;

                    for (int j = 0; j < 3; j++)
                    {
                        AbilityBars[j].fillAmount = playerScript.AbilityEnergy(j + 1) / playerScript.MaxEnergy;
                    }

                    if (Indicate == null)
                    {
                        Indicate = Instantiate(P1IndicatorPlane);
                    }
                    
                    if (P1iconsSet == false)
                    {
                        SetIcon(player, PlayerIndex.One);
                        SetWeaponIcon(player, PlayerIndex.One);
                        //SetAbilityIcon(player, PlayerIndex.One);
                        P1iconsSet = true;
                    }

                    Indicate.transform.SetParent(playerScript.transform, false);
                }
                break;

            case PlayerIndex.Two:
                {
                    player2HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player2Heat.fillAmount = playerScript.PrimaryHeat / playerScript.PrimaryMaxHeat;

                    for (int j = 3; j < 6; j++)
                    {
                        AbilityBars[j].fillAmount = playerScript.AbilityEnergy(j - 2) / playerScript.MaxEnergy;
                    }

                    if (Indicate2 == null)
                    {
                        Indicate2 = Instantiate(P2IndicatorPlane);
                    }

                    if (P2iconsSet == false)
                    {
                        SetIcon(player, PlayerIndex.Two);
                        P2iconsSet = true;
                    }
                    Indicate2.transform.SetParent(playerScript.transform, false);
                }
                break;

            case PlayerIndex.Three:
                {
                    player3HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player3Heat.fillAmount = playerScript.PrimaryHeat / playerScript.PrimaryMaxHeat;

                    for (int j = 6; j < 9; j++)
                    {
                        AbilityBars[j].fillAmount = playerScript.AbilityEnergy(j - 5) / playerScript.MaxEnergy;
                    }

                    if (Indicate3 == null)
                    {
                        Indicate3 = Instantiate(P3IndicatorPlane);
                    }

                    if (P3iconsSet == false)
                    {
                        SetIcon(player, PlayerIndex.Three);
                        P3iconsSet = true;
                    }
                    Indicate3.transform.SetParent(playerScript.transform, false);
                }
                break;

            case PlayerIndex.Four:
                {
                    player4HealthBar_2.fillAmount = damageAbleObject.Health / maxPlayerHealth;
                    player4Heat.fillAmount = playerScript.PrimaryHeat / playerScript.PrimaryMaxHeat;

                    for (int j = 9; j < 12; j++)
                    {
                        AbilityBars[j].fillAmount = playerScript.AbilityEnergy(j - 8) / playerScript.MaxEnergy;
                    }

                    if (Indicate4 == null)
                    {
                        Indicate4 = Instantiate(P4IndicatorPlane);
                    }

                    if (P4iconsSet == false)
                    {
                        SetIcon(player, PlayerIndex.Four);
                        P4iconsSet = true;
                    }
                    Indicate4.transform.SetParent(playerScript.transform, false);
                }
                break;

            default:
                break;
        }
    }

    private void SetWeaponIcon(GameObject player, PlayerIndex index)
    {
        switch (index)
        {
            case PlayerIndex.One:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        player1Weapon_2.sprite = WeaponIcons[0];
                        player1Weapon2_2.sprite = WeaponIcons[1];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        player1Weapon_2.sprite = WeaponIcons[2];
                        player1Weapon2_2.sprite = WeaponIcons[3];
                    }
                }
                break;
            case PlayerIndex.Two:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        player2Weapon_2.sprite = WeaponIcons[0];
                        player2Weapon2_2.sprite = WeaponIcons[1];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        player2Weapon_2.sprite = WeaponIcons[2];
                        player2Weapon2_2.sprite = WeaponIcons[3];
                    }
                }
                break;
            case PlayerIndex.Three:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        player3Weapon_2.sprite = WeaponIcons[0];
                        player3Weapon2_2.sprite = WeaponIcons[1];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        player3Weapon_2.sprite = WeaponIcons[2];
                        player3Weapon2_2.sprite = WeaponIcons[3];
                    }
                }
                break;
            case PlayerIndex.Four:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        player4Weapon_2.sprite = WeaponIcons[0];
                        player4Weapon2_2.sprite = WeaponIcons[1];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        player4Weapon_2.sprite = WeaponIcons[2];
                        player4Weapon2_2.sprite = WeaponIcons[3];
                    }
                }
                break;
            default:
                break;
        }
    }

    private void SetIcon(GameObject player, PlayerIndex index)
    {
        //print("setting icon");
        switch (index)
        {
            case PlayerIndex.One:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        p1icon.sprite = PlayerIcons[0];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        p1icon.sprite = PlayerIcons[1];
                    }
                }
                break;
            case PlayerIndex.Two:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        p2icon.sprite = PlayerIcons[2];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        p2icon.sprite = PlayerIcons[3];
                    }
                }
                break;
            case PlayerIndex.Three:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        p3icon.sprite = PlayerIcons[4];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        p3icon.sprite = PlayerIcons[5];
                    }
                }
                break;
            case PlayerIndex.Four:
                {
                    if (player.name == ("PlayerClassAegis(Clone)"))
                    {
                        p4icon.sprite = PlayerIcons[6];
                    }
                    else if (player.name == ("PlayerClassStalker(Clone)"))
                    {
                        p4icon.sprite = PlayerIcons[7];
                    }
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
                    Destroy(player1Heat2.gameObject);
                    Destroy(player1Weapon_1.gameObject);
                    Destroy(player1Weapon_2.gameObject);
                    Destroy(player1Weapon_3.gameObject);
                    Destroy(player1Weapon2_1.gameObject);
                    Destroy(player1Weapon2_2.gameObject);
                    Destroy(player1Weapon2_3.gameObject);
                    Destroy(p1icon.gameObject);
                    P1iconsSet = false;

                    for (int k = 0; k < 3; k++)
                    {
                        Destroy(AbilityBars[k].gameObject);
                    }
                }
                break;
            case PlayerIndex.Two:
                {
                    Destroy(player2HealthBar_1.gameObject);
                    Destroy(player2HealthBar_2.gameObject);
                    Destroy(player2HealthBar_3.gameObject);
                    Destroy(player2Heat.gameObject);
                    Destroy(player2Heat2.gameObject);
                    Destroy(player2Weapon_1.gameObject);
                    Destroy(player2Weapon_2.gameObject);
                    Destroy(player2Weapon_3.gameObject);
                    Destroy(player2Weapon2_1.gameObject);
                    Destroy(player2Weapon2_2.gameObject);
                    Destroy(player2Weapon2_3.gameObject);
                    Destroy(p2icon.gameObject);
                    P2iconsSet = false;

                    for (int k = 3; k < 6; k++)
                    {
                        Destroy(AbilityBars[k].gameObject);
                    }
                }
                break;
            case PlayerIndex.Three:
                {
                    Destroy(player3HealthBar_1.gameObject);
                    Destroy(player3HealthBar_2.gameObject);
                    Destroy(player3HealthBar_3.gameObject);
                    Destroy(player3Heat.gameObject);
                    Destroy(player3Heat2.gameObject);
                    Destroy(player3Weapon_1.gameObject);
                    Destroy(player3Weapon_2.gameObject);
                    Destroy(player3Weapon_3.gameObject);
                    Destroy(player3Weapon2_1.gameObject);
                    Destroy(player3Weapon2_2.gameObject);
                    Destroy(player3Weapon2_3.gameObject);
                    Destroy(p3icon.gameObject);
                    P3iconsSet = false;

                    for (int k = 6; k < 9; k++)
                    {
                        Destroy(AbilityBars[k].gameObject);
                    }
                }
                break;
            case PlayerIndex.Four:
                {
                    Destroy(player4HealthBar_1.gameObject);
                    Destroy(player4HealthBar_2.gameObject);
                    Destroy(player4HealthBar_3.gameObject);
                    Destroy(player4Heat.gameObject);
                    Destroy(player4Heat2.gameObject);
                    Destroy(player4Weapon_1.gameObject);
                    Destroy(player4Weapon_2.gameObject);
                    Destroy(player4Weapon_3.gameObject);
                    Destroy(player4Weapon2_1.gameObject);
                    Destroy(player4Weapon2_2.gameObject);
                    Destroy(player4Weapon2_3.gameObject);
                    Destroy(p4icon.gameObject);
                    P4iconsSet = false;

                    for (int k = 9; k < 12; k++)
                    {
                        Destroy(AbilityBars[k].gameObject);
                    }
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
                    GameObject P1_3 = Instantiate(Player1Icon);
                    p1icon = P1_3.GetComponent<Image>();
                    p1icon.transform.SetParent(UICanvas, false);
                    GameObject P1_4 = Instantiate(Player1HealthUI_outline);
                    player1HealthBar_3 = P1_4.GetComponent<Image>();
                    player1HealthBar_3.transform.SetParent(UICanvas, false);
                    P1IndicatorPlane = IndicatorPlaneOne;
                    GameObject Heat1_1 = Instantiate(P1Weapon_background);
                    player1Weapon_1 = Heat1_1.GetComponent<Image>();
                    player1Weapon_1.transform.SetParent(UICanvas, false);
                    GameObject Heat1_2 = Instantiate(P1Weapon_icon);
                    player1Weapon_2 = Heat1_2.GetComponent<Image>();
                    player1Weapon_2.transform.SetParent(UICanvas, false);
                    GameObject Heat1_3 = Instantiate(P1WeaponHeat);
                    player1Heat = Heat1_3.GetComponent<Image>();
                    player1Heat.transform.SetParent(UICanvas, false);
                    GameObject Heat1_4 = Instantiate(P1Weapon_border);
                    player1Weapon_3 = Heat1_4.GetComponent<Image>();
                    player1Weapon_3.transform.SetParent(UICanvas, false);
                    GameObject Heat5_1 = Instantiate(P1SecWeapon_background);
                    player1Weapon2_1 = Heat5_1.GetComponent<Image>();
                    player1Weapon2_1.transform.SetParent(UICanvas, false);
                    GameObject Heat5_2 = Instantiate(P1SecWeapon_icon);
                    player1Weapon2_2 = Heat5_2.GetComponent<Image>();
                    player1Weapon2_2.transform.SetParent(UICanvas, false);
                    GameObject Heat5_3 = Instantiate(P1SecWeaponHeat);
                    player1Heat2 = Heat5_3.GetComponent<Image>();
                    player1Heat2.transform.SetParent(UICanvas, false);
                    GameObject Heat5_4 = Instantiate(P1SecWeapon_border);
                    player1Weapon2_3 = Heat5_4.GetComponent<Image>();
                    player1Weapon2_3.transform.SetParent(UICanvas, false);

                    for (int i = 0; i < 3; i++)
                    {
                        GameObject Energy = Instantiate(EnergyBars[i]);
                        AbilityBars[i] = Energy.GetComponent<Image>();
                        AbilityBars[i].transform.SetParent(UICanvas, false);
                    }
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
                    GameObject P2_3 = Instantiate(Player2Icon);
                    p2icon = P2_3.GetComponent<Image>();
                    p2icon.transform.SetParent(UICanvas, false);
                    GameObject P2_4 = Instantiate(Player2HealthUI_outline);
                    player2HealthBar_3 = P2_4.GetComponent<Image>();
                    player2HealthBar_3.transform.SetParent(UICanvas, false);
                    P2IndicatorPlane = IndicatorPlaneTwo;
                    GameObject Heat2_1 = Instantiate(P2Weapon_background);
                    player2Weapon_1 = Heat2_1.GetComponent<Image>();
                    player2Weapon_1.transform.SetParent(UICanvas, false);
                    GameObject Heat2_2 = Instantiate(P2Weapon_icon);
                    player2Weapon_2 = Heat2_2.GetComponent<Image>();
                    player2Weapon_2.transform.SetParent(UICanvas, false);
                    GameObject Heat2_3 = Instantiate(P2WeaponHeat);
                    player2Heat = Heat2_3.GetComponent<Image>();
                    player2Heat.transform.SetParent(UICanvas, false);
                    GameObject Heat2_4 = Instantiate(P2Weapon_border);
                    player2Weapon2_3 = Heat2_4.GetComponent<Image>();
                    player2Weapon2_3.transform.SetParent(UICanvas, false);
                    GameObject Heat6_1 = Instantiate(P2SecWeapon_background);
                    player2Weapon2_1 = Heat6_1.GetComponent<Image>();
                    player2Weapon2_1.transform.SetParent(UICanvas, false);
                    GameObject Heat6_2 = Instantiate(P2SecWeapon_icon);
                    player2Weapon2_2 = Heat6_2.GetComponent<Image>();
                    player2Weapon2_2.transform.SetParent(UICanvas, false);
                    GameObject Heat6_3 = Instantiate(P2SecWeaponHeat);
                    player2Heat2 = Heat6_3.GetComponent<Image>();
                    player2Heat2.transform.SetParent(UICanvas, false);
                    GameObject Heat6_4 = Instantiate(P2SecWeapon_border);
                    player2Weapon2_3 = Heat6_4.GetComponent<Image>();
                    player2Weapon2_3.transform.SetParent(UICanvas, false);

                    for (int i = 3; i < 6; i++)
                    {
                        GameObject Energy = Instantiate(EnergyBars[i]);
                        AbilityBars[i] = Energy.GetComponent<Image>();
                        AbilityBars[i].transform.SetParent(UICanvas, false);
                    }
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
                    GameObject P3_3 = Instantiate(Player3Icon);
                    p3icon = P3_3.GetComponent<Image>();
                    p3icon.transform.SetParent(UICanvas, false);
                    GameObject P3_4 = Instantiate(Player3HealthUI_outline);
                    player3HealthBar_3 = P3_4.GetComponent<Image>();
                    player3HealthBar_3.transform.SetParent(UICanvas, false);
                    P3IndicatorPlane = IndicatorPlaneThree;
                    GameObject Heat3_1 = Instantiate(P3Weapon_background);
                    player3Weapon_1 = Heat3_1.GetComponent<Image>();
                    player3Weapon_1.transform.SetParent(UICanvas, false);
                    GameObject Heat3_2 = Instantiate(P3Weapon_icon);
                    player3Weapon_2 = Heat3_2.GetComponent<Image>();
                    player3Weapon_2.transform.SetParent(UICanvas, false);
                    GameObject Heat3_3 = Instantiate(P3WeaponHeat);
                    player3Heat = Heat3_3.GetComponent<Image>();
                    player3Heat.transform.SetParent(UICanvas, false);
                    GameObject Heat3_4 = Instantiate(P3Weapon_border);
                    player3Weapon_3 = Heat3_4.GetComponent<Image>();
                    player3Weapon_3.transform.SetParent(UICanvas, false);
                    GameObject Heat7_1 = Instantiate(P3SecWeapon_background);
                    player3Weapon_1 = Heat7_1.GetComponent<Image>();
                    player3Weapon_1.transform.SetParent(UICanvas, false);
                    GameObject Heat7_2 = Instantiate(P3SecWeapon_icon);
                    player3Weapon_2 = Heat7_2.GetComponent<Image>();
                    player3Weapon_2.transform.SetParent(UICanvas, false);
                    GameObject Heat7_3 = Instantiate(P3SecWeaponHeat);
                    player3Heat = Heat7_3.GetComponent<Image>();
                    player3Heat.transform.SetParent(UICanvas, false);
                    GameObject Heat7_4 = Instantiate(P3SecWeapon_border);
                    player3Weapon_3 = Heat7_4.GetComponent<Image>();
                    player3Weapon_3.transform.SetParent(UICanvas, false);

                    for (int i = 6; i < 9; i++)
                    {
                        GameObject Energy = Instantiate(EnergyBars[i]);
                        AbilityBars[i] = Energy.GetComponent<Image>();
                        AbilityBars[i].transform.SetParent(UICanvas, false);
                    }
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
                    GameObject P4_3 = Instantiate(Player4Icon);
                    p4icon = P4_3.GetComponent<Image>();
                    p4icon.transform.SetParent(UICanvas, false);
                    GameObject P4_4 = Instantiate(Player4HealthUI_outline);
                    player4HealthBar_3 = P4_4.GetComponent<Image>();
                    player4HealthBar_3.transform.SetParent(UICanvas, false);
                    P4IndicatorPlane = IndicatorPlaneFour;
                    GameObject Heat4_1 = Instantiate(P4Weapon_background);
                    player4Weapon_1 = Heat4_1.GetComponent<Image>();
                    player4Weapon_1.transform.SetParent(UICanvas, false);
                    GameObject Heat4_2 = Instantiate(P4Weapon_icon);
                    player4Weapon_2 = Heat4_2.GetComponent<Image>();
                    player4Weapon_2.transform.SetParent(UICanvas, false);
                    GameObject Heat4_3 = Instantiate(P4WeaponHeat);
                    player4Heat = Heat4_3.GetComponent<Image>();
                    player4Heat.transform.SetParent(UICanvas, false);
                    GameObject Heat4_4 = Instantiate(P4Weapon_border);
                    player4Weapon_3 = Heat4_4.GetComponent<Image>();
                    player4Weapon_3.transform.SetParent(UICanvas, false);
                    GameObject Heat8_1 = Instantiate(P4SecWeapon_background);
                    player4Weapon2_1 = Heat8_1.GetComponent<Image>();
                    player4Weapon2_1.transform.SetParent(UICanvas, false);
                    GameObject Heat8_2 = Instantiate(P4SecWeapon_icon);
                    player4Weapon2_2 = Heat8_2.GetComponent<Image>();
                    player4Weapon2_2.transform.SetParent(UICanvas, false);
                    GameObject Heat8_3 = Instantiate(P4SecWeaponHeat);
                    player4Heat2 = Heat8_3.GetComponent<Image>();
                    player4Heat2.transform.SetParent(UICanvas, false);
                    GameObject Heat8_4 = Instantiate(P4SecWeapon_border);
                    player4Weapon2_3 = Heat8_4.GetComponent<Image>();
                    player4Weapon2_3.transform.SetParent(UICanvas, false);

                    for (int i = 9; i < 12; i++)
                    {
                        GameObject Energy = Instantiate(EnergyBars[i]);
                        AbilityBars[i] = Energy.GetComponent<Image>();
                        AbilityBars[i].transform.SetParent(UICanvas, false);
                    }
                }
                break;

            default:
                break;
        }
    }
}