using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part - сериализуемый класс, предназначенный для хранения данных
/// </summary>
[System.Serializable]
public class Part
{
    public string name; // имя части
    public float health; // степень стойкости
    public string[] protectedBy; // другиче части, защищающие эту

    [HideInInspector] public GameObject go;
    [HideInInspector] public Material mat; // для повреждений
}

/// summary
/// Enemy_4 создается за верхней границей, выбирает случайную точку 
/// на экране и перемещается к ней. Добравшись до места, выбирает другую случайную точку
/// и продолжает двигаться, пока игрок не уничтожит его
///  </summary>

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // части корабля


    private Vector3 p0, p1; // точки интерполяции
    private float timeStart; // Время создания корабля
    private float duration = 4; // Продолжитеьномть перемещения

    void Start()
    {
        // Начальная позиция записана уже в <pos> в  Main.SpawnEnemy()
        p0 = p1 = pos;
        InitMovement();

        // записать в кэш игровой объект и материал каждой части в parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;
        // новые координаты p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Сбросить время
        timeStart = Time.time;
    }

    public override void Move()
    {
        // а вот и линейная интерполяция подъехала
        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // плавное замедление
        pos = (1 - u) * p0 + u * p1; // простая линейная интерполяция
    }

    // Эти две функции выполняют поиск части в массиве parts
    // по имени и сслыке на игровой объект
    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    // Эти две функции возвращают true, если данная часть уничтожена
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt)
    {
        if (prt == null) // Если ссылка на часть не передана
        {
            return (true); // значит она уничтожена (true)
        }
        return (prt.health <= 0); // Вернуть результат сравнения
    }

    // окрашивает в красный часть корабля
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
    private void OnCollisionEnter(Collision coll)
    {
    
    GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // Если корабль за границами экрана, не повреждать его
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                // Эта функция пытается найти игровой объект, в который попал снаряд
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // contacts[] — массив точек контакта ContactPoint
                Part prtHit = FindPart(goHit);
                if (prtHit == null) // Если prtHit не найден
                {
                    goHit = coll.contacts[0].otherCollider.gameObject; // проверить другой объект
                    prtHit = FindPart(goHit);
                }
                // проверить, защищена ли часть корабля
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s)) // Если хотя бы одна из защищающих частей еще не разрушена
                        {
                            Destroy(other); // уничтожить снаряд
                            return; // выйти, не повреждая Enemy_4
                        }
                    }
                }

                // Эта часть не защищена, нанести ей повреждение
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit; // вычесть здоровье от урона оружия
                ShowLocalizedDamage(prtHit.mat); // засветить повреждение
                if (prtHit.health <= 0) // если часть корабля добита
                {
                    prtHit.go.SetActive(false); // деактивировать ее
                }

                // проверить, полностью ли корабль разрушен
                bool allDestroyed = true; // Предположить, что разрушен
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt)) // если еще что-то живо
                    {
                        allDestroyed = false; // предположение отвергнуто
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this); // в другом случае, сообщаем Main, что корабль разрушен
                    Destroy(this.gameObject);
                }
                Destroy(other); // уничтожить снаряд
                break;
        }
    }
}