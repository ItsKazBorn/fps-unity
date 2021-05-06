using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    private List<Weapon> weapons = new List<Weapon>();
    
    public int selectedWeapon = 0;
    
    void Start()
    {
        foreach (Transform obj in transform)
        {
            Weapon weapon = obj.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }
        }
        SelectWeapon(0);
    }

    void Update()
    {
        ChangeSelectedWeapon(Input.GetAxis("Mouse ScrollWheel"));
    }

    public void ChangeSelectedWeapon(float axis)
    {
        int previousWeapon = selectedWeapon;
        if (axis > 0f)
        {
            if (selectedWeapon >= weapons.Count - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        else if (axis < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = weapons.Count - 1;
            else
                selectedWeapon--;
        }

        if (previousWeapon != selectedWeapon)
            SelectWeapon(previousWeapon);
    }
    
    void SelectWeapon(int previousWeapon)
    {
        weapons[previousWeapon].gameObject.SetActive(false);
        weapons[selectedWeapon].gameObject.SetActive(true);
    }

    public Weapon GetSelectedWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                return weapon.GetComponent<Weapon>();
            i++;
        }
        return null;
    }
}
