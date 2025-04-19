using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager Instance;
    [Header("Cutscene")]
    [SerializeField] private string currentCutsceneId = "";
    private GameObject spawnedCutscene;
    private bool isPlayingCutscene = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject LoadCutscene(string cutsceneId)
    {
        return ResourceSystem.GetGameObject(cutsceneId);
    }

    public void PlayCutscene(string cutsceneId)
    {
        currentCutsceneId = cutsceneId;
        isPlayingCutscene = true;
        GameObject _cutscene = LoadCutscene(cutsceneId);

        // ✅ Correct way to instantiate
        spawnedCutscene = Instantiate(_cutscene, Vector3.zero, Quaternion.identity);

        // ✅ Get the PlayableDirector component
        CutsceneManager playDir = spawnedCutscene.GetComponent<CutsceneManager>();

        playDir.cutsceneStart.Invoke();
    }

    public void EndCutscene()
    {
        Destroy(this.spawnedCutscene);
        currentCutsceneId = "";
        isPlayingCutscene = false;
    }

    public string GetCurrentCutsceneId()
    {
        return currentCutsceneId;
    }

    public bool IsPlayingCutscene()
    {
        return isPlayingCutscene;
    }
}
