using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    // ��� ������������� �������� ��������� ���� _type
    // � ������������ �������� ������������ ��� ������ ��������
    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }
  
    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (bndCheck.offUp) { Destroy(gameObject); }
    }

    /// <summary>
    /// �������� ������� ���� _type � ������������� ���� ����� �������,
    /// ��� ���������� � WeaponDefinition.
    /// </summary>
    /// <param name="eType">��� WeaponType ������������� ������.</param>
    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}

    /*
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "Enemy")
        {
            Destroy(otherGO);
            Destroy(gameObject);
        }
        else
        {
            print("Projectile hit by non-Enemy: " + otherGO.name);
        }
    }*/

