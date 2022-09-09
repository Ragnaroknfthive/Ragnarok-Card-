//from http://answers.unity3d.com/questions/45186/can-i-auto-run-a-script-when-editor-launches-or-a.html
//
// This works great ... save this as Autorun.cs in your Editor folder. The InitializeOnLoad attribute is 
// the special sauce that makes it work. (I've deprecated my previous answer with the custom editor for 
// Transform, this is a much better approach.)
//

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
//using System.IO;
//using Common.IO;
        
[InitializeOnLoad]
public class VersionIncrementor
{
    static VersionIncrementor()
    {
        //If you want the scene to be fully loaded before your startup operation, 
        // for example to be able to use Object.FindObjectsOfType, you can defer your 
        // logic until the first editor update, like this:
        EditorApplication.update += RunOnce;
    }

    static void RunOnce()
    {
        EditorApplication.update -= RunOnce;
        ReadVersionAndIncrementBundle();
    }

    /// <summary>
    /// Read version and Increment for ProjectSettings
    /// Code is not validated and version structure is as follows: Major, Minor, SubMinor, SubVersionCodename (0.0.000.codename)
    /// </summary>
    static void ReadVersionAndIncrementBundle()
    {

        string versionText = PlayerSettings.bundleVersion;
        if (versionText != null)
        {
            versionText = versionText.Trim(); //clean up whitespace if necessary
            string[] lines = versionText.Split('.');
           
            int MajorVersion = int.Parse(lines[0]);
            int MinorVersion = int.Parse(lines[1]);
            //int SubMinorVersion = int.Parse(lines[2]) + 1; //increment here
           // string SubVersionText = lines[3].Trim();

            //Debug.Log("Major, Minor, SubMinor, SubVerLetter: " + MajorVersion + " " + MinorVersion + " " + SubMinorVersion + " " + SubVersionText);

            versionText = MajorVersion.ToString("0") + "." +
                          MinorVersion.ToString("0");// + "." +
                        //  SubMinorVersion.ToString("000") + "." +
                        //  SubVersionText;

            

            PlayerSettings.bundleVersion = versionText;
            Debug.Log("Version Incremented " + PlayerSettings.bundleVersion);

        }
    }

}
#endif