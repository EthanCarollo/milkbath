#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MilkBath.Editor
{
    public enum SceneContentVariant
    {
        MainMenu,
        Game,
        Sample
    }

    public static class SceneContentSetup
    {
        private const string ContentRootName = "Scene Content";
        private const string MaterialsFolder = "Assets/Settings/SceneContentMaterials";

        public static void Configure(Scene scene, SceneContentVariant variant)
        {
            RemoveExistingContent(scene);

            GameObject contentRoot = new GameObject(ContentRootName);
            SceneManager.MoveGameObjectToScene(contentRoot, scene);

            MaterialSet materials = CreateMaterials();
            CreateEnvironment(contentRoot.transform, materials, variant);
            ConfigureCamera(variant);
            ConfigureLighting(contentRoot.transform, materials, variant);

            if (variant == SceneContentVariant.Sample)
                CreateSampleOverlay();

            AssetDatabase.SaveAssets();
        }

        private static void RemoveExistingContent(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == ContentRootName)
                    Object.DestroyImmediate(root);
            }

            GameObject overlay = GameObject.Find("SampleSceneOverlay");
            if (overlay != null)
                Object.DestroyImmediate(overlay);
        }

        private static void CreateEnvironment(Transform parent, MaterialSet materials, SceneContentVariant variant)
        {
            CreatePrimitive(parent, PrimitiveType.Cube, "Floor", new Vector3(0f, -0.3f, 2f), new Vector3(18f, 0.5f, 18f), materials.Floor);
            CreatePrimitive(parent, PrimitiveType.Cube, "Back Wall", new Vector3(0f, 4.5f, 8f), new Vector3(18f, 9f, 0.5f), materials.Backdrop);

            CreatePrimitive(parent, PrimitiveType.Cylinder, "Bath Platform", new Vector3(0f, 0.15f, 2.5f), new Vector3(5.5f, 0.3f, 5.5f), materials.Platform);
            CreatePrimitive(parent, PrimitiveType.Cube, "Bath Base", new Vector3(0f, 1f, 2.5f), new Vector3(5.6f, 1.7f, 3.8f), materials.Tub);
            CreatePrimitive(parent, PrimitiveType.Cube, "Bath Rim Front", new Vector3(0f, 2f, 0.8f), new Vector3(6.2f, 0.35f, 0.45f), materials.TubAccent);
            CreatePrimitive(parent, PrimitiveType.Cube, "Bath Rim Back", new Vector3(0f, 2f, 4.2f), new Vector3(6.2f, 0.35f, 0.45f), materials.TubAccent);
            CreatePrimitive(parent, PrimitiveType.Cube, "Bath Rim Left", new Vector3(-2.9f, 2f, 2.5f), new Vector3(0.45f, 0.35f, 3.8f), materials.TubAccent);
            CreatePrimitive(parent, PrimitiveType.Cube, "Bath Rim Right", new Vector3(2.9f, 2f, 2.5f), new Vector3(0.45f, 0.35f, 3.8f), materials.TubAccent);
            CreatePrimitive(parent, PrimitiveType.Cylinder, "Milk Surface", new Vector3(0f, 2.05f, 2.5f), new Vector3(2.65f, 0.06f, 1.65f), materials.Milk);

            CreatePrimitive(parent, PrimitiveType.Sphere, "Milk Orb", new Vector3(0f, 3.25f, 2.5f), new Vector3(1.1f, 1.1f, 1.1f), materials.Glow);

            Vector3[] bubblePositions =
            {
                new Vector3(-1.7f, 3.05f, 2.25f),
                new Vector3(1.45f, 2.8f, 2.8f),
                new Vector3(-0.9f, 3.55f, 3.25f),
                new Vector3(1.05f, 3.8f, 1.9f),
                new Vector3(2.2f, 3.35f, 2.15f)
            };

            for (int i = 0; i < bubblePositions.Length; i++)
            {
                float size = 0.22f + (i % 3) * 0.1f;
                CreatePrimitive(parent, PrimitiveType.Sphere, $"Milk Bubble {i + 1}", bubblePositions[i], Vector3.one * size, materials.Glow);
            }

            CreatePedestal(parent, new Vector3(-6f, 0f, 4.5f), materials, "Left Pedestal");
            CreatePedestal(parent, new Vector3(6f, 0f, 4.5f), materials, "Right Pedestal");

            if (variant == SceneContentVariant.MainMenu)
            {
                CreatePrimitive(parent, PrimitiveType.Cube, "Menu Sign", new Vector3(0f, 5.2f, 6.8f), new Vector3(7f, 2.1f, 0.25f), materials.Sign);
            }
            else if (variant == SceneContentVariant.Game)
            {
                CreatePrimitive(parent, PrimitiveType.Cube, "Game Marker", new Vector3(0f, 0.9f, -1.5f), new Vector3(3.2f, 0.12f, 0.35f), materials.Glow);
            }
        }

        private static void CreatePedestal(Transform parent, Vector3 position, MaterialSet materials, string name)
        {
            CreatePrimitive(parent, PrimitiveType.Cylinder, name, position + new Vector3(0f, 1.1f, 0f), new Vector3(1.8f, 1.1f, 1.8f), materials.Platform);
            CreatePrimitive(parent, PrimitiveType.Sphere, name + " Orb", position + new Vector3(0f, 2.65f, 0f), Vector3.one * 0.6f, materials.Glow);
        }

        private static GameObject CreatePrimitive(Transform parent, PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = position;
            primitive.transform.localScale = scale;

            Renderer renderer = primitive.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            return primitive;
        }

        private static void ConfigureCamera(SceneContentVariant variant)
        {
            Camera camera = Camera.main;
            if (camera == null)
                return;

            Vector3 position = variant == SceneContentVariant.MainMenu
                ? new Vector3(0f, 5.2f, -13.5f)
                : new Vector3(0f, 4.6f, -12.5f);
            Vector3 target = new Vector3(0f, 2.25f, 2.5f);

            camera.transform.SetPositionAndRotation(position, Quaternion.LookRotation(target - position, Vector3.up));
            camera.fieldOfView = 45f;
            camera.backgroundColor = new Color(0.025f, 0.04f, 0.09f, 1f);
        }

        private static void ConfigureLighting(Transform parent, MaterialSet materials, SceneContentVariant variant)
        {
            GameObject directionalObject = GameObject.Find("Directional Light");
            Light directional = directionalObject != null ? directionalObject.GetComponent<Light>() : null;
            if (directional != null)
            {
                directional.color = new Color(0.7f, 0.82f, 1f);
                directional.intensity = 1.2f;
                directional.shadows = LightShadows.Soft;
                directionalObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            }

            CreatePointLight(parent, "Milk Light", new Vector3(0f, 5.5f, 2.5f), new Color(0.55f, 0.85f, 1f), 7f, 10f);
            CreatePointLight(parent, "Warm Rim Light", new Vector3(-5f, 3f, 4f), new Color(1f, 0.35f, 0.2f), 5f, 9f);
            CreatePointLight(parent, "Cool Rim Light", new Vector3(5f, 3f, 4f), new Color(0.25f, 0.45f, 1f), 5f, 9f);
        }

        private static void CreatePointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            GameObject lightObject = new GameObject(name);
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.localPosition = position;

            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static void CreateSampleOverlay()
        {
            GameObject canvasObject = new GameObject("SampleSceneOverlay");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            CreateLabel(canvas.transform, "MILKBATH // PLAYGROUND", new Vector2(0f, -430f), 26, new Color(0.75f, 0.9f, 1f, 1f));
            CreateLabel(canvas.transform, "URP SHOWCASE SCENE", new Vector2(0f, -470f), 16, new Color(0.6f, 0.68f, 0.8f, 1f));
        }

        private static void CreateLabel(Transform parent, string text, Vector2 anchoredPosition, int fontSize, Color color)
        {
            GameObject labelObject = new GameObject(text + " Label");
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(900f, 50f);
            rectTransform.anchoredPosition = anchoredPosition;

            Text label = labelObject.AddComponent<Text>();
            label.text = text;
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = fontSize;
            label.color = color;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static MaterialSet CreateMaterials()
        {
            EnsureFolder(MaterialsFolder);
            return new MaterialSet
            {
                Floor = GetOrCreateMaterial("MilkBath_Floor", new Color(0.025f, 0.04f, 0.09f), 0.15f, 0.6f),
                Backdrop = GetOrCreateMaterial("MilkBath_Backdrop", new Color(0.06f, 0.08f, 0.16f), 0f, 0.8f),
                Platform = GetOrCreateMaterial("MilkBath_Platform", new Color(0.16f, 0.2f, 0.32f), 0.55f, 0.3f),
                Tub = GetOrCreateMaterial("MilkBath_Tub", new Color(0.2f, 0.26f, 0.42f), 0.2f, 0.4f),
                TubAccent = GetOrCreateMaterial("MilkBath_TubAccent", new Color(0.45f, 0.7f, 0.95f), 0.5f, 0.2f),
                Milk = GetOrCreateMaterial("MilkBath_Milk", new Color(0.72f, 0.92f, 1f), 0f, 0.15f),
                Glow = GetOrCreateMaterial("MilkBath_Glow", new Color(0.45f, 0.85f, 1f), 0f, 0.15f, true),
                Sign = GetOrCreateMaterial("MilkBath_Sign", new Color(0.12f, 0.08f, 0.2f), 0.1f, 0.45f)
            };
        }

        private static Material GetOrCreateMaterial(string name, Color color, float metallic, float smoothness, bool emission = false)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", smoothness);
            if (emission && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.8f);
            }

            EditorUtility.SetDirty(material);
            return material;
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

        private sealed class MaterialSet
        {
            public Material Floor;
            public Material Backdrop;
            public Material Platform;
            public Material Tub;
            public Material TubAccent;
            public Material Milk;
            public Material Glow;
            public Material Sign;
        }
    }
}
#endif
