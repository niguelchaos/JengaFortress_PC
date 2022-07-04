using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSwitch : PlayerClient
{
   // private enum State { One, Two }
   // [SerializeField] private State state = State.One;

   [SerializeField] private Transform[] weapons;

   [SerializeField] private float switchTime;
   [SerializeField] private float timeSinceLastSwitch;
   
   [SerializeField] private int selectedWeapon = 0;


    
    
   private void Start()
   {
      SetWeapons();
      Select(selectedWeapon);
      timeSinceLastSwitch = 0f;
   }


   private void Update()
   {

   }

   private void SetWeapons()
   {
      // attached to weapon holder
      // each child is weapon
      weapons = new Transform[transform.childCount];

      for (int i = 0; i < transform.childCount; i++)
      {
         weapons[i] = transform.GetChild(i);
      }

      // dynamically set buttons
      // if (keys == null) keys = new KeyCode[weapons.Length];
      if (weapons.Length > 1)
      {
         print("adding weapons " + weapons.Length);
         InputManager.Instance.AddWeaponNumberBindings(weapons.Length);
      }
   }

   private void Select(int weaponIndex)
   {
      for (int i = 0; i < weapons.Length; i++)
      {
         weapons[i].gameObject.SetActive(i == weaponIndex);
      }

      timeSinceLastSwitch = 0f;

      OnWeaponSelected();
   }

   private void OnWeaponSelected()
   {

   }

   private void ChangeWeapon()
   {

      // if (InputManager.Instance.weaponSwapInput != selectedWeapon)
      // {
         int prevSelectedWeapon = selectedWeapon;

         if (timeSinceLastSwitch >= switchTime)
         {
            Select(selectedWeapon);
         }
         timeSinceLastSwitch += Time.deltaTime;
      // }
   }

}