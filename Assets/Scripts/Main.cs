using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f; // отступ Enemy от экрана
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {
                                                                WeaponType.blaster, WeaponType.blaster,
                                                                WeaponType.spread, WeaponType.shield
                                                            };
    private BoundsCheck bndCheck;

    // Сгенерировать бонус с заданной вероятностью
    public void ShipDestroyed(Enemy e)
    {
        if (Random.value <= e.powerUpDropChance)
        {
            // Выбрать тип бонуса
            int ndx = Random.Range(0, powerUpFrequency.Length);
            // Выбрать один из элементов powerUpFrequency
            WeaponType puType = powerUpFrequency[ndx];

            // Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            // Установить на месте разрушенного корбаля
            pu.transform.position = e.transform.position;
        }
    }

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
        // Словарь с ключами типа WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
          WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length); // случайный шаблон Enemy
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // Начальные координаты Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond); // и по новой
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>
    /// Статическая функция, возвращающая WeaponDefinition из статического
    /// защищенного поля WEAP_DICT класса Main.
    /// </summary>
    /// <returns> Экземпляр WeaponDefinition или, если нет такого определения
    /// для указанного WeaponType, возвращает новый экземпляр WeaponDefinition с
    /// типом none.</returns>
    /// <param name="wt">Тип оружия WeaponType, для которого требуется получить
    /// WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        // Проверить наличие указанного ключа в словаре
        // (Попытка извлечь значение по отсутсвтующему ключу вызовет ошибку)
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // Следующая инструкция возвращает новый экземпляр WeaponDefinition с типом
        // оружия WeaponType.none, что означает неудачную попытку найти и требуемое
        // определение We WeaponDefinition
        return (new WeaponDefinition());
    }
}