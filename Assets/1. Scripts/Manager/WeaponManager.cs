using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public LayerMask monsterMask;
    
    public List<Weapon> allKnifeList;
    public GameObject[] weaponPrefabArray;
    private List<GameObject>[] _poolWeaponList;

    public Transform weaponSpawnPoint;
    public Transform weaponSpawnParent;

    public bool IsUsingMissile;
    public bool IsUsingKnife;
    public bool IsUsingShield;  
    public bool IsUsingFragGrenade;
    public bool IsUsingSmokeGrenade;
    public bool IsUsingMine;
    
    private float degree;
    private float radius = 2.0f;
    private float gravity = -9.81f;
    private Collider[] _missileResults = new Collider[100];
    private Collider[] _bombResults = new Collider[100];
    private Collider[] _knifeResults = new Collider[100];
    private Collider[] _shieldResults = new Collider[100];
    private Collider[] _fragGrenadeResults = new Collider[100];
    private Collider[] _smokeGrenadeResults = new Collider[100];
    private Collider[] _mineResults = new Collider[100];
    
    private void Awake()
    {
        allKnifeList = new List<Weapon>();
        _poolWeaponList = new List<GameObject>[weaponPrefabArray.Length];
        
        for (int i = 0; i < weaponPrefabArray.Length; i++)
        {
            _poolWeaponList[i] = new List<GameObject>();
        }
    }

    IEnumerator Missile(Weapon missile)
    {
        IsUsingMissile = true;
        float moveAmout = 0.0f;
        while (missile.gameObject.activeSelf)
        {
            float moveDistance = 5.0f * Time.deltaTime;
            missile.transform.Translate(missile.transform.forward * moveDistance, Space.World);
            moveAmout += moveDistance;

            if (moveAmout >= 20.0f)
            {
                missile.gameObject.SetActive(false);
                yield break;
            }

            int hitCount = Physics.OverlapSphereNonAlloc(missile.transform.position, missile.transform.localScale.x, _missileResults, monsterMask);
            for (int j = 0; j < hitCount; ++j)
            {
                Collider col = _missileResults[j];
                if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out var monster))
                {
                    missile.gameObject.SetActive(false);
                    Vector3 bombPosition = new Vector3(monster.transform.position.x, 1.0f, monster.transform.position.z);

                    GameWorld.Instance.FXManager.FXSpawn(1, bombPosition, Quaternion.identity); 
                    int hitcount2 = Physics.OverlapSphereNonAlloc(missile.transform.position, 
                        missile.weaponData.hitRange * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]], 
                        _bombResults, monsterMask);
                    for (int k = 0; k < hitcount2; ++k)
                    {
                        Collider col2 = _bombResults[k];
                        if (GameWorld.Instance.MonsterManager.IsMonster(col2.gameObject.GetInstanceID(), out var monster2))
                        {
                            GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster2, missile.weaponData.weaponDamage[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.Missile]]);
                        }
                    }
                    yield break;
                }
            }
            yield return null;
        }
        IsUsingMissile = false;
    }
    
    IEnumerator Knife()
    {
        IsUsingKnife = true;
        float elapsedTime = 0.0f;
        while (elapsedTime < 10.0f)
        {
            degree += Time.deltaTime * 90.0f;
            if (degree < 360)
            {
                for (int i = 0; i < allKnifeList.Count; i++)
                {
                    Weapon knife = allKnifeList[i];
                    
                    float radian = Mathf.Deg2Rad * (degree + (i * (360.0f / allKnifeList.Count)));
                    float x = radius * Mathf.Sin(radian);
                    float z = radius * Mathf.Cos(radian);
                    allKnifeList[i].transform.position = weaponSpawnPoint.transform.position + new Vector3(x, 0, z);
                    
                    if (degree == 0)
                    {
                        knife.hitMonsterList.Clear();
                    }
                    
                    int hitCount = Physics.OverlapSphereNonAlloc(knife.transform.position, 
                        knife.weaponData.hitRange * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]],
                        _knifeResults, monsterMask);
                    for (int j = 0; j < hitCount; j++)
                    {
                        Collider col = _knifeResults[j];
                        if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out Monster monster) && !knife.hitMonsterList.Contains(monster))
                        {
                            knife.hitMonsterList.Add(monster);
                            GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster,
                                knife.weaponData.weaponDamage[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.Knife]]);
                        }
                    }
                }
            }
            else
            {
                degree = 0;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        for (int i = 0; i < allKnifeList.Count; i++)
        {
            allKnifeList[i].gameObject.SetActive(false);
        }
        allKnifeList.Clear();
        IsUsingKnife = false;
    }
    
    IEnumerator Shield(Weapon shield)
    {
        IsUsingShield = true;
        float elapsedTime = 0.0f;
        while (elapsedTime < 10.0f)
        {
            shield.transform.position = GameWorld.Instance.PlayerManager.player.transform.position;
            
            shield.attackElapsedTime += Time.deltaTime;
            if (shield.attackElapsedTime > 2.0f)
            {
                shield.attackElapsedTime = 0.0f;
                shield.hitMonsterList.Clear();
            }
            int hitCount = Physics.OverlapSphereNonAlloc(shield.transform.position, 
                shield.weaponData.hitRange * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]], 
                _shieldResults, monsterMask);
            for (int j = 0; j < hitCount; j++)
            {
                Collider col = _shieldResults[j];
                if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out Monster monster) && !shield.hitMonsterList.Contains(monster))
                {
                    shield.hitMonsterList.Add(monster);
                    GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, shield.weaponData.weaponDamage[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.Shield]]);
                }
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        shield.gameObject.SetActive(false);
        IsUsingShield = false;
    }
    
    IEnumerator FragGrenade(Weapon fragGreande)
    {
        IsUsingFragGrenade = true;
        while (fragGreande.isMoving)
        {
            fragGreande.currentVelocity.y += gravity * Time.deltaTime;
            fragGreande.transform.position += fragGreande.currentVelocity * Time.deltaTime;
            
            if (fragGreande.transform.position.y <= 0.0f)
            {
                fragGreande.isMoving = false;
                fragGreande.currentVelocity = Vector3.zero;
            }

            yield return null;
        }
        
        GameWorld.Instance.FXManager.FXSpawn(2, fragGreande.transform.position, Quaternion.identity);
        int hitcount = Physics.OverlapSphereNonAlloc(fragGreande.transform.position, 
            fragGreande.weaponData.hitRange * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]], 
            _fragGrenadeResults, monsterMask);
        for (int k = 0; k < hitcount; ++k)
        {
            Collider col = _fragGrenadeResults[k];
            if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out var monster))
            {
                GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, fragGreande.weaponData.weaponDamage[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.FragGrenade]]); 
            }
        }
        fragGreande.gameObject.SetActive(false);
        IsUsingFragGrenade = false;
    }
    
    IEnumerator SmokeGrenade(Weapon smokeGrenade)
    {
        IsUsingSmokeGrenade = true;
        while (smokeGrenade.isMoving)
        {
            smokeGrenade.currentVelocity.y += gravity * Time.deltaTime;
            smokeGrenade.transform.position += smokeGrenade.currentVelocity * Time.deltaTime;
            
            if (smokeGrenade.transform.position.y <= 0.0f)
            {
                smokeGrenade.isMoving = false;
                smokeGrenade.currentVelocity = Vector3.zero;
            }

            yield return null;
        }
        StartCoroutine(SmokeGrenadeDamage(smokeGrenade, smokeGrenade.transform.position));
        smokeGrenade.gameObject.SetActive(false);
        IsUsingSmokeGrenade = false;
    }
    
    private IEnumerator SmokeGrenadeDamage(Weapon smokeGrenade, Vector3 position)
    {
        GameWorld.Instance.FXManager.FXSpawn(3, smokeGrenade.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        for (int i = 0; i < 5; i++)
        {
            int hitcount = Physics.OverlapSphereNonAlloc(position, 
                smokeGrenade.weaponData.hitRange * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]], 
                _smokeGrenadeResults, monsterMask);
            for (int k = 0; k < hitcount; ++k)
            {
                Collider col = _smokeGrenadeResults[k];
                if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out var monster))
                {
                    GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster, smokeGrenade.weaponData.weaponDamage[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.SmokeGrenade]]);
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
    
    IEnumerator Mine(Weapon mine)
    {
        IsUsingMine = true;
        while (mine.isMoving)
        {
            mine.transform.position += Vector3.down * Time.deltaTime;
            
            if (mine.transform.position.y <= 0.0f)
            {
                mine.isMoving = false;
                mine.currentVelocity = Vector3.zero;
                mine.transform.position = new Vector3(mine.transform.position.x, 0.0f, mine.transform.position.z);
            }

            yield return null;
        }

        while (mine.gameObject.activeSelf)
        {
            for (int i = GameWorld.Instance.MonsterManager.allMonsterList.Count - 1; i >= 0; i--)
            {
                Monster monster = GameWorld.Instance.MonsterManager.allMonsterList[i];
                if (Vector3.Distance(monster.transform.position, mine.transform.position) <= 0.5f * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]])
                {
                    GameWorld.Instance.FXManager.FXSpawn(4, mine.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                    int hitcount = Physics.OverlapSphereNonAlloc(mine.transform.position, 
                        mine.weaponData.hitRange * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]], 
                        _mineResults, monsterMask);
                    for (int k = 0; k < hitcount; ++k)
                    {
                        Collider col = _mineResults[k];
                        if (GameWorld.Instance.MonsterManager.IsMonster(col.gameObject.GetInstanceID(), out var monster2))
                        {
                            GameWorld.Instance.MonsterManager.MonsterInflictDamage(monster2, mine.weaponData.weaponDamage[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.Mine]]);
                        }
                    }

                    mine.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
        IsUsingMine = false;
    }
    
    private Vector3 CalculateInitialVelocity(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - startPosition;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        float xz = toTargetXZ.magnitude;
        float y = toTarget.y;
        float radianAngle = 60.0f * Mathf.Deg2Rad;

        float v0 = Mathf.Sqrt((Mathf.Abs(gravity) * xz * xz) / (2 * (xz * Mathf.Tan(radianAngle) - y)));
        Vector3 result = toTargetXZ.normalized * v0;
        result.y = v0 * Mathf.Tan(radianAngle);

        return result;
    }
    
    public void WeaponSpawn(int index, int count)
    {
        if (count == 0) return;
        WeaponType weaponType = (WeaponType)index;

        switch (weaponType)
        {
            case WeaponType.Missile:
                SpawnMissile(index, count);
                break;
            case WeaponType.Knife:
                SpawnKnife(index, count);
                break;
            case WeaponType.Shield:
                SpawnShield(index);
                break;
            case WeaponType.FragGrenade:
            case WeaponType.SmokeGrenade:
                SpawnGrenade(index, count, weaponType);
                break;
            case WeaponType.Mine:
                SpawnMine(index, count);
                break;
        }
    }

    private void SpawnMissile(int index, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 targetPosition = GameWorld.Instance.PlayerManager.player.targetMonster[i].transform.position;
            Vector3 direction = (targetPosition - weaponSpawnPoint.transform.position).normalized;
            direction.y = 0.0f;

            Weapon missile = GetOrCreateWeapon(index, weaponSpawnPoint.position, Quaternion.LookRotation(direction));
            StartCoroutine(Missile(missile));
        }
    }

    private void SpawnKnife(int index, int count)
    {
        float angleStep = 360f / count;
        float angle = 0;

        for (int i = 0; i < count; i++)
        {
            float meleePosX = weaponSpawnPoint.position.x + Mathf.Sin((angle * Mathf.Deg2Rad)) * radius;
            float meleePosZ = weaponSpawnPoint.position.z + Mathf.Cos((angle * Mathf.Deg2Rad)) * radius;
            Vector3 meleePosVector = new Vector3(meleePosX, weaponSpawnPoint.position.y, meleePosZ);

            Weapon knife = GetOrCreateWeapon(index, meleePosVector, Quaternion.identity);
            allKnifeList.Add(knife);

            angle += angleStep;
        }
        StartCoroutine(Knife());
    }

    private void SpawnShield(int index)
    {
        Weapon shield = GetOrCreateWeapon(index, GameWorld.Instance.PlayerManager.player.transform.position, Quaternion.identity);
        StartCoroutine(Shield(shield));
    }

    private void SpawnGrenade(int index, int count, WeaponType weaponType)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 targetPosition = GameWorld.Instance.PlayerManager.player.targetMonster[i].transform.position;
            Weapon grenade = GetOrCreateWeapon(index, weaponSpawnPoint.position, Quaternion.identity);
            grenade.Initialize(CalculateInitialVelocity(weaponSpawnPoint.position, targetPosition), targetPosition);

            if (weaponType == WeaponType.FragGrenade)
            {
                StartCoroutine(FragGrenade(grenade));
            }
            else
            {
                StartCoroutine(SmokeGrenade(grenade));
            }
        }
    }

    private void SpawnMine(int index, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0.0f, 360.0f);
            float x = 1.0f * Mathf.Sin(angle);
            float z = 1.0f * Mathf.Cos(angle);
            Vector3 randomPosition = weaponSpawnPoint.position + new Vector3(x, 0.0f, z);

            Weapon mine = GetOrCreateWeapon(index, randomPosition, Quaternion.identity);
            mine.isMoving = true;
            StartCoroutine(Mine(mine));
        }
    }

    private Weapon GetOrCreateWeapon(int index, Vector3 position, Quaternion rotation)
    {
        foreach (GameObject obj in _poolWeaponList[index])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                Weapon weapon = obj.GetComponent<Weapon>();
                weapon.WeaponInit();
                weapon.weaponType = (WeaponType)index;
                weapon.weaponData = GameWorld.Instance.DataManager.weaponDic[weapon.weaponType];
                weapon.transform.localScale = Vector3.one * (weapon.weaponData.scale * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]]);
                return weapon;
            }
        }

        GameObject newObj = Instantiate(weaponPrefabArray[index], position, rotation, weaponSpawnParent);
        _poolWeaponList[index].Add(newObj);
        Weapon newWeapon = newObj.GetComponent<Weapon>();
        newWeapon.WeaponInit();
        newWeapon.weaponType = (WeaponType)index;
        newWeapon.weaponData = GameWorld.Instance.DataManager.weaponDic[newWeapon.weaponType];
        newWeapon.transform.localScale = Vector3.one * (newWeapon.weaponData.scale * GameWorld.Instance.PlayerManager.player.playerData.allAttackRange[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[UpgradeType.AllAttackRange]]);
        return newWeapon;
    }
}
