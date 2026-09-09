using Framework.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MilkBath.Scenes
{
    public sealed class MainMenuController : MonoBehaviour
    {
        private const string GameSceneName = "GameScene";

        private void Start()
        {
            SceneTransitioner transitioner = SceneTransitioner.GetOrCreate();
            CreateMenuUi(transitioner);
        }

        private static void CreateMenuUi(SceneTransitioner transitioner)
        {
            Canvas canvas = CreateCanvas("MainMenuCanvas");
            CreateLabel(canvas.transform, "MILKBATH", new Vector2(0f, 120f), 42, Color.white);

            Button startButton = CreateButton(canvas.transform, "START GAME", new Vector2(0f, -10f));
            startButton.onClick.AddListener(() => transitioner.LoadScene(GameSceneName));
        }

        private static Canvas CreateCanvas(string objectName)
        {
            GameObject canvasObject = new GameObject(objectName);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static Button CreateButton(Transform parent, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObject = new GameObject(label);
            buttonObject.transform.SetParent(parent, false);

            RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(320f, 80f);
            rectTransform.anchoredPosition = anchoredPosition;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.15f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            CreateLabel(buttonObject.transform, label, Vector2.zero, 24, Color.white);
            return button;
        }

        private static void CreateLabel(Transform parent, string text, Vector2 anchoredPosition, int fontSize, Color color)
        {
            GameObject labelObject = new GameObject(text + "Label");
            labelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = labelObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition = anchoredPosition;

            Text label = labelObject.AddComponent<Text>();
            label.text = text;
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = fontSize;
            label.color = color;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
