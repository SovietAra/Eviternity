using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSwitch : MonoBehaviour
{
    [SerializeField]
    public GameObject Weapon1;
    [SerializeField]
    public GameObject Weapon2;

    public bool showWeapon1;
    public bool showWeapon2;
    // Use this for initialization
    void Start()
    {
        showWeapon1 = false;
        showWeapon2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (showWeapon1 == false)
        {
            Weapon1.SetActive(false);
        }
        if (showWeapon1)
        {
            Weapon1.SetActive(true);
        }
        if (showWeapon2 == false)
        {
            Weapon2.SetActive(false);
        }
        if (showWeapon2)
        {
            Weapon2.SetActive(true);
        }
        //Get Weapon 1
        if (Input.GetKeyDown(KeyCode.Alpha1) && showWeapon1 == false)
        {
            showWeapon1 = true;
            showWeapon2 = false;
        }

        //Get Weapon 2
        if (Input.GetKeyDown(KeyCode.Alpha2) && showWeapon2 == false)
        {
            showWeapon2 = true;
            showWeapon1 = false;
        }

        //Disable all Weapons
        if (Input.GetKeyDown(KeyCode.R))
        {
            showWeapon2 = false;
            showWeapon1 = false;
        }
    }
}