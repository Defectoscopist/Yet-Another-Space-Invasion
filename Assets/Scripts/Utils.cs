using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===================== Функции для работы с материалами =====================\\

public class Utils : MonoBehaviour
{
    // Возвращает список всех материалов в данном игровом объекте
    // и в его дочерних объектах
    static public Material[] GetAllMaterials(GameObject go)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)
        {
            mats.Add(rend.material);
        }
        return (mats.ToArray());
    }
}