using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/* The Idea of this curve is to use the regular Fibonacci Sequence in order to give our curve an exponential curve with out increasing too high.
 * Just utilizes the regular fibonacci number 
 */ 

/// <summary>
/// This script controls the streak in the mini game
/// The streak is need to increase the multiplier
/// </summary>
[System.Serializable]
public class StreakFibonacci: Fibonacci
{
    //readonly property for the current streak(which is represented by the current fibonacci number)
    public float WantedStreak
    {
        get
        {
            return currentFibonacci;
        }

        private set
        {

        }
    }


    /// <summary>
    /// Initialize the sequence
    /// </summary>
    public override void Initialize()
    {
        previousFibonacci = orgPreviousFib;
        currentFibonacci = orgCurrentFib;

    }

    /// <summary>
    /// Get The Current Number in the sequence
    /// </summary>

    /// <summary>
    /// Reset the Sequence
    /// </summary>
    public override void Reset()
    {
        Initialize();
    }
    /// <summary>
    /// Increment the Sequence
    /// </summary>
    public override void IncrementSequence()
    {
        secPrevious = previousFibonacci;
        previousFibonacci = currentFibonacci;
        currentFibonacci = previousFibonacci + secPrevious;
    }
    
  /// <summary>
  /// Returns a list of Streak values based off given variables(initial Value isn't needed)
  /// </summary>
    public override List<float> GetValuesWithMultiplier(int multiplier, float initialFib, float F0Fib, float initialValue)
    {
        List<float> toReturn = new List<float>();
        toReturn.Add(initialFib);

            
        if (multiplier == 1)
        {
            return toReturn;
        }

        float temp2;
        float temp1 = F0Fib;
        float current = initialFib;

        for(int i = 2; i<= multiplier; i++)
        {
            temp2 = temp1;
            temp1 = current;
            current = temp1 + temp2;
            toReturn.Add(current);
        }

        return toReturn;
    }

}
