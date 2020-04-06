using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    // Инициализация игровых объектов
    public GameObject wall;
    public GameObject field;
    public GameObject pit;
    public GameObject demon;
    public GameObject soul;
    public GameObject killer;

    // Размер считываемой карты (в инициализации MapSizeX и mapSizeY вводить вручную)
    int mapSizeX = 22;
    int mapSizeY = 22;
    int[,] map;
    public List<int> coord;
    int soul_count = 10;
    //List<GameObject> souls;

    void Start()
    {
        map = new int[mapSizeX, mapSizeY];
        // Считывание карты с текстового файла
        using (StreamReader sr = new StreamReader("Maps.txt", System.Text.Encoding.Default))
        {
            for (int i = 0; i < mapSizeX; i++)
            {
                var line = sr.ReadLine().Split(' ');
                for (int j = 0; j < line.Length; j++)
                    map[i, j] = int.Parse(line[j]);
            }
        }

        // Генерация карты
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                if (map[i, j] == 1)
                {
                    Instantiate(wall, new Vector3(j, mapSizeX - i, 1), Quaternion.identity);
                }                    
                else if (map[i, j] == 0)
                {
                    Instantiate(field, new Vector3(j, mapSizeX - i, 1), Quaternion.identity);
                    int u = i * 12 + j;
                    coord.Add(u);
                }
                else if (map[i, j] == 2)
                {
                    Instantiate(killer, new Vector3(j, mapSizeX - i, 1), Quaternion.identity);
                }
                else if (map[i, j] == 3)
                {
                    Instantiate(pit, new Vector3(j, mapSizeX - i, 1), Quaternion.identity);
                }
            }
                
        }


        
        // Поиск первой свободной ячейки для генерации Демона
        bool flag = false;
        for (int i = 0; i < mapSizeX - 1; i++)
        {
            for (int j = 0; j < mapSizeY - 1; j++)
                if (map[i, j] == 0 && map[i - 1, j] == 0)
                {
                    demon.transform.position = new Vector3(j, mapSizeX - i, -1);
                    flag = true;
                    break;
                }

            if (flag) break;
        }

        for (int i = 0; i < soul_count; i++)
        {
            int spawnX = UnityEngine.Random.Range(1, mapSizeX), spawnY = UnityEngine.Random.Range(1, mapSizeY);
            while (map[spawnX, spawnY] != 0)
            {
                spawnX = UnityEngine.Random.Range(1, mapSizeX);
                spawnY = UnityEngine.Random.Range(1, mapSizeY);
            }
            Instantiate(soul, new Vector3(spawnY, mapSizeX - spawnX, 0), Quaternion.identity);
        }

        /*
        int d_p = coord[new System.Random().Next(0, coord.Count)];
        coord.Remove(d_p);
        int y = (d_p + 1) % mapSizeY;
        int x = (d_p - y) / mapSizeX;
        demon.transform.position = new Vector3(y,x, 0);

        for (int i = 0; i < soul_count; i++)
        {
            d_p = coord[new System.Random().Next(coord.Count)];
            coord.Remove(d_p);
            y = d_p % mapSizeY;
            x = (d_p - y) / mapSizeX;
            Instantiate(soul, new Vector3(y, x, 0), Quaternion.identity);
        }
        */

    }
}