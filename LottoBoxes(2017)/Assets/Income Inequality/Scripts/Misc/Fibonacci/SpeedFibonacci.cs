using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* The way this difficulty curve is managed is by using the inverse order of the reciprocal fibonacci sequence
 * This will give us a small exponential growth curve that won't get out of hand 
 * How this works is when the sequence is initialized we make a Stack of Values 
 * that has pre calculated fibonacci values based off the highest mutliplier 
 * the highest Fibonacci number being at the top
 * When we need to increment the sequence we pop the new fibonacci number off of the stack 
 * We then take the reciprocal of that fibonacci number and add it to the speed
 * As the sequence continues to grow, the fibonacci number used will get smaller and smaller
 * BUT as the fibonacci number becomes smaller, the reciprocal grows (ex. 1/50(.02) < 1/5(.2))
 * Thus causing the increases of speed between multipliers to go up
 * This will give us an exponential Curve
 */

/// <summary>
/// This class controls the speed difficulty curve for the mini Game
/// </summary>
[System.Serializable]
public class SpeedFibonacci : Fibonacci
{
    [Header("What is the starting speed of the boxes")]
    [SerializeField]
    private float initialSpeed;
    
    private float currentSpeed;

    private Stack<float> fibonacciValues;

    //Initialize the sequence
    public override void Initialize()
    {
        previousFibonacci = orgPreviousFib;
        currentFibonacci = orgCurrentFib;

        currentSpeed = initialSpeed;
        InitializeStack();
    }

    //readonly property for the curent speed
    public float CurrentSpeed
    {
        get
        {
            return currentSpeed;
        }

        private set
        {

        }
    }

    //reset the sequence 
    public override void Reset()
    {
       Initialize();
    }

    //Increment the sequence 
    public override void IncrementSequence()
    {
        currentFibonacci = fibonacciValues.Pop();

        //add reciprocal to the speed
        currentSpeed += (1f / currentFibonacci);
     
    }

    /// <summary>
    /// Function that returns a list of the speed values given the correct variables
    /// Only To ve called be FibonacciCheck.cs
    /// </summary>
    public override List<float> GetValuesWithMultiplier(int multiplier, float initialFib, float F0Fib, float initialValue)
    {
        //don't use class variables 
        List<float> values = new List<float>();
        values.Add(initialValue);

        if (multiplier == 1)
        {
            return values;
        }

        //calculates fibnoacci values below
        Stack<float> fibValues = new Stack<float>();
        float temp2;
        float temp1 = F0Fib;
        float current = initialFib;

        //starts at 2 because we are doing numbers 1 - highest multiplier, not 0 - (highestmultiplier - 1)
        //then we add 1 because we dont need a fibonacci value for multiplier 1
        for (int i = 2; i <= multiplier; i++)
        {
            temp2 = temp1;
            temp1 = current;
            current = temp1 + temp2;

            fibValues.Push(current);
        }

        float speed = initialValue;

        for (int i = 2; i <= multiplier; i++)
        {
            float currentFib = fibValues.Pop();
            speed += (1 / currentFib);
            values.Add(speed);

        }


        return values;
    }

    //function used to initialize the stack 
    private void InitializeStack()
    {
        //grab the highest multiplier
        int highestMultiplier = MiniGameDifficultyManager.Instance.HighestMultiplier;

        fibonacciValues = new Stack<float>();

        //set the fibonacci numbers to prepare for the numbers
        previousFibonacci = orgPreviousFib;
        currentFibonacci = orgCurrentFib;

        //starts at 2 because we are doing numbers 1 - highest multiplier, not 0 - highestmultiplier - 1
        //then we add 1 because we don't need a fibonacci value for multiplier 1
        for (int i = 2; i <= highestMultiplier; i++)
        {
            //set F2 to F1
            secPrevious = previousFibonacci;
            //set F1 to the current 
            previousFibonacci = currentFibonacci;
            //Calculate new current with F1 + F2
            currentFibonacci = previousFibonacci + secPrevious;
            //push it to the stack
            fibonacciValues.Push(currentFibonacci);
        }
    }
}
