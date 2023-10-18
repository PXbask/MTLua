using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PXSceneManager : MonoBehaviour
{
    private string m_logicName = "[SceneLogic]";
    private string m_curSceneName = string.Empty;
    private void Awake()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene s1, Scene s2)
    {
        if(!s1.isLoaded || !s2.isLoaded) return;
        SceneLogic logic1 = GetSceneLogic(s1);
        SceneLogic logic2 = GetSceneLogic(s2);
        logic1?.OnInActive();
        logic2?.OnActive();
    }

    /// <summary>
    ///º§ªÓ≥°æ∞ 
    /// </summary>
    /// <param name="sceneName"></param>
    public void SetActive(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    } 
    /// <summary>
    /// µ˛º”º”‘ÿ≥°æ∞
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="luaName"></param>
    public void LoadScene(string sceneName,string luaName)
    {
        Manager.Resources.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
        });
    }
    /// <summary>
    /// «–ªª≥°æ∞
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="luaName"></param>
    public void ChangeScene(string sceneName,string luaName)
    {
        Manager.Resources.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            var tmp = this.m_curSceneName;
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
            if(!string.IsNullOrEmpty(tmp)) this.UnLoadSceneAsync(tmp);
        });
    }
    /// <summary>
    /// –∂‘ÿ≥°æ∞
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnLoadSceneAsync(string sceneName)
    {
        StartCoroutine(UnLoadScene(sceneName));
    }
    private bool IsLoadedScene(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        return scene.isLoaded;
    }
    IEnumerator StartLoadScene(string sceneName,string luaName,LoadSceneMode mode)
    {
        if (IsLoadedScene(sceneName)) yield break;
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
        async.allowSceneActivation= true;
        yield return async;
        Scene scene = SceneManager.GetSceneByName(sceneName);
        GameObject gameObject = new GameObject(m_logicName);

        SceneManager.MoveGameObjectToScene(gameObject, scene);

        this.m_curSceneName = sceneName;
        SceneLogic logic = gameObject.AddComponent<SceneLogic>();
        logic.SceneName= sceneName;
        logic.Init(luaName);
        logic.OnEnter();
    }
    IEnumerator UnLoadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(name: sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogErrorFormat("scene:{0} is not load", sceneName);
            yield break;
        }
        SceneLogic sceneLogic = GetSceneLogic(scene);
        sceneLogic?.OnQuit();
        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }
    private SceneLogic GetSceneLogic(Scene scene)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.name.Equals(m_logicName))
            {
                return gameObject.GetComponent<SceneLogic>();
            }
        }
        Debug.LogWarningFormat("scenelogic in scene:{0} is not found", scene.name);
        return null;
    }
}
