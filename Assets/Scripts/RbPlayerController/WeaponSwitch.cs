using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : PlayerClient
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
      // Player.notNetworkOwnerEvent += OnNotNetworkOwner;
   }

   // private void OnNotNetworkOwner(ulong id)
   // {
   //    Destroy(this);
   // }

   // void OnDestroy()
   // {
   //    Player.notNetworkOwnerEvent -= OnNotNetworkOwner;
   // }

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
      int prevSelectedWeapon = selectedWeapon;

      if (timeSinceLastSwitch >= switchTime)
      {
         if (selectedWeapon == 0) { selectedWeapon = 1; }
         else { selectedWeapon = 0; }
      }
   }

}