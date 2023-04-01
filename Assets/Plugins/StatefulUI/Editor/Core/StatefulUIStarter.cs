using StatefulUI.Runtime.Core;
using UnityEditor;

namespace StatefulUI.Editor.Core
{
    public static class StatefulUIStarter
    {
        [InitializeOnLoadMethod]
        public static void Init()
        {
            if (!StatefulUiUtils.IsImplementationExists<IStatefulView>())
            {
                RoleGenerator.GenerateAll();
                AssetDatabase.Refresh();
            }
        }
    }
}
