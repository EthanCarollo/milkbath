using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Framework.SceneManagement
{
    [DefaultExecutionOrder(-1000)]
    public sealed class SceneTransitioner : MonoBehaviour
    {
        public static SceneTransitioner Instance { get; private set; }

        [SerializeField, Min(0f)] private float fadeDuration = 0.25f;
        [SerializeField] private int sortingOrder = 10000;

        private CanvasGroup canvasGroup;
        private Coroutine fadeRoutine;
        private bool isLoading;

        public bool IsLoading => isLoading;

        public static SceneTransitioner GetOrCreate()
        {
            if (Instance != null)
                return Instance;

            return new GameObject(nameof(SceneTransitioner)).AddComponent<SceneTransitioner>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateOverlay();
            StartFade(0f);
        }

        public void LoadScene(string sceneName)
        {
            if (!isLoading && !string.IsNullOrWhiteSpace(sceneName))
                StartCoroutine(LoadSceneRoutine(sceneName));
        }

        public void LoadScene(int buildIndex)
        {
            if (!isLoading && buildIndex >= 0)
                StartCoroutine(LoadSceneRoutine(buildIndex));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene '{sceneName}' is not enabled in the Build Settings.");
                yield break;
            }

            isLoading = true;
            yield return FadeAndWait(1f);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            while (loadOperation != null && !loadOperation.isDone)
                yield return null;

            yield return FadeAndWait(0f);
            isLoading = false;
        }

        private IEnumerator LoadSceneRoutine(int buildIndex)
        {
            if (!Application.CanStreamedLevelBeLoaded(buildIndex))
            {
                Debug.LogError($"Build index '{buildIndex}' is not enabled in the Build Settings.");
                yield break;
            }

            isLoading = true;
            yield return FadeAndWait(1f);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);
            while (loadOperation != null && !loadOperation.isDone)
                yield return null;

            yield return FadeAndWait(0f);
            isLoading = false;
        }

        private IEnumerator FadeAndWait(float targetAlpha)
        {
            StartFade(targetAlpha);
            if (fadeRoutine != null)
                yield return fadeRoutine;
        }

        private void StartFade(float targetAlpha)
        {
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
        }

        private IEnumerator FadeRoutine(float targetAlpha)
        {
            float initialAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            float duration = Mathf.Max(0f, fadeDuration);

            canvasGroup.blocksRaycasts = targetAlpha > 0f;

            if (duration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                yield break;
            }

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.blocksRaycasts = targetAlpha > 0f;
        }

        private void CreateOverlay()
        {
            GameObject canvasObject = new GameObject("SceneTransitionCanvas");
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject blackScreenObject = new GameObject("BlackScreen");
            blackScreenObject.transform.SetParent(canvasObject.transform, false);

            RectTransform rectTransform = blackScreenObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Image blackScreen = blackScreenObject.AddComponent<Image>();
            blackScreen.color = Color.black;

            canvasGroup = blackScreenObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
        }
    }
}
