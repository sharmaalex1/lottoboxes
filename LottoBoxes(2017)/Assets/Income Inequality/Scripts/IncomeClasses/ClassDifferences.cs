using UnityEngine;
using System.Collections;
using System;


//Boxes on Screen should be proportionate to how many boxes that class has.
//number of boxes gained from a single box is proportionate
//Chances for items should be the same for both classes


// Location of scriptable objects: /Assets/IncomeInequality/Resources/ClassesData

[System.Serializable]
public class ClassDifferences : ScriptableObject
{
    [Header("What class is this data for?")]
    public Classes myClass;

    [Header("How Many Boxes spawn at a time in the Main Game?")]
    [Range(0, 10)]
    public int numSpawnBoxes;

    [Header("How many boxes spawn during this class' hyper mode")]
    [Range(0, 10)]
    public int numHyperModeBoxes;

    [Header("Chance at the start of the game for the player to receive a ticket piece")]
    [Range(0, 100)]
    public float startingTicketChance;

    [Header("Chance at the start of the game for the player to receive a box")]
    [Range(0, 100)]
    public float startingBoxChance;

    [Header("Chance at the start of the game for the player to receive nothing")]
    [Range(0, 100)]
    public float startingNothingChance;

    [Header("How many boxes does this class start with?")]
    public int startingBoxes;

    [Header("The Background image of the class")]
    public Sprite background;

    [Header("How many boxes does this class receive from a box at the beginning of the Game")]
    public int startingNumBoxesReceive;

    [Header("When receiving boxes, what percentage of total opened boxes should the play receive")]
    [Range(0, 1)]
    public float openBoxPercent;
}
