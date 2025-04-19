using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("Reference")]
    public UnityEvent cutsceneStart;

    public void StartCutscene()
    {
        cutsceneStart.Invoke();
    }
}
