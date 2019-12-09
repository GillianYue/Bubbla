//script from https://www.windykeep.com/2018/02/15/make-loading-screen-unity/

using UnityEngine;
using UnityEngine.UI;
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    // The reference to the current loading operation running in the background:
    private AsyncOperation currentLoadingOperation;
    // A flag to tell whether a scene is being loaded or not:
    private bool isLoading;
    // The rect transform of the bar fill game object:

    //[SerializeField]
    //private RectTransform barFillRectTransform;
    // Initialize as the initial local scale of the bar fill game object. Used to cache the Y-value (just in case):
    //private Vector3 barFillLocalScale;
    // The text that shows how much has been loaded:

    [SerializeField]
    private Text percentLoadedText;
    private void Awake()
    {
        // Singleton logic:
        if (Instance == null)
        {
            Instance = this;
            // Don't destroy the loading screen while switching scenes:
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // Save the bar fill's initial local scale:
        // barFillLocalScale = barFillRectTransform.localScale;
        Hide();
    }
    private void Update()
    {
        if (isLoading)
        {
            // Get the progress and update the UI. Goes from 0 (start) to 1 (end):
            SetProgress(currentLoadingOperation.progress);
            // If the loading is complete, hide the loading screen:
            if (currentLoadingOperation.isDone)
            {
                Hide();
            }
        }
    }
    // Updates the UI based on the progress:
    private void SetProgress(float progress)
    {
        // Update the fill's scale based on how far the game has loaded:
      //  barFillLocalScale.x = progress;
        // Update the rect transform:
      //  barFillRectTransform.localScale = barFillLocalScale;

        // Set the percent loaded text:
        percentLoadedText.text = Mathf.CeilToInt(progress * 100).ToString() + "%";
    }
    // Call this to show the loading screen.
    // We can determine the loading's progress when needed from the AsyncOperation param:
    public void Show(AsyncOperation loadingOperation)
    {
        // Enable the loading screen:
        gameObject.SetActive(true);
        // Store the reference:
        currentLoadingOperation = loadingOperation;
        // Reset the UI:
        SetProgress(0f);
        isLoading = true;
    }
    // Call this to hide it:
    public void Hide()
    {
        // Disable the loading screen:
        gameObject.SetActive(false);
        currentLoadingOperation = null;
        isLoading = false;
    }
}