using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Collections.Generic;

//Very slightly modified from dannyd's CloudBuildHelper script on the Unity forums
public class pListHelper : MonoBehaviour
{
	#if UNITY_IOS
	[PostProcessBuild]
	static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
	// Read plist
	var plistPath = Path.Combine(path, "Info.plist");
	var plist = new PlistDocument();
	plist.ReadFromFile(plistPath);

	// Update value
	PlistElementDict rootDict = plist.root;
	rootDict.SetString("NSCameraUsageDescription", "Lotto' Boxes may need to access the camera.");

	// Write plist
	File.WriteAllText(plistPath, plist.WriteToString());
	}
	#endif
}