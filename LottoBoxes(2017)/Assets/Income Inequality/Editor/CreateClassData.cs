using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateClassData : EditorWindow
{
    private ClassDifferences classToCreate;
    private string assetName;
    private string assetPath;
    private bool created = false;

    private static CreateClassData instance;

    [MenuItem("Petricore Tools/Create Class Data")]
	public static void ShowCreateData()
    {
        instance = (CreateClassData)EditorWindow.GetWindow(typeof(CreateClassData));
        instance.Show();
    }


    private void OnGUI()
    {
        if(!created)
        {
            CreateScriptableObj();
            created = true;
        }

        //make our textStyle
        GUIStyle textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 13;

        EditorGUILayout.LabelField("Make that new Class Data my Son",textStyle);

        assetName = EditorGUILayout.TextField("Name: ", assetName);

        if(assetName == null || assetName.Length == 0)
        {
            EditorGUILayout.HelpBox("Must name the class object before continuing", MessageType.Error);
        }

        else
        {
            AskVariables();
            if(GUILayout.Button("Create"))
            {
                CreateData();
            }
        }
    }

    private void CreateScriptableObj()
    {
        classToCreate = ScriptableObject.CreateInstance<ClassDifferences>();
    }

    private void CreateData()
    {
        //make assetPath
        assetPath = "Assets/Income Inequality/Resources/ClassesData/" + assetName + ".asset";
        //Make sure the assetPath is Unique so we don't over write anything
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        //create the asset
        AssetDatabase.CreateAsset(classToCreate, assetPath);
        //refresh so the asset can be viewd
        AssetDatabase.Refresh();
        //focus the project window
        EditorUtility.FocusProjectWindow();
        //show this object in the project window
        Selection.activeObject = classToCreate;

        instance.Close();
       
    }

    private void AskVariables()
    {

        GUIStyle textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleLeft;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 11;

        EditorGUILayout.HelpBox("You will need to set the Background Sprite after Creation", MessageType.Warning);

        EditorGUILayout.LabelField("What class is this data for?",textStyle);
        classToCreate.myClass = (Classes)EditorGUILayout.EnumPopup("Class Type: ",classToCreate.myClass);

        EditorGUILayout.LabelField("How Many Boxes spawn at a time in the Main Game?", textStyle);
        classToCreate.numSpawnBoxes = EditorGUILayout.IntSlider("Boxes Spawn At a Time: ",classToCreate.numSpawnBoxes,0,10);

        EditorGUILayout.LabelField("How many boxes spawn during hyper mode for this class", textStyle);
        classToCreate.numHyperModeBoxes = EditorGUILayout.IntSlider("Boxes Spawn at a time during Hyper Mode,",classToCreate.numHyperModeBoxes,0,10);

        EditorGUILayout.LabelField("Chance at the start of the game for this class to receive a ticket piece", textStyle);
        classToCreate.startingTicketChance = EditorGUILayout.Slider("Starting Ticket Chance", classToCreate.startingTicketChance,0,100);

        EditorGUILayout.LabelField("Chance at the start of the game for this class to receive the box",textStyle);
        classToCreate.startingBoxChance = EditorGUILayout.Slider("Starting Box Chance", classToCreate.startingBoxChance,0,100);

        EditorGUILayout.LabelField("Chance at the start of the game for this class to receive nothing", textStyle);
        classToCreate.startingNothingChance = EditorGUILayout.Slider("Starting Nothing Chance", classToCreate.startingNothingChance,0,100);

        EditorGUILayout.LabelField("How many boxes does this class start with",textStyle);
        classToCreate.startingBoxes = EditorGUILayout.IntField("Starting Boxes:", classToCreate.startingBoxes);

        EditorGUILayout.LabelField("How many boxes does this class receive from a box at the beginning of the Game", textStyle);
        classToCreate.startingNumBoxesReceive = EditorGUILayout.IntField("Starting number of Boxes To Receive:", classToCreate.startingNumBoxesReceive);

        EditorGUILayout.LabelField("When receiving boxes, what percentage of total opened boxes should the play receive", textStyle);
        classToCreate.openBoxPercent = EditorGUILayout.Slider("Open Box Percentage:", classToCreate.openBoxPercent, 0, 1);

    }

}
