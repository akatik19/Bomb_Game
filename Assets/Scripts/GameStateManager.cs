using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameStateManager : MonoBehaviour
{

    [Tooltip("Prefab of the bomb game object.")]
    public GameObject Bomb;

    [Tooltip("Prefabs of the module game objects.")]
    public GameObject[] ModulePrefabs = new GameObject[0];

    private GameObject _currentBomb;

    public void InitializeEasySetup()
    {
        InitializeBomb(ShuffleModules(1));
    }

    public void InitializeMediumSetup()
    {
        InitializeBomb(ShuffleModules(2));
    }

    public void InitializeHardSetup()
    {
        InitializeBomb(ShuffleModules(3));
    }

    private GameObject[] ShuffleModules(int modulesCount)
    {
        if (modulesCount > ModulePrefabs.Length)
            Debug.LogWarningFormat("There are less available modules ({0}) than requested ({1}).", ModulePrefabs.Length, modulesCount);

        GameObject[] shuffledModules = ModulePrefabs.OrderBy(m => UnityEngine.Random.Range(0, ModulePrefabs.Length)).Take(modulesCount).ToArray();

        Debug.Log(string.Join(",", shuffledModules.Select(x => x.ToString()).ToArray()), gameObject);

        return shuffledModules;
    }

    private void InitializeBomb(GameObject[] modules)
    {
        _currentBomb = Instantiate(Bomb, transform);

        for (int i = 0; i < modules.Length; i++)
        {
            Instantiate(modules[i], _currentBomb.transform);
        }
    }

    public void SetBombActive(bool active)
    {
        _currentBomb.SetActive(active);
    }

    public void RemoveBomb()
    {
        Destroy(_currentBomb);
    }

    public void ExplodeByTime()
    {
        Debug.Log("Bomb explodes by time out.");
        var ids = GetComponentsInChildren<ModuleIdentifier>();
        foreach (ModuleIdentifier id in ids)
            SendMessageUpwards("Send", new Message { Module = id.Name, Text = "reset" });
    }

    public void ExplodeByStrikes()
    {
        Debug.Log("Bomb explodes by too many strikes.");
        var ids = GetComponentsInChildren<ModuleIdentifier>();
        foreach (ModuleIdentifier id in ids)
            SendMessageUpwards("Send", new Message { Module = id.Name, Text = "reset" });
    }
}
