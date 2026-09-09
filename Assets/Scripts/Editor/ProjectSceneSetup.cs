#if UNITY_EDITOR
using Framework.SceneManagement;
using MilkBath.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MilkBath.Editor
{
    public static class ProjectSceneSetup
    {
        private const string ScenesFolder = "Assets/Scenes";
        private const string MainMenuScenePath = ScenesFolder + "/MainMenuScene.unity";
        private const string GameScenePath = ScenesFolder + "/GameScene.unity";
        private const string PostProcessingProfilePath = "Assets/Settings/SampleSceneProfile.asset";

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

            ConfigurePostProcessing();
            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static void ConfigurePostProcessing()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                throw new MissingReferenceException("The generated scene does not contain a MainCamera.");

            UniversalAdditionalCameraData cameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData == null)
                cameraData = mainCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();

            cameraData.renderType = CameraRenderType.Base;
            cameraData.renderPostProcessing = true;
            cameraData.volumeLayerMask = ~0;

            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(PostProcessingProfilePath);
            if (profile == null)
                throw new MissingReferenceException($"Post-processing profile not found at '{PostProcessingProfilePath}'.");

            GameObject volumeObject = new GameObject("Global Volume");
            Volume volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 0f;
            volume.weight = 1f;
            volume.sharedProfile = profile;
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
