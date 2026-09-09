#if UNITY_EDITOR
using Framework.SceneManagement;
using MilkBath.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MilkBath.Editor
{
    public static class ProjectSceneSetup
    {
        private const string ScenesFolder = "Assets/Scenes";
        private const string MainMenuScenePath = ScenesFolder + "/MainMenuScene.unity";
        private const string GameScenePath = ScenesFolder + "/GameScene.unity";

        public static void ConfigureScenes()
        {
            EnsureFolder(ScenesFolder);
            CreateScene(MainMenuScenePath, "MainMenu", typeof(MainMenuController));
            CreateScene(GameScenePath, "Game", typeof(GameSceneController));

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(GameScenePath, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Configured MainMenuScene and GameScene in Build Settings.");
        }

        private static void CreateScene(string scenePath, string rootName, System.Type controllerType)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            GameObject root = new GameObject(rootName);
            root.AddComponent<SceneTransitioner>();
            root.AddComponent(controllerType);
            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static void EnsureFolder(string folderPath)
        {
            string[] segments = folderPath.Split('/');
            string currentPath = segments[0];

            for (int i = 1; i < segments.Length; i++)
            {
                string nextPath = currentPath + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(nextPath))
                    AssetDatabase.CreateFolder(currentPath, segments[i]);
                currentPath = nextPath;
            }
        }
    }
}
#endif
