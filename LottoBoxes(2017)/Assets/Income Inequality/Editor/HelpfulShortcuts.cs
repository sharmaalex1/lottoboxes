using UnityEngine;
using UnityEditor;
using System.IO;

static class HelpfulShortcuts
{
    [MenuItem("Tools/Clear Console %#x")] // CTRL + SHIFT + X
    static void ClearConsole()
    {
        //Allows for clearing of the console window if it is the current window being focused on.
        if (EditorWindow.focusedWindow.GetType().ToString() == "UnityEditor.ConsoleWindow")
        {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }

    [MenuItem("Tools/Clear PlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// A nuance to using this is that the user mut clear player prefs when they are all set
    /// taking screenshots for a particular session. The number after the screenshot name
    /// will continue to increase forever otherwise.
    /// </summary>
    [MenuItem("Tools/Take Screenshot %#V")]
    static void TakeScreenshot()
    {
        string filename = "Screenshots/Capture";
        if (!PlayerPrefs.HasKey("SSCount"))
        {
            PlayerPrefs.SetInt("SSCount", 1);
        }
        filename += PlayerPrefs.GetInt("SSCount") + ".png";
        PlayerPrefs.SetInt("SSCount", PlayerPrefs.GetInt("SSCount") + 1);
        Application.CaptureScreenshot(filename);

        Debug.Log("A screenshot has been captured");
    }

    [MenuItem("Tools/UI/Align Anchors to Corners %#o")]
    static void AlignAnchorsToCorners()
    {
        foreach (Transform transform in Selection.transforms)
        {
            RectTransform t = transform as RectTransform;
            RectTransform pt = Selection.activeTransform.parent as RectTransform;

            if (t == null || pt == null)
                return;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                        t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                        t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }
    }

    //	[MenuItem("Tools/UI/Align Anchors to Corners %#i")]
    //	static void CenterAnchors()
    //	{
    //		foreach (Transform transform in Selection.transforms)
    //		{
    //			RectTransform t = transform as RectTransform;
    //
    //			if (t == null)
    //				return;
    //
    //
    //			t.anchorMax = new Vector2(0.5f, 0.5f);
    //			t.anchorMin = new Vector2(0.5f, 0.5f);
    //
    //
    //		}
    //	}
}
