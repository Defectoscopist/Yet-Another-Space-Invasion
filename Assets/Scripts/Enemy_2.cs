using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // регулируем синосуидальный характер движения
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;

    // регулируем линейную интерполяцию
    [Header("Set Dynamically: Enemy_2")]
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    void Start()
    {
        // выбрать случайную точку на левой границе экрана
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // выбрать случайную точку на правой границе экрана
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth - bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // случайно поменять их местами
        if (Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }

        // Запоминаем время спавна
        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;
        // Уничтожаем, если корабль живет дольше чем надо
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        // Магия перемещения по синусоиде
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //Интерполяция местоположения между двумя точками
        pos = (1 - u) * p0 + u * p1;
    }
}