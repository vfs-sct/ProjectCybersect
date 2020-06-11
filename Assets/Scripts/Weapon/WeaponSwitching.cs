// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private int selectedWeapon = 0;

    private GameManager gameManager = null;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        //checks if the game is paused if not check selected weapons
        if(gameManager.isPaused) return;
        ScrollSelectWeapon();
        ManuelSelectWeapon();
    }

    private void ScrollSelectWeapon()
    {
        int previousSelectedWeapon = selectedWeapon;

        //check scroll wheel down
        if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }

        //check scroll wheel up
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(selectedWeapon <= 0)
                selectedWeapon = transform.childCount -1;
            else
                selectedWeapon--;
        }

        if(previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void ManuelSelectWeapon()
    {
        //selects weapon, also checks to see if the weapon exists
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 5)
        {
            selectedWeapon = 4;
        }

        SelectWeapon();
    }

    private void SelectWeapon()
    {
        //activate selected weapon and deactivates unselected weapon
        int i = 0;
        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
