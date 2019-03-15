using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tools that is used to show values for the difficulty manager
/// </summary>
public class FibonacciCheck : EditorWindow {

    //Enum for which window we want to show
    private enum WindowTypes {Ask,EnterValues,ShowValues};
    private WindowTypes type = WindowTypes.Ask;

    //Enum for what Sequence is wanted
    private enum WantedSequence {Streak,Speed,SpawnTime,None};
    private WantedSequence sequence = WantedSequence.None;

    // private SpawnFibonacci spawn;
    private StreakFibonacci streak;
    private SpeedFibonacci speed;
    private SpawnFibonacci spawn;


    //Fibonacci Numbers
    private float F0 = 0;
    private float F1 = 0;
    //initial values for Speed and Spawn Time fibonacci Sequences
    private float initialSpeed = 0;
    private float initialSpawnTime = 0;
    //what is the highest multiplier that you want to show values for 
    private int highestMultiplier = 0;

    private List<float> values;

    //Function that allows us to open the initial Window
    [MenuItem("Petricore Tools/Check Fibonacci")]
    public static void ShowFibonnaciWindow()
    {
        FibonacciCheck window = (FibonacciCheck)EditorWindow.GetWindow(typeof(FibonacciCheck));
        window.Show();
    }


    //Acts like Update but for the editor window, this is how we show everything
    void OnGUI()
    {
        switch (type)
        {
            case WindowTypes.Ask:
                {
                    ShowAskWindow();
                    break;
                }
            case WindowTypes.EnterValues:
                {
                    ShowEnterValuesWindow();
                    ResetValues();
                    break;
                }
            case WindowTypes.ShowValues:
                {
                    ShowFinishedValues();
                    break;
                }
            default:
                {
                    break;
                }
        }

        BackButton();

    }

    #region Which Sequence? Window

    //this function handles asking the programmer what sequence he wants to check
    private void ShowAskWindow()
    {
        //make our textStyle
        GUIStyle textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 13;
        //title of page
        EditorGUILayout.LabelField("Please Select Which Sequence You would like to Check", textStyle);

        //change font size for smaller Label Fields
        textStyle.fontSize = 11;

        //Button For Streak with Label Field explaining the fibonacci sequence used
        EditorGUILayout.LabelField("The streak needed for each Multiplier", textStyle);
        if (GUILayout.Button("Streak"))
        {
            //if the sequence changes, reset the values 
            if (sequence != WantedSequence.Streak)
            {
                ResetValues(true);
                sequence = WantedSequence.Streak;
            }
            type = WindowTypes.EnterValues;
        }

        //Button For Streak with Label Field explaining the fibonacci sequence used
        EditorGUILayout.LabelField("The Speed of the boxes at each multiplier.", textStyle);
        if (GUILayout.Button("Speed"))
        {
            if(sequence != WantedSequence.Speed)
            {
                ResetValues(true);
                sequence = WantedSequence.Speed;
            }
            type = WindowTypes.EnterValues;
        }

        //Button For Streak with Label Field explaining the fibonacci sequence used
        EditorGUILayout.LabelField("How fast the boxes spawn at each multiplier", textStyle);
        if (GUILayout.Button("SpawnTime"))
        {
            if (sequence != WantedSequence.SpawnTime)
            {
                ResetValues(true);
                sequence = WantedSequence.SpawnTime;
            }
            type = WindowTypes.EnterValues;
        }

    }

    #endregion

    #region Enter Values Windows
    
    //Function that takes care of what Window to show to the programmer when they want to enter values
    private void ShowEnterValuesWindow()
    {
        //text style setup
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 13;

        //title
        EditorGUILayout.LabelField("Please Enter the Values you would like to Check", style);

        //Field for multiplier
        highestMultiplier = EditorGUILayout.IntField("HighestMultiplier: ", highestMultiplier);

        //show error if highest multiplier isn't entered
        if (highestMultiplier <= 0)
        {
            EditorGUILayout.HelpBox("Highest Multiplier must be greater than zero to continue", MessageType.Error);
        }

        //only show the rest of the values when they enter a multiplier
        else
        {
            switch (sequence)
            {
                case WantedSequence.Streak:
                    {
                        EnterStreakValues();
                        break;
                    }

                case WantedSequence.Speed:
                    {
                        EnterSpeedValues();
                        break;
                    }

                case WantedSequence.SpawnTime:
                    {
                        EnterSpawnValues();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
   
    //function that takes car of asking for Streak Fibonacci Values
    private void EnterStreakValues()
    {
        //set up text style
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 13;

        EditorGUILayout.LabelField("The Equation is CurrentMultiplierStreak = PreviousMultiplierStreak + SecondPreviousMultiplierStreak",style);
        EditorGUILayout.LabelField("For Multiplier 1, the number used is F1",style);
        EditorGUILayout.LabelField("Multipler 2 begins the cycle with its number being F1 + F0.",style);
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = 11;

        //Ask for initial Value with explanation of value wanted
        EditorGUILayout.LabelField("F1 will represent the initial Streak that is wanted",style);
        F1 = EditorGUILayout.FloatField("F1:", F1);

        //ask for Value if multiplier 0 existed with explanation for the value
        EditorGUILayout.LabelField("F0 is used for calculations. Imagine this being the streak for multiplier 0",style);
        F0 = EditorGUILayout.FloatField("F0:", F0);

        //show error if initial value is less than zero
       if(F1 <= 0)
        {
            EditorGUILayout.HelpBox("F1 can't be less than or equal to zero!", MessageType.Error);
        }

       //show error if F0 value is less than zero
       if(F0 <= 0)
        {
            EditorGUILayout.HelpBox("F0 can't be less than or equal to zero", MessageType.Error);
        }

       //show warning if initial value is less than F0 value
        if (F1 < F0)
        {
            EditorGUILayout.HelpBox("If F1 is greater than F0, it will yield better results", MessageType.Warning);
        }

        //only show button to Calculate values if both initial value and F0 are greater than zero
        if (F1 > 0 && F0 > 0)
        {
            if(GUILayout.Button("Show Values"))
            {
                CalculateStreakValues();
                type = WindowTypes.ShowValues;
            }
        }

    }

    //Function that takes care of entering values for Speed Fibonacci Sequence
    private void EnterSpeedValues()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 13;

        EditorGUILayout.LabelField("The Equation is CurrentSpeed = initialSpeed + F(x)", style);
        EditorGUILayout.LabelField("Where F(x) = 1/Fh + 1/F(h-1)+ .... + 1/F(h-x), where x is current multiplier & H is the highest multiplier", style);
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = 11;

        //ask for initial speed with explanation
        EditorGUILayout.LabelField("What is the initial Speed of the Box", style);
        initialSpeed = EditorGUILayout.FloatField("Initial Speed:", initialSpeed);

        //ask for first Reciprocal value with explanation
        EditorGUILayout.LabelField("What is the first Reciprocal value, used to calculate F2", style);
        F1 = EditorGUILayout.FloatField("F1:", F1);

        //ask for second Reciprocal Value with explanation
        EditorGUILayout.LabelField("What is the zero Reciprocal value, used to calculate F2", style);
        F0 = EditorGUILayout.FloatField("F0:", F0);


        //Show error if initial speed is less than zero
        if(initialSpeed <= 0)
        {
            EditorGUILayout.HelpBox("Initial Speed can't be less than or equal to zero!", MessageType.Error);
        }

        //show error if First Reciprocal is less than zero
        if (F1 <= 0)
        {
            EditorGUILayout.HelpBox("F1 can't be less than or equal to zero!", MessageType.Error);
        }

        //show error if zero Reciprocal is less than zero
        if (F0 <= 0)
        {
            EditorGUILayout.HelpBox("F0 can't be less than or equal to zero", MessageType.Error);
        }

        //show warning if first reciprocal value is less than zero reciprocal value
        if(F1 < F0)
        {
            EditorGUILayout.HelpBox("If F1 is greater than F0, it will yield better results", MessageType.Warning);
        }

        //only show button to show values if both reciprocals and speed are greater than zero
        if (F1 > 0 && F0 > 0 && initialSpeed > 0)
        {
            if (GUILayout.Button("Show Values"))
            {
                CalculateSpeedValues();
                type = WindowTypes.ShowValues;
            }
        }
    }

    private void EnterSpawnValues()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 13;

        EditorGUILayout.LabelField("The Equation is CurrentSpawnRate = initialTime - F(x)", style);
        EditorGUILayout.LabelField("Where F(x) = 1/F2 + 1/F3 + .... + 1/Fx, where x is the current multiplier", style);
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = 11;


        //ask for initial spawn rate with explanation
        EditorGUILayout.LabelField("What is the initial spawn rate of the Box", style);
        initialSpawnTime = EditorGUILayout.FloatField("Initial Spawn Time:", initialSpawnTime);

        //Ask for initial Fib value with explanation
        EditorGUILayout.LabelField("What is the first Fibonnaci value, used to calculate other Fibonacci Values", style);
        F1 = EditorGUILayout.FloatField("F1:", F1);

        //Ask for zero fib value with explanation
        EditorGUILayout.LabelField("What is the second Fibonacci value, used to calculate other Fibonacci Values", style);
        F0 = EditorGUILayout.FloatField("F0:", F0);

        //show error if spawn time is less than zero
        if (initialSpawnTime <= 0)
        {
            EditorGUILayout.HelpBox("Initial Spawn Time can't be less than or equal to zero!", MessageType.Error);
        }

        //show error if initial fib value is less than zero
        if (F1 <= 0)
        {
            EditorGUILayout.HelpBox("F1 can't be less than or equal to zero!", MessageType.Error);
        }

        //who error if zero fib value is less than zero 
        if (F0 <= 0)
        {
            EditorGUILayout.HelpBox("F0 can't be less than or equal to zero", MessageType.Error);
        }

        //Show warning if initial Fib value is less than zero fib value
        if (F1 < F0)
        {
            EditorGUILayout.HelpBox("If F1 is greater than F0, it will yield better results", MessageType.Warning);
        }

        //only who button to Calculate values if all wanted values are greater than zero
        if (F1 > 0 && F0 > 0 && initialSpawnTime > 0)
        {
            if (GUILayout.Button("Show Values"))
            {
                CalculateSpawnTimeValues();
                type = WindowTypes.ShowValues;
            }
        }
    }

    #endregion

    #region Show Values Windows
    //Function To Show the finished values of the calculations
    private void ShowFinishedValues()
    {
        GUIStyle textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 13;
        EditorGUILayout.LabelField("Here are the results!", textStyle);

        for(int i = 0; i < highestMultiplier; i++)
        {
            EditorGUILayout.LabelField(sequence.ToString()+ " at Multiplier " + (i + 1).ToString() + " is " + values[i].ToString());
        }
        
    }
    #endregion

    #region Calculate Values

    //Function to calculate the values for the streak at each multiplier
    private void CalculateStreakValues()
    {
        values = new List<float>();
        streak = new StreakFibonacci();
        values = streak.GetValuesWithMultiplier(highestMultiplier, F1, F0,0);
    }

    //function to calculate the values of the speed at each multiplier
    private void CalculateSpeedValues()
    {
        values = new List<float>();
        speed = new SpeedFibonacci();
        values = speed.GetValuesWithMultiplier(highestMultiplier, F1, F0, initialSpeed);
        if(values == null)
        {
            Debug.Log("Null");
        }
    }

    //Function to calculate values of the spawn time at each mutliplier
    private void CalculateSpawnTimeValues()
    {
        values = new List<float>();
        spawn = new SpawnFibonacci();
        values = spawn.GetValuesWithMultiplier(highestMultiplier, F1, F0, initialSpawnTime);
    }

    #endregion

    #region Utility
    //Crappy back button 
    private void BackButton()
    {
        if (type != WindowTypes.Ask && GUILayout.Button("Go Back"))
        {
            if(type == WindowTypes.ShowValues)
            {
                type = WindowTypes.EnterValues;
            }
            else if(type == WindowTypes.EnterValues)
            {   
                type = WindowTypes.Ask;
            }
        }
    }

    //function used to reset values
    //has a optional Bool that forces the restet
    //Otherwise show a button to ask theme
    private void ResetValues(bool automaticReset = false)
    {

        if(automaticReset)
        {
            F1 = 0;
            F0 = 0;
            highestMultiplier = 0;
            initialSpawnTime = 0;
            initialSpeed = 0;
            values = new List<float>();
        }

        else if (GUILayout.Button("Reset Values"))
        {
            F1 = 0;
            F0 = 0;
            highestMultiplier = 0;
            initialSpawnTime = 0;
            initialSpeed = 0;
            values = new List<float>();
        }
    }

    #endregion


}
