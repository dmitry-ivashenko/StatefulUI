using UnityEditor;
using UnityEngine;

public class ExportUnityPackage
{
    public static void ExportPackageWithVersion(string version)
    {
        PlayerSettings.bundleVersion = version;

        string[] folders = { "Assets/Plugins/StatefulUI" };

        var exportDir = "Releases";
        if (!System.IO.Directory.Exists(exportDir))
        {
            System.IO.Directory.CreateDirectory(exportDir);
        }

        var packageName = $"{exportDir}/stateful_ui_{version}.unitypackage";

        AssetDatabase.ExportPackage(folders, packageName, ExportPackageOptions.Recurse);

        Debug.Log($"Exported to: {packageName}");
    }
}