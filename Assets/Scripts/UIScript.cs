using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class UIScript : MonoBehaviour {
    // GameInspeector: Calls function to Add or Remove the UI
    public GameObject UICanvas;
    public GameObject Player1HealthUI;
    public GameObject Player2HealthUI;
    public GameObject Player3HealthUI;
    public GameObject Player4HealthUI;
    public int playerCount;
    public int prevPlayerCount;
    private Canvas canvas;
    private Slider P1healthSlider;
    private Slider P2healthSlider;
    private Slider P3healthSlider;
    private Slider P4healthSlider;
    // Use this for initialization
    void Start () {
        //GameObject UIparent = Resources.Load("Canvas") as GameObject;
        //object1 = Instantiate(UIparent);
        GameObject newCanvas = Instantiate(UICanvas);
        canvas = UICanvas.GetComponent<Canvas>();
    }
	
	// Update is called once per frame
	void Update () {
        List<GameObject> CurrentPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        playerCount = CurrentPlayers.Count;

        if (CurrentPlayers != null && playerCount != prevPlayerCount)
        {
            CreateUI(playerCount, canvas);
        }

        foreach (GameObject player in CurrentPlayers)
        {
            UpdateHealth(player.GetComponent<Player>());
        }

        prevPlayerCount = playerCount;
    }

    private void UpdateHealth(Player player)
    {
        //player.Index == PlayerIndex.One;
        switch (player.Index)
        {
            case PlayerIndex.One:
                {
                    P1healthSlider.value = player.health;
                }
                break;
            case PlayerIndex.Two:
                {
                    P2healthSlider.value = player.health;
                }
                break;
            case PlayerIndex.Three:
                {
                    P3healthSlider.value = player.health;
                }
                break;
            case PlayerIndex.Four:
                {
                    P4healthSlider.value = player.health;
                }
                break;

            default:
                break;
        }
    }

    private void CreateUI(int playerCount, Canvas canvas)
    {
        switch (playerCount)
        {
            case 1:
                {
                    GameObject P1 = Instantiate(Player1HealthUI);
                    P1healthSlider = P1.GetComponent<Slider>();
                }
                break;
            case 2:
                {
                    GameObject P2 = Instantiate(Player2HealthUI, canvas.transform);
                }
                break;
            case 3:
                {
                    GameObject P3 = Instantiate(Player3HealthUI, canvas.transform);
                }
                break;
            case 4:
                {
                    GameObject P4 = Instantiate(Player4HealthUI, canvas.transform);
                }
                break;
            default:
                break;
        }
    }
}
