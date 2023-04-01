using System;
using UnityEngine;
using UnityEngine.U2D;

namespace StatefulUI.Runtime.Core
{
    public static class ResourceManagerExt
    {
        private const double SINGLE_LOAD_TIMEOUT_WARNING = 0.5f;

        public static T LoadResource<T>(this string path, bool nullable = true) where T : UnityEngine.Object
        {
            var startTime = DateTime.Now;

            try
            {
                var result = Resources.Load<T>(path);
                if (result == null && !nullable) Debug.LogError($"Can't load resource: {path}");
                ShowLoadingWarning(path, startTime, SINGLE_LOAD_TIMEOUT_WARNING);
                return result;
            }
            catch
            {
                Debug.LogError($"FAILURE! Can't load resource: {path}");
            }

            return null;
        }

        public static bool TryLoadSprite(this string path, out Sprite sprite)
        {
            sprite = path.LoadResource<Sprite>();
            if (sprite == null) sprite = path.LoadAtlasSprite();
            return sprite != null;
        }

        public static Sprite LoadAtlasSprite(this string path)
        {
            try
            {
                var lastSlashIndex = path.LastIndexOf("/", StringComparison.Ordinal);

                var atlasPath = path.Remove(lastSlashIndex);
                var startIndex = lastSlashIndex + 1;
                var spriteName = path.Substring(startIndex, path.Length - startIndex);
                
                var atlas = Resources.Load<SpriteAtlas>(atlasPath);
                return atlas.GetSprite(spriteName);
            }
            catch (Exception exception)
            {
                Debug.LogError($"FAILURE! Can't load resource: '{path}' {exception}");
            }

            return null;
        }

        private static void ShowLoadingWarning(string path, DateTime startTime, double timeout)
        {
            var elapsedTime = (DateTime.Now - startTime).TotalSeconds;

            if (elapsedTime > timeout)
            {
                Debug.LogWarning("Loading the object " + path + " takes too long time.");
            }
        }
    }
}
