using UnityEditor;
using UnityEngine;

public class ExportPackageWindow : EditorWindow
{
    int majorVersion = 1;
    int minorVersion = 0;
    int patchVersion = 0;

    [MenuItem("Export/Export StatefulUI Package With Version")]
    public static void ShowWindow()
    {
        // Открытие окна и установка начального размера
        var window = GetWindow<ExportPackageWindow>("Export Package");
        window.minSize = new Vector2(250, 120);
    }

    private void OnEnable()
    {
        // Разбор текущей версии на составляющие
        var versionParts = PlayerSettings.bundleVersion.Split('.');
        if (versionParts.Length == 3)
        {
            int.TryParse(versionParts[0], out majorVersion);
            int.TryParse(versionParts[1], out minorVersion);
            int.TryParse(versionParts[2], out patchVersion);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Enter the version number:", EditorStyles.boldLabel);
        majorVersion = EditorGUILayout.IntField("Major Version", majorVersion);
        minorVersion = EditorGUILayout.IntField("Minor Version", minorVersion);
        patchVersion = EditorGUILayout.IntField("Patch Version", patchVersion);

        if (GUILayout.Button("Export"))
        {
            var version = $"{majorVersion}.{minorVersion}.{patchVersion}";
            ExportUnityPackage.ExportPackageWithVersion(version);
            Close();
        }
    }
}