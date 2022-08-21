using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName ="Weapon/Gun")]
public class GunData : ScriptableObject
{
    public new string name;

    public float damage;
    public float maxDistance;
    
    public int currentAmmo;
    public int magSize;
    
    // Rounds per minute 
    public float fireRate;
    public float reloadTime;

    public float fireBlockForce = 10000;

}
