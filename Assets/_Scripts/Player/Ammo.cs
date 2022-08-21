using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: SKIT I DENNA HELT JUST NU, SPELAR JU FAN INGEN ROLL!
// todo: borde troligtvis ärva från typ Item, men vem bryr sig...

// nice relic of the course 10/10 requires more refactors

[SerializeField]
public class ProjectileInfo : MonoBehaviour
{
    // public string name; // key
    public string displayName;
    public GameObject ammoPrefab;
    public int defaultAmmoCount;

    public int count {
        get { return count; }
        private set {
            count = value;
            // if (count <= 0)
            //     Destroy(gameObject);
        }
    }


    void Start()
    {
        
    }

    public static ProjectileInfo CreateFromJson(string jsonString)
    {
        ProjectileInfo proj = JsonUtility.FromJson<ProjectileInfo>(jsonString);
        proj.count = proj.defaultAmmoCount;
        return proj;
    }
        
}

[SerializeField]
public class Inventory : MonoBehaviour
{
    private Dictionary<string, ProjectileInfo> ammo;


    void Start()
    {
        ammo = new Dictionary<string, ProjectileInfo>();
        ProjectileInfo regularBlock = ProjectileInfo.CreateFromJson(
                "{\"name\":\"block\",\"displayName\":\"Bullet\",\"defaultAmmoCount\":10,\"ammoPrefab\":\"Prefabs/Bullet\"}"
            );
        ProjectileInfo explosiveBlock = ProjectileInfo.CreateFromJson(
                "{\"name\":\"explosiveBlock\",\"displayName\":\"Bullet\",\"defaultAmmoCount\":10,\"ammoPrefab\":\"Prefabs/Bullet\"}"
            );
        ProjectileInfo iceBlock = ProjectileInfo.CreateFromJson(
                "{\"name\":\"iceBlock\",\"displayName\":\"Bullet\",\"defaultAmmoCount\":10,\"ammoPrefab\":\"Prefabs/Bullet\"}"
            );

        ammo.Add("regularBlock", regularBlock);
        ammo.Add("regularBlock", regularBlock);
        ammo.Add("regularBlock", regularBlock);
    }

    //public static Ammo Create
}