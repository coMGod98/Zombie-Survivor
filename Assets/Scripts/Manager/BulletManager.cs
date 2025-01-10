using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletManager : MonoBehaviour
{
    public List<Bullet> allBulletList;
    public GameObject[] bulletPrefabArray;
    private List<GameObject>[] _poolBulletList;
    
    public Transform playerBulletSpawnPoint;
    public Transform bulletSpawnParent;
    
    public LayerMask monsterMask;

    private Collider[] _bulletResults = new Collider[100];
    private Collider[] _bombResults = new Collider[100];

    private void Awake()
    {
        allBulletList = new List<Bullet>();
        _poolBulletList = new List<GameObject>[bulletPrefabArray.Length];

        for (int i = 0; i < _poolBulletList.Length; i++)
        {
            _poolBulletList[i] = new List<GameObject>();
        }
    }

    public void BulletDetectMonster()
    {
        Player player = GameWorld.Instance.PlayerManager.player;
        if (allBulletList.Count == 0) return;
        for (int i = allBulletList.Count - 1; i >= 0; --i)
        {
            Bullet bullet = allBulletList[i];
            float hitDamage = player.playerData.bulletDamage[player.upgradeSelectionCounts[UpgradeType.BulletDamage]] * bullet.bulletData.damageCoefficient;
            float hitRange = bullet.bulletData.hitRange * player.playerData.allAttackRange[player.upgradeSelectionCounts[UpgradeType.AllAttackRange]];
            switch (bullet.bulletType)
            {
                case BulletType.Bullet:
                {
                    int hitCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, hitRange, _bulletResults, monsterMask);
                    for (int j = 0; j < hitCount; ++j)
                    {
                        Collider col = _bulletResults[j];
                        if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out var monster) && !bullet.hitMonsterList.Contains(monster))
                        {
                            GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, hitDamage);
                            bullet.hitMonsterList.Add(monster);
                            // 관통
                            if (player.playerData.bulletPenetrationMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletPenetrationMaxCount]] <= bullet.hitMonsterList.Count)
                            {
                                allBulletList.Remove(bullet);
                                bullet.gameObject.SetActive(false);
                            }

                            // 퍼짐
                            SmallBulletSpawn(1, bullet.transform.position, player.playerData.bulletSpreadCount[player.upgradeSelectionCounts[UpgradeType.BulletSpreadCount]]);;

                            // 주변 데미지
                            if (Random.value < player.playerData.bombBulletChance[player.upgradeSelectionCounts[UpgradeType.BombBulletChance]])
                            {
                                // 총알 폭발 FX
                                GameWorld.Instance.FXManager.FXSpawn(0, bullet.transform.position);
                                int hitcount2 = Physics.OverlapSphereNonAlloc(bullet.transform.position, hitRange * 1.5f, _bombResults, monsterMask);
                                for (int k = 0; k < hitcount2; ++k)
                                {
                                    Collider col2 = _bombResults[k];
                                    if (GameWorld.Instance.MonsterManager.IsMonster(col2.gameObject.GetInstanceID(), out var monster2))
                                    {
                                        GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster2,player.playerData.bulletDamage[player.upgradeSelectionCounts[UpgradeType.BulletDamage]] * 1.5f);
                                    }
                                }
                            }
                        }
                    }

                    break;
                }

                case BulletType.SmallBullet:
                {
                    int hitCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, hitRange, _bulletResults, monsterMask);
                    for (int j = 0; j < hitCount; ++j)
                    {
                        Collider col = _bulletResults[j];
                        if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out var monster) && !bullet.hitMonsterList.Contains(monster))
                        {
                            GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, hitDamage);
                            bullet.hitMonsterList.Add(monster);
                            if (player.playerData.bulletPenetrationMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletPenetrationMaxCount]] <= bullet.hitMonsterList.Count)
                            {
                                allBulletList.Remove(bullet);
                                bullet.gameObject.SetActive(false);
                            }
                        }
                    }

                    break;
                }

                case BulletType.FireBullet:
                {
                    int hitCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, hitRange, _bulletResults,
                        monsterMask);
                    for (int j = 0; j < hitCount; ++j)
                    {
                        Collider col = _bulletResults[j];
                        if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(),
                                out var monster) && !bullet.hitMonsterList.Contains(monster))
                        {
                            GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, hitDamage);
                            bullet.hitMonsterList.Add(monster);
                            if (player.playerData.bulletPenetrationMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletPenetrationMaxCount]] +
                                2 <= bullet.hitMonsterList.Count)
                            {
                                allBulletList.Remove(bullet);
                                bullet.gameObject.SetActive(false);
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

    public void BulletMove()
    {
        if (allBulletList.Count == 0) return;
        for (int i = allBulletList.Count - 1; i >= 0; --i)
        { 
            Bullet bullet = allBulletList[i];
            float moveDistance = bullet.bulletData.bulletSpeed[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.BulletSpeed]] * Time.deltaTime;
            bullet.transform.Translate
                (bullet.moveDirection * moveDistance, Space.World);
            bullet.moveDistance += moveDistance;

            if (bullet.gameObject.activeSelf && bullet.moveDistance >= 20.0f)
            {
                allBulletList.Remove(bullet);
                bullet.gameObject.SetActive(false);
            }
        }
    }

    private void SmallBulletSpawn(int index, Vector3 position, int count)
    {
        if(count == 0) return;
        float angleStep = 360f / count;
        float angle = 0;

        for (int i = 0; i < count; i++)
        {
            float bulletDirX = position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float bulletDirZ = position.z + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector3 bulletMoveVector = new Vector3(bulletDirX, position.y, bulletDirZ);
            Vector3 bulletDir = (bulletMoveVector - position).normalized;
            Quaternion bulletRotation = Quaternion.LookRotation(bulletDir) * Quaternion.Euler(0, 90, 0);
            
            SpawnBullet(index, position, bulletRotation, bulletSpawnParent, bulletDir);
            angle += angleStep;
        }
    }
    
    private void SpawnBullet(int index, Vector3 position, Quaternion rotation, Transform parent, Vector3 direction)
    {
        bool getBullet = false;

        foreach (GameObject obj in _poolBulletList[index])
        {
            if (!obj.activeSelf)
            {
                getBullet = true;
                obj.gameObject.SetActive(true);
                Bullet bullet = obj.GetComponent<Bullet>();
                bullet.BulletInit();
                bullet.bulletType = (BulletType) index;
                bullet.transform.position = position;
                bullet.transform.rotation = rotation;
                bullet.moveDirection = direction;
                bullet.bulletData = GameWorld.Instance.DataManager.bulletDic[(BulletType) index];
                bullet.transform.localScale = Vector3.one * (bullet.bulletData.scale * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]]);
                allBulletList.Add(bullet);
                break;
            }
        }

        if (!getBullet)
        {
            GameObject obj = Instantiate(bulletPrefabArray[index], position, rotation, parent);
            _poolBulletList[index].Add(obj);
            Bullet newBullet = obj.GetComponent<Bullet>();
            newBullet.BulletInit();
            newBullet.bulletType = (BulletType) index;
            newBullet.moveDirection = direction;
            newBullet.bulletData = GameWorld.Instance.DataManager.bulletDic[(BulletType) index];
            newBullet.transform.localScale = Vector3.one * (newBullet.bulletData.scale * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]]);
            allBulletList.Add(newBullet);
        }
    }

    public void BulletSpawn(int index)
    {
        Player player = GameWorld.Instance.PlayerManager.player;
        Quaternion rotation = player.transform.rotation;
        Vector3 direction = player.transform.forward;
        BulletType bulletType = (BulletType) index;

        switch (bulletType)
        {
            case BulletType.Bullet:
                if (player.bulletCurrentCount <= 0) return;
                player.bulletCurrentCount--;

                int bulletFireCount = player.playerData.bulletFireCount[player.upgradeSelectionCounts[UpgradeType.BulletFireCount]];
                float spreadAngle = (bulletFireCount - 1) * 20.0f;

                for (int i = 0; i < bulletFireCount; i++)
                {
                    float angle = bulletFireCount > 1 ? -spreadAngle / 2 + (spreadAngle / (bulletFireCount - 1)) * i : 0.0f;
                    Quaternion shootRotation = Quaternion.Euler(0, angle, 0);
                    Quaternion bulletRotation = rotation * Quaternion.Euler(0, 90, 0) * shootRotation;
                    Vector3 bulletDirection = shootRotation * direction;

                    SpawnBullet(index, playerBulletSpawnPoint.transform.position, bulletRotation, bulletSpawnParent, bulletDirection);
                }

                break;

            case BulletType.FireBullet:
                SpawnBullet(index, playerBulletSpawnPoint.transform.position, rotation, bulletSpawnParent, direction);
                break;
        }
    }
}
