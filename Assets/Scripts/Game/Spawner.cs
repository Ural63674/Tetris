using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] shapes;
    
    [SerializeField]
    private int[] values =
    {
        20,
        15,
        15,
        15,
        15,
        10,
        10
    };

    void Start()
    {
        SpawnNextShape();
    }


    public void SpawnNextShape()
    {
        int total = 0;

        foreach (int item in values)
        {
            total += item;
        }
        
        int randomNumber = Random.Range(0, total);

        /// <summary>
        /// Создаем фигуру из массива shapes согласно вероятностям выпадения в массиве values
        /// </summary>
        for (int i = 0; i < values.Length; i++)
        {
            if(randomNumber <= values[i])
            {
                GameObject newShape = shapes[i];
                Instantiate(newShape, transform.position, Quaternion.identity);
                return;
            }
            else
            {
                randomNumber -= values[i];
            }
        }       
    }
}
