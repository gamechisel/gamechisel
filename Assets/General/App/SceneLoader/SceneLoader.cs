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

                // Check if we are in a networked context
                // if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
                // {
                //     // Use Netcode's scene loading for networked environments
                //     StartCoroutine(LoadNetworkSceneAsync(id));
                //     NetworkController.Instance.SetNewSceneClientRpc(sceneName);
                // }
                // else
                // {
                // Use local scene loading for single-player
                StartCoroutine(LoadLocalSceneAsync(id));
                // }
                // 
                Debug.Log("Scene loaded: " + id);
                switchingScene = false;
            }
        }
        else
        {
            Debug.Log("Already loading a scene!");
        }
    }

    // Networked Scene Loading
    // private IEnumerator LoadNetworkSceneAsync(string id)
    // {
    //     // animator.SetTrigger("close");
    //     // yield return new WaitForSeconds(animationTime); // Wait for animation to finish

    //     // Trigger Netcode's networked scene loading
    //     NetworkManager.Singleton.SceneManager.LoadScene(id, LoadSceneMode.Single);

    //     // Wait for networked scene loading to complete (optional check)
    //     while (NetworkManager.Singleton.IsServer)
    //     {
    //         yield return null; // Wait for the scene to fully load
    //     }

    //     animator.SetTrigger("open");
    //     yield return new WaitForSeconds(animationTime);
    //     switchingScene = false;
    // }

    // Local Scene Loading
    private IEnumerator LoadLocalSceneAsync(string id)
    {
        // animator.SetTrigger("close");
        // yield return new WaitForSeconds(animationTime); // Wait for animation to finish

        // Load scene locally
        AsyncOperation op = SceneManager.LoadSceneAsync(id);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            yield return null;
        }

        animator.SetTrigger("open");
        yield return new WaitForSeconds(animationTime);
        switchingScene = false;
    }
}
