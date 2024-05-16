using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ёто перечисление всех возможных типов оружи€.
/// “акже включает тип "shield", чтобы дать возможность совершенствовать защиту.
/// јббревиатурой [HP] ниже отмечены нереализованные элементы
/// </summary>
public enum WeaponType
{
    none,     // нет оружи€
    blaster,  // простой бластер
    spread,   // веерный бластер
    phaser,   // [HP] волновой бластер
    missile,  // [HP] самонавод€щиес€
    laser,    // [HP] лазер, урон которого зависит от времени воздействи€
    shield    // + к щиту
}

/// <summary>
///  ласс WeaponDefinition позвол€ет настраивать свойства
/// конкретного вида оружи€ в инспекторе. ƒл€ этого класс Main
/// будет хранить массив элементов типа WeaponDefinition
[System.Serializable] // дл€ досутупа к изменени€м в инспеткоре
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // Ѕуква на кубике, изображающа€ бонус
    public Color color = Color.white; // ÷вет ствлоа оружи€ и кубика бонуса
    public GameObject projectilePrefab; // шаблон снар€да
    public Color projectileColor = Color.white; // // цвет снар€да
    public float damageOnHit = 0; // мощь
    public float continuousDamage = 0; // степень разрушени€ в секунду
    public float delayBetweenShots = 0; // период стрельбы
    public float velocity = 20; // скорость снар€да
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // ¬рем€ последнего выстрела
    private Renderer collarRend;

    void Start ()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();
        SetType(_type ); // заменить тип оружи€
        // ƒинамически создавать точку прив€зки дл€ всех снар€дов
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // найти fireDelegate в корневом игровом объекте
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }
  
    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // стрел€ть сразу после установки _type
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return; // ≈сли this.gameObject не активен, выйти
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch(type)
        {
          case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile(); // пр€мо
                p.rigid.velocity = vel;
                p = MakeProjectile(); // влево
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // вправо
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        //ѕолю lastShotTime присваиваетс€ текущее врем€,
        // что предотвращает возможность повторного выстрела раньше,
        // чем через def.delayBetweenShots секунд
        lastShotTime = Time.time;
        return (p);
    }
}