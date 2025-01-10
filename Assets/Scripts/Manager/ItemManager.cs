using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> allItemList;
    public GameObject[] itemPrefabArray;
    private List<GameObject>[] _poolItemList;

    public Transform itemSpawnParent;
    
    private void Awake()
    {
        allItemList = new List<Item>();
        _poolItemList = new List<GameObject>[itemPrefabArray.Length];
        
        for (int i = 0; i<_poolItemList.Length; i++)
        {
            _poolItemList[i] = new List<GameObject>();
        }
    }

    private bool CheckForExpItems()
    {
        for (int i = allItemList.Count - 1; i >= 0; i--)
        {
            if (allItemList[i].itemType == ItemType.Exp)
            {
                return true;
            }
        }
        return false;
    }

    public void ItemUpdate()
    {
        if(allItemList.Count == 0) return;
        bool expItems = CheckForExpItems();
        for (int i = allItemList.Count - 1; i >= 0; i--)
        {
            Item item = allItemList[i];
            
            if (Vector3.Distance(item.transform.position, GameWorld.Instance.PlayerManager.player.transform.position) 
                < GameWorld.Instance.PlayerManager.player.playerData.detectObjRadius[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.DetectObjRadius]])
            {
                if (item.target == null)
                {
                    item.target = GameWorld.Instance.PlayerManager.player;
                    StartCoroutine(ItemMove(item));
                }
            }
        }

        if (!expItems)
        {
            Time.timeScale = 1;
        }
    }
    
    private void ItemHandle(Item item)
    {
        item.gameObject.SetActive(false);
        allItemList.Remove(item);

        switch (item.itemType)
        {
            case ItemType.Exp:
            {
                GameWorld.Instance.PlayerManager.GetExpItem(item);
                break;
            }
            case ItemType.Heal:
            {
                GameWorld.Instance.PlayerManager.GetHealItem(10);    
                break;
            }
            case ItemType.Armor:
            {
                GameWorld.Instance.PlayerManager.GetArmorItem();
                break;
            }
            case ItemType.Magnet:
            {
                for (int i = allItemList.Count - 1; i >= 0; i--)
                {
                    Item expObj = allItemList[i];
                    if (expObj.itemType == ItemType.Exp)
                    {
                        Time.timeScale = 0;
                        if (expObj.target == null)
                        {
                            expObj.target = GameWorld.Instance.PlayerManager.player;
                            StartCoroutine(ItemMove(expObj));
                        }
                    }
                }

                break;
            }
            case ItemType.Bomb:
            {
                Camera mainCamera = Camera.main;
                for (int i = GameWorld.Instance.MonsterManager.allMonsterList.Count - 1; i >= 0; i--)
                {
                    Monster monster = GameWorld.Instance.MonsterManager.allMonsterList[i];
                    Vector3 viewportPoint = mainCamera.WorldToViewportPoint(monster.transform.position);
                    if (viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
                    {
                        GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, 10);
                    }
                }

                break;
            }
        }
    }

    private IEnumerator ItemMove(Item item)
    {
        while (item.target != null)
        {
            Vector3 moveDir = item.target.transform.position - item.transform.position;
            float moveDist = moveDir.magnitude;
            moveDir.Normalize();

            float moveAmout = 5.0f * Time.unscaledDeltaTime;
            if (moveDist < moveAmout) moveAmout = moveDist;
            item.transform.Translate(moveDir * moveAmout, Space.World);
            
            if (moveDist < 0.1f)
            {
                item.target = null;
                break;
            }
            yield return null;
        }
        ItemHandle(item);
    }

    public void ItemSpawn(int index, Monster monster, Vector3 position)
    {
        bool getObject = false;
        foreach (GameObject obj in _poolItemList[index])
        {
            if (!obj.activeSelf)
            {
                getObject = true;
                obj.SetActive(true);
                Item item  = obj.GetComponent<Item>();
                item.transform.position = position;
                item.transform.rotation = Quaternion.identity;
                allItemList.Add(item);
                if (item.itemType == ItemType.Exp)
                {
                    item.expData = GameWorld.Instance.DataManager.expDic[monster.monsterData.expKey];
                }
                break;
            }
        }
        
        if (!getObject)
        {
            GameObject obj = Instantiate(itemPrefabArray[index], position, Quaternion.identity, itemSpawnParent);
            _poolItemList[index].Add(obj);
            Item item = obj.GetComponent<Item>();
            item.itemType = (ItemType) index;
            allItemList.Add(item);
            if (item.itemType == ItemType.Exp)
            {
                item.expData = GameWorld.Instance.DataManager.expDic[monster.monsterData.expKey];
            }
        }
    }
}
