using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneSettings
{
    public static bool isTraining;
    public static int AiCount;
    public static string AiTemplateFilePath;
}

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager instance;

    private const string MAIN_SCENE_NAME = "Main";
    private const string UI_SCENE_NAME = "UI";

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (!Application.isEditor)
        //{
            StartCoroutine(SetupScenes("Track1", "FixedSpeedTraining"));
        //}
    }

    public void ChangeGameMode(CarControllerMode mode, bool isTraining, string levelScene, int aiCount)
    {
        var gameScene = "";
        switch (mode)
        {
            case CarControllerMode.FixedSpeed:
                gameScene = isTraining ? "FixedSpeedTraining" : "FixedSpeedPlaying";
                break;
            case CarControllerMode.VariableSpeed:
                gameScene = isTraining ? "VariableSpeedTraining" : "VariableSpeedPlaying";
                break;
            case CarControllerMode.Physically:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();

        }

        SceneSettings.isTraining = isTraining;
        SceneSettings.AiCount = 100;
        SetupScenes(levelScene, gameScene);
    }

    private IEnumerator SetupScenes(string levelScene, string gameScene)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name != MAIN_SCENE_NAME)
            {
                yield return SceneManager.UnloadSceneAsync(scene.name);
            }
        }

        SceneManager.LoadSceneAsync(UI_SCENE_NAME, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(levelScene, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameScene));
    }
}

public enum CarControllerMode
{
    FixedSpeed,
    VariableSpeed,
    Physically
}
