using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/* The Idea of this curve is to utilize the fibonacci sequence in order to give our curve Exponential Decay without the decay being crazy
 * (The Decay(not the actual number) is large at the beginning, but becomes smaller as the multiplier grows larger
 * The way we do this is when we increase the sequence, we calculate the new fibonacci number and then we subtract that fibonacci number's
 * reciprocal from the spawn time, as the fibonacci number grows, the reciprocal becomes smaller.
 * You can check your numbers in the tool found in Petricore Tools Tab
 */

/// <summary>
/// Class that controls the spawn rate difficulty Curve of the Mini Game
/// </summary>
[System.Serializable]
public class SpawnFibonacci : Fibonacci
{
    [Header("What is the initial Spawn time for the boxes")]
    [SerializeField]
    //what is the initial Spawn Time for the mini game 
    private float initialSpawnTime;

    //readonly property for the spawn time
    private float currentSpawnTime;
    public float CurrentSpawnTime
    {
        get
        {
          
            return currentSpawnTime;
        }
        private set
        {

        }
    }


    //initialize the sequence
    public override void Initialize()
    {
        secPrevious = 0;
        currentSpawnTime = initialSpawnTime;

        currentFibonacci = orgCurrentFib;
        previousFibonacci = orgPreviousFib;
    }

    //reset the sequence 
    public override void Reset()
    {
        Initialize();
    }

    //increment the sequence 
    public override void IncrementSequence()
    {
        //F2 = F1
        secPrevious = previousFibonacci;
    
        //F1 = Current
        previousFibonacci = currentFibonacci;

        //Current = F1 + F2
        currentFibonacci = previousFibonacci + secPrevious;
        //calculate new spawn time
        currentSpawnTime -= (1f / currentFibonacci);
    }

    /// <summary>
    /// Function to return a list of Time Values given the multiplier, the F1, F0, and an Initial Value
    /// Only to be called By FibonacciCheck.cs
    /// </summary>
    public override List<float> GetValuesWithMultiplier(int multiplier, float initialFib, float F0Fib, float initialValue)
    {
        List<float> values = new List<float>();
        values.Add(initialValue);

        //if multiplier is 1, just return list with initial value
        if(multiplier == 1)
        {
            return values;
        }

        float temp2;
        float temp1 = F0Fib;
        float current = initialFib;
        float spawn = initialValue;
        //starts at 2 because we are doing numbers 1 - highest multiplier, not 0 - highestmultiplier - 1
        //then we add 1 because the first number is what the initial speed is
        for (int i = 2; i <= multiplier; i++)
        {
            //F2 = F1
            temp2 = temp1;
            //F1 = current
            temp1 = current;
            //current = F1 + F2
            current = temp1 + temp2;

            //subtract from spawn time
            spawn -= (1 / current);
            //add new spawn time to the list
            values.Add(spawn);
        }


        return values;

    }

}
