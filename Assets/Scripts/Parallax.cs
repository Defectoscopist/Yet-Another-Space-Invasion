using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject poi; // Корабль игрока
    public GameObject[] panels; // прокручиваемая панель переднего плана
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f; // степень реакции панели на перемещение корабля
    private float panelHt; // высота каждой панели
    private float depth; // глубина каждой панели

    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        // Установить панели в начальные позиции
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, 0, depth);
    }

    void Update()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);

        if (poi != null)
        {
            tX = -poi.transform.position.x * motionMult;
        }

        // Сместить panels[0]
        panels[0].transform.position = new Vector3(tX, tY, depth);
        // Сместить panels[1], эффект непрерывности звездного поля
        if (tY >= 0)
        {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
        }
    }
}




