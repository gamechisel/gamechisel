using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// ----------------------------------------------------------------
// Handles the scene switching
public class SceneLoader : MonoBehaviour
{
    // Singleton instance
    public static SceneLoader Instance { get; private set; }

    [Header("Infos")]
    public string sceneName;
    public bool switchingScene = false;

    [Header("LoadingScreen")]
    [SerializeField] private Animator animator;
    private float animationTime = 0.11f;

    // ----------------------------------------------------------------
    private void Awake()
    {
        // Initialize
        if (Instance == null)
        {
            Instance = this;
            sceneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Load Scene
    public void LoadScene(string id)
    {
        if (!switchingScene)
        {
            if (sceneName == id)
            {
                Debug.Log("Already in scene: " + id);
            }
            else
            {
                sceneName = id;
                switchingScene = true;
                StartCoroutine(LoadLocalSceneAsync(id));
            }
        }
        else
        {
            Debug.Log("Already loading a scene!");
        }
    }

    public void SetSceneName(string _sceneName)
    {
        sceneName = _sceneName;
    }

    // Local Scene Loading
    private IEnumerator LoadLocalSceneAsync(string id)
    {
         StartSceneTransitionAnimation();
         yield return new WaitForSeconds(animationTime);

        // Load scene locally
        AsyncOperation op = SceneManager.LoadSceneAsync(id);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            yield return null;
        }

        EndSceneTransitionAnimation();
        yield return new WaitForSeconds(animationTime);

        switchingScene = false;
    }

    public void StartSceneTransitionAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("close");
        }
    }

    // End of transition (e.g., fade in)
    public void EndSceneTransitionAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("open");
        }
    }
}
