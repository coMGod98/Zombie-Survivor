using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public GameObject[] fxPrefabArray;
    private List<GameObject>[] _poolFXList;
    
    public Transform fxSpawnParent;

    private void Awake()
    {
        _poolFXList = new List<GameObject>[fxPrefabArray.Length];
        
        for (int i = 0; i < fxPrefabArray.Length; i++)
        {
            _poolFXList[i] = new List<GameObject>();
        }
    }

    public void FXSpawn(int index, Vector3 position)
    {
        bool getFX = false;
        foreach (GameObject obj in _poolFXList[index])
        {
            if (!obj.activeSelf)
            {
                getFX = true;
                obj.gameObject.SetActive(true);
                obj.transform.position = position;
                obj.transform.localScale = Vector3.one *
                                           GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[
                                               GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]];
                break;  
            }
        }
        if (!getFX)
        {
            GameObject obj = Instantiate(fxPrefabArray[index], position, Quaternion.identity, fxSpawnParent);
            _poolFXList[index].Add(obj);
            obj.transform.localScale = Vector3.one *
                                       GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[
                                           GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]];
        }   
    }
}
