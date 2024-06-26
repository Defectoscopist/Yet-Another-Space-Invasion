using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    public float waveFrequency = 2f; // ����� ����� ���������
    public float waveWidth = 4; // ������
    public float waveRotY = 45;

    private float x0; // ��������� x
    private float birthTime;

    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }

    // override - ��������������
    // ������ �������� �� �����������
    public override void Move()
    {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        // ���������� base ��� ������� �� Y, ��� � ���� � ����������� Enemy
        base.Move();

        print(bndCheck.isOnScreen);// ������� � protected � private
    }
}