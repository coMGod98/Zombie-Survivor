using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    [SerializeField] private GameObject[] fxPrefabArray;
    private List<GameObject>[] _poolFXList;
    
    [SerializeField] private Transform fxSpawnParent;

    private void Awake()
    {
        _poolFXList = new List<GameObject>[fxPrefabArray.Length];
        
        for (int i = 0; i < fxPrefabArray.Length; i++)
        {
            _poolFXList[i] = new List<GameObject>();
        }
    }

    public void FXSpawn(int index, Vector3 position, Quaternion rotation)
    {
        GameWorld.Instance.SoundManager.fxAudio.PlayOneShot(GameWorld.Instance.SoundManager.bombSound, index);
        bool getFX = false;
        foreach (GameObject obj in _poolFXList[index])
        {
            if (!obj.activeSelf)
            {
                getFX = true;
                obj.gameObject.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.transform.localScale = Vector3.one *
                                           GameWorld.Instance.PlayerManager.Player.playerData.allAttackRange[
                                               GameWorld.Instance.PlayerManager.Player.upgradeSelectionCounts[UpgradeType.AllAttackRange]];
                break;  
            }
        }
        if (!getFX)
        {
            GameObject obj = Instantiate(fxPrefabArray[index], position, rotation, fxSpawnParent);
            _poolFXList[index].Add(obj);
            obj.transform.localScale = Vector3.one *
                                       GameWorld.Instance.PlayerManager.Player.playerData.allAttackRange[
                                           GameWorld.Instance.PlayerManager.Player.upgradeSelectionCounts[UpgradeType.AllAttackRange]];
        }   
    }
}
