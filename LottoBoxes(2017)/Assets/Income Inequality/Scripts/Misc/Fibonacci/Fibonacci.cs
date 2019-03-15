using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Fibonacci
{
    [Header("Represents F1 and the Initial number if Streak Fibonacci")]
    [SerializeField]
    protected float orgCurrentFib;


    [Header("represents F0")]
    [SerializeField]
    protected float orgPreviousFib;


    //current Relevant Fibonacci Numbers
    protected float secPrevious;
    protected float previousFibonacci;
    protected float currentFibonacci;

    public abstract void Initialize();

    /// <summary>
    /// Reset the Sequence
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// Moves the Sequence Forward
    /// </summary>
    public abstract void IncrementSequence();

    public abstract List<float> GetValuesWithMultiplier(int multiplier, float initialFib, float F0Fib, float initialValue );
}
