using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Runner : MonoBehaviour
{
    public int num_penguins;

    void Start()
    {
        GameObject g = GameObject.Find("PenguinFBX"); //the prefab for the 'rudy' chicken
        for (int i = 0; i < num_penguins; i++)
        {
            GameObject c = GameObject.Instantiate(g); //instatiates rudy
            c.name = "Penguin_" + i; //names the penguin object based on its number
            
            /*
             * places all of the penguins in random start locations in the game area, ensures they
             * won't spawn in a location outside of view
             */ 
            float y_val = Random.Range(1.0f, 4.0f);
            float z_val;
            float x_val;

            if (i >= (num_penguins / 2))
            {
                z_val = -3.0f;
                x_val = Random.Range(-8.0f, 2.5f);
            }
            else
            {
                z_val = -5.0f;
                x_val = Random.Range(-8.0f, 5.0f);
            }

            c.GetComponent<Rigidbody>().transform.position = new Vector3(x_val, y_val, z_val);
        }
    }

    /*
     * Decrements the count of the number of chickens 
     */ 
    public void Decrement_Num_Penguins()
    {
        num_penguins--;
    }

    /*
     * Returns the current count of the number of chickens
     * 
     * @return current chicken count
     */ 
    public int Get_Num_Penguins()
    {
        return num_penguins;
    }
}
