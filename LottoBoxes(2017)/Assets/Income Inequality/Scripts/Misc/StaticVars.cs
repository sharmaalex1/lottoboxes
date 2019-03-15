using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StaticVars
{

    //const variables are inherently static
    public const int MAX_BOX_COMPONENTS = 1;

    //These threee variables are the probability of obtaining each item.
    //They must add up to 10000.
    public const int BOX_PERCENT_CHANCE = 1200;
    public const int GOLDTICKET_PERCENT_CHANCE = 300;
    public const int NOTHING_PERCENT_CHANCE = 8500;

    // Seperate chances for opportunity box, follows same rules
    // Must add up to 10000
    public const int OPPBOX_BOX_PERCENT_CHANCE = 7500;
    public const int OPPBOX_GOLDTICKET_PERCENT_CHANCE = 2000;
    public const int OPPBOX_NOTHING_PERCENT_CHANCE = 500;

    // Seperate chances for opportunity box, follows same rules
    // Poor/Rich opportunity boxes have seperate drop rates
    // Must add up to 10000
    public const int POOR_OPPBOX_BOX_PERCENT_CHANCE = 6000;
    public const int POOR_OPPBOX_GOLDTICKET_PERCENT_CHANCE = 1000;
    public const int POOR_OPPBOX_NOTHING_PERCENT_CHANCE = 3000;


    #region Main Game Boxes Gained Text Variables

    public const float TUTORIAL_TEXT_SCALE = 0.1f;

    //scale of the text
    public const float TEXT_SCALE = 0.3f;

    //how long it takes the the text to move
    public const float TIME_MOVE_NUMBER = 0.4f;

    //time to allow the player to see the number
    public const float TIME_SHOW_ITEM = 0.5f;

    #endregion


    //enums are inherently static
    public enum BoxContent
    {
        Nothing = 0,
        Box = 1,
        GoldTicket = 2
    }

    public enum Race
    {
        none = -1,
        black = 0,
        white = 1,
    }

    private static List<BoxContent> chanceList;

    public static void GenerateProbabilityList()
    {
        chanceList = new List<BoxContent>();
        for (int i = 0; i < GOLDTICKET_PERCENT_CHANCE; i++)
        {
            chanceList.Add(BoxContent.GoldTicket);
        }

        for (int i = 0; i < BOX_PERCENT_CHANCE; i++)
        {
            chanceList.Add(BoxContent.Box);
        }

        for (int i = 0; i < NOTHING_PERCENT_CHANCE; i++)
        {
            chanceList.Add(BoxContent.Nothing);
        }
    }

    public static BoxContent GetContents(int num)
    {
        //indices are 0 through 999
        if (num < 0 || num >= 10000)
        {
            #if UNITY_EDITOR
            Debug.LogError("Tried Getting a component that doesn't exist in limits of the List, returning nothing");
            #endif

            return BoxContent.Nothing;
        }

        return chanceList[num];
    }

    private static List<BoxContent> oppBoxChanceList;

    public static void OppBoxGenerateProbabilityList()
    {
        oppBoxChanceList = new List<BoxContent>();

        for (int i = 0; i < OPPBOX_GOLDTICKET_PERCENT_CHANCE; i++)
        {
            oppBoxChanceList.Add(BoxContent.GoldTicket);
        }

        for (int i = 0; i < OPPBOX_NOTHING_PERCENT_CHANCE; i++)
        {
            oppBoxChanceList.Add(BoxContent.Nothing);
        }

        for (int i = 0; i < OPPBOX_BOX_PERCENT_CHANCE; i++)
        {
            oppBoxChanceList.Add(BoxContent.Box);
        }
    }

    public static BoxContent OppBoxGetContents(int num)
    {
        if (num < 0 || num >= 10000)
        {
            #if UNITY_EDITOR
            Debug.LogError("Tried Getting a component that doesn't exist in limits of the List, returning nothing");
            #endif

            return BoxContent.Nothing;
        }

        return oppBoxChanceList[num];
    }

    private static List<BoxContent> poorOppBoxChanceList;

    public static void PoorOppBoxGenerateProbabilityList()
    {
        poorOppBoxChanceList = new List<BoxContent>();

        for (int i = 0; i < POOR_OPPBOX_GOLDTICKET_PERCENT_CHANCE; i++)
        {
            poorOppBoxChanceList.Add(BoxContent.GoldTicket);
        }

        for (int i = 0; i < POOR_OPPBOX_NOTHING_PERCENT_CHANCE; i++)
        {
            poorOppBoxChanceList.Add(BoxContent.Nothing);
        }

        for (int i = 0; i < POOR_OPPBOX_BOX_PERCENT_CHANCE; i++)
        {
            poorOppBoxChanceList.Add(BoxContent.Box);
        }
    }

    public static BoxContent PoorOppBoxGetContents(int num)
    {
        if (num < 0 || num >= 10000)
        {
            #if UNITY_EDITOR
            Debug.LogError("Tried Getting a component that doesn't exist in limits of the List, returning nothing");
            #endif

            return BoxContent.Nothing;
        }

        return poorOppBoxChanceList[num];
    }


    //Data path for saving
    public static readonly string PATH_SAVE_DATA = Application.persistentDataPath + "/data.dat";

    public const int CLICKED_LAYER = 8;

    private static float cameraHalfWidth = float.MinValue;

    public static float CameraHalfWidth
    {
        get
        {
            if (cameraHalfWidth == float.MinValue)
            {
                cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
            }

            return cameraHalfWidth;
        }
    }

    public static float GetBoxHalfDiagonal(Vector2 size, Vector3 scale)
    {
        float halfX = (size.x * scale.x) / 2f;
        float halfY = (size.y * scale.y) / 2f;

        return Mathf.Sqrt((halfX * halfX) + (halfY * halfY));
    }

 
}

/// <summary>
/// List of Scenes in the game, integer associated with scene should reflect the number in the build settings
/// </summary>
public enum Scenes
{
    Splash = 0,
    StartScene = 1,
    ClassRoulette = 2,
    MainMiniCombo = 3,
    ClassSelection = 4,
};

/// <summary>
/// All classes/campaigns in the game
/// </summary>
public enum Classes
{
    Rich = 0,
    Poor = 1}
;



/// <summary>
/// The ticket Pieces that can be found in the game. 
/// </summary>
public enum TicketPieceLocations
{
    TopLeft = 0,
    BottomLeft = 1,
    Middle = 2,
    TopRight = 3,
    BottomRight = 4}
;
//^ The name of the elements in the enum should match the elements that make up the completed ticket
//Ex. right now the ticket when complete has 5 pieces, if this should change, the enum should change to match the new ticket
//and it's pieces

