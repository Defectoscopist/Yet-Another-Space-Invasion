using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part - ������������� �����, ��������������� ��� �������� ������
/// </summary>
[System.Serializable]
public class Part
{
    public string name; // ��� �����
    public float health; // ������� ���������
    public string[] protectedBy; // ������� �����, ���������� ���

    [HideInInspector] public GameObject go;
    [HideInInspector] public Material mat; // ��� �����������
}

/// summary
/// Enemy_4 ��������� �� ������� ��������, �������� ��������� ����� 
/// �� ������ � ������������ � ���. ���������� �� �����, �������� ������ ��������� �����
/// � ���������� ���������, ���� ����� �� ��������� ���
///  </summary>

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // ����� �������


    private Vector3 p0, p1; // ����� ������������
    private float timeStart; // ����� �������� �������
    private float duration = 4; // ���������������� �����������

    void Start()
    {
        // ��������� ������� �������� ��� � <pos> �  Main.SpawnEnemy()
        p0 = p1 = pos;
        InitMovement();

        // �������� � ��� ������� ������ � �������� ������ ����� � parts
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
        // ����� ���������� p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // �������� �����
        timeStart = Time.time;
    }

    public override void Move()
    {
        // � ��� � �������� ������������ ���������
        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // ������� ����������
        pos = (1 - u) * p0 + u * p1; // ������� �������� ������������
    }

    // ��� ��� ������� ��������� ����� ����� � ������� parts
    // �� ����� � ������ �� ������� ������
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

    // ��� ��� ������� ���������� true, ���� ������ ����� ����������
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
        if (prt == null) // ���� ������ �� ����� �� ��������
        {
            return (true); // ������ ��� ���������� (true)
        }
        return (prt.health <= 0); // ������� ��������� ���������
    }

    // ���������� � ������� ����� �������
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
                // ���� ������� �� ��������� ������, �� ���������� ���
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                // ��� ������� �������� ����� ������� ������, � ������� ����� ������
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // contacts[] � ������ ����� �������� ContactPoint
                Part prtHit = FindPart(goHit);
                if (prtHit == null) // ���� prtHit �� ������
                {
                    goHit = coll.contacts[0].otherCollider.gameObject; // ��������� ������ ������
                    prtHit = FindPart(goHit);
                }
                // ���������, �������� �� ����� �������
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s)) // ���� ���� �� ���� �� ���������� ������ ��� �� ���������
                        {
                            Destroy(other); // ���������� ������
                            return; // �����, �� ��������� Enemy_4
                        }
                    }
                }

                // ��� ����� �� ��������, ������� �� �����������
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit; // ������� �������� �� ����� ������
                ShowLocalizedDamage(prtHit.mat); // ��������� �����������
                if (prtHit.health <= 0) // ���� ����� ������� ������
                {
                    prtHit.go.SetActive(false); // �������������� ��
                }

                // ���������, ��������� �� ������� ��������
                bool allDestroyed = true; // ������������, ��� ��������
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt)) // ���� ��� ���-�� ����
                    {
                        allDestroyed = false; // ������������� ����������
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this); // � ������ ������, �������� Main, ��� ������� ��������
                    Destroy(this.gameObject);
                }
                Destroy(other); // ���������� ������
                break;
        }
    }
}