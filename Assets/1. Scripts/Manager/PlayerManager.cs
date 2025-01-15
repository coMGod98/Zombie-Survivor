using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class PlayerManager : MonoBehaviour
{
    private Camera _camera;
    
    public Player player;
    public LayerMask groundMask;
    
    private Vector2 _axis;
    private Vector2 _lerpAxis;
    private Vector3 _lookDirection;
    private Vector3 _moveDirection;

    private List<UpgradeType> _upgradeOptions;
    private List<UpgradeType> _selectedUpgradeTypes;
    
    public void Awake()
    {
        _camera = Camera.main;
        player.playerData = GameWorld.Instance.DataManager.playerData;

        _upgradeOptions = new List<UpgradeType>()
        {
            UpgradeType.ExpGainAmount, UpgradeType.MaxHp, UpgradeType.MaxArmor, UpgradeType.MoveSpeed, UpgradeType.DetectObjRadius,
            UpgradeType.BulletSpeed, UpgradeType.BulletFireCoolTime, UpgradeType.BulletDamage, UpgradeType.BulletReloadTime, UpgradeType.BulletMaxCount,
            UpgradeType.WeaponCooldown, UpgradeType.AllAttackRange, UpgradeType.BulletFireCount, UpgradeType.BulletPenetrationMaxCount, UpgradeType.BulletSpreadCount,
            UpgradeType.BombBulletChance, UpgradeType.FireBulletChance, UpgradeType.Missile, UpgradeType.Knife, UpgradeType.Shield, UpgradeType.FragGrenade, UpgradeType.SmokeGrenade, UpgradeType.Mine
        };
        _selectedUpgradeTypes = new List<UpgradeType>();
    }
    
    public void PlayerAI()
    {
        if (player.IsDead)
        {
            player.playerAnimator.SetBool("IsDead", true);
            return;
        }
        
        player.bulletFireElapsedTime += Time.deltaTime;
        player.missileUseElapsedTime += Time.deltaTime;
        player.knifeUseElapsedTime += Time.deltaTime;
        player.shieldUseElapsedTime += Time.deltaTime;
        player.fragUseElapsedTime += Time.deltaTime;
        player.smokeUseElapsedTime += Time.deltaTime;
        player.mineUseElapsedTime += Time.deltaTime;
        
        if (player.bulletReloadElapsedTime >= player.playerData.bulletReloadTime[player.upgradeSelectionCounts[UpgradeType.BulletReloadTime]])
        {
            player.bulletCurrentCount = player.playerData.bulletMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletMaxCount]];
        }
        
        using (ListPool<Monster>.Get(out var rangedMonsters))
        {
            for (int i = GameWorld.Instance.MonsterManager.allMonsterList.Count - 1; i >= 0; i--)
            {
                Monster monster = GameWorld.Instance.MonsterManager.allMonsterList[i];
                if (Vector3.Distance(player.transform.position, monster.transform.position) <= 10.0f)
                    rangedMonsters.Add(monster);
            }
            
            if (rangedMonsters.Count > 0)
            {
                rangedMonsters.Sort((a, b) =>
                    Vector3.Distance(player.transform.position, a.transform.position)
                        .CompareTo(Vector3.Distance(player.transform.position, b.transform.position)));
                player.targetMonster.Clear();
                player.targetMonster.AddRange(rangedMonsters);
            }
            else
            {
                player.targetMonster.Clear();
            }
        }
    }

    public void PlayerDead()
    {
        
    }

    public void PlayerInflictDamage(float dmg)
    {
        if (player.curArmor > 0)
        {
            player.curArmor -= dmg;
            if (player.curArmor < 0)
            {
                player.curHp += player.curArmor;
                player.curArmor = 0.0f;
            }
        }
        else
        {
            player.curHp -= dmg;
        }
    }
    
    public void PlayerAttack()
    {
        if (Input.GetMouseButton(0) && player.bulletCurrentCount > 0)
        {
            player.IsFire = true;
            if (player.IsAttackable)
            {
                player.bulletCurrentCount--;
                player.bulletFireElapsedTime = 0.0f;
                player.bulletReloadElapsedTime = 0.0f;
                player.playerAnimator.SetBool("IsReload", false);
                GameWorld.Instance.UIManager.reloadTimeImage.gameObject.SetActive(false);

                GameWorld.Instance.BulletManager.BulletSpawn(0);
                if (Random.value <
                    player.playerData.fireBulletChance[player.upgradeSelectionCounts[UpgradeType.FireBulletChance]])

                {
                    GameWorld.Instance.BulletManager.BulletSpawn(2);
                }
            }
        }
        else
        {
            player.IsFire = false;
            if (player.bulletCurrentCount < player.playerData.bulletMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletMaxCount]] &&
                player.bulletFireElapsedTime >= 1.0f)
            {
                player.bulletReloadElapsedTime += Time.deltaTime;
                player.playerAnimator.SetBool("IsReload", true);
                GameWorld.Instance.UIManager.reloadTimeImage.gameObject.SetActive(true);
            }
            else
            {
                player.playerAnimator.SetBool("IsReload", false);
                GameWorld.Instance.UIManager.reloadTimeImage.gameObject.SetActive(false);
            }
        }

        if (player.IsMissileUsable && player.targetMonster.Count > player.playerData.missileCount[player.upgradeSelectionCounts[UpgradeType.Missile]]
            && GameWorld.Instance.WeaponManager.IsUsingMissile == false)
        {
            player.missileUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(0, player.playerData.missileCount[player.upgradeSelectionCounts[UpgradeType.Missile]]);
        }
        if (player.IsKnifeUsable && GameWorld.Instance.WeaponManager.IsUsingKnife == false)
        {
            player.knifeUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(1, player.playerData.knifeCount[player.upgradeSelectionCounts[UpgradeType.Knife]]);
        }
        if (player.IsShieldUsable && GameWorld.Instance.WeaponManager.IsUsingShield == false)
        {
            player.shieldUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(2, player.playerData.shieldCount[player.upgradeSelectionCounts[UpgradeType.Shield]]);
        }
        if (player.IsFragUsable && player.targetMonster.Count > player.playerData.fragCount[player.upgradeSelectionCounts[UpgradeType.FragGrenade]]
            && GameWorld.Instance.WeaponManager.IsUsingFragGrenade == false)
        {
            player.fragUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(3, player.playerData.fragCount[player.upgradeSelectionCounts[UpgradeType.FragGrenade]]);
        }
        if (player.IsSmokeUsable && player.targetMonster.Count > player.playerData.smokeCount[player.upgradeSelectionCounts[UpgradeType.SmokeGrenade]]
            && GameWorld.Instance.WeaponManager.IsUsingSmokeGrenade == false)
        {
            player.smokeUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(4, player.playerData.smokeCount[player.upgradeSelectionCounts[UpgradeType.SmokeGrenade]]);
        }
        if (player.IsMineUsable && GameWorld.Instance.WeaponManager.IsUsingMine == false)
        {
            player.mineUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(5, player.playerData.mineCount[player.upgradeSelectionCounts[UpgradeType.Mine]]);
        }
    }
    
    public void PlayerMove()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition); 
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            _lookDirection = (hit.point - player.transform.position).normalized;
            
            float rotAngle = Vector3.Angle(player.transform.forward, _lookDirection); 
            float rotDir = Vector3.Dot(player.transform.right, _lookDirection) < 0.0f ? -1.0f : 1.0f;
            float rotAmount = 1440 * Time.deltaTime;
            if (rotAngle < rotAmount) rotAmount = rotAngle;
            player.transform.Rotate(Vector3.up * (rotAmount * rotDir));
        }
        
        _axis = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputDir = new Vector3(_axis.x, 0, _axis.y).normalized;
        
        // 이동 방향의 y값을 카메라의 y값에 더해줌
        Quaternion euler = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
        _moveDirection = euler * inputDir;
        
        float moveSpeed = player.playerData.moveSpeed[player.upgradeSelectionCounts[UpgradeType.MoveSpeed]] * (player.IsFire ? 0.5f : 1.0f);
        Vector3 newPosition = player.transform.position + _moveDirection * (moveSpeed * Time.deltaTime);
        
        newPosition.x = Mathf.Clamp(newPosition.x, -100, 100);
        newPosition.z = Mathf.Clamp(newPosition.z, -100, 100);

        player.transform.position = newPosition;
        
        // 보고있는 방향을 기준으로 이동 방향이 반대라면 -1, 같다면 1
        var forwardDot = Vector3.Dot(player.transform.forward, _moveDirection);
        var rightDot = Vector3.Dot(player.transform.right, _moveDirection);
        player.playerAnimator.SetFloat("Y", forwardDot);
        player.playerAnimator.SetFloat("X", rightDot);
    }

    public void ApplyUpgrade(UpgradeType upgradeType)
    {
        player.IncreaseAbilityCount(upgradeType);
        
        if (player.upgradeSelectionCounts.ContainsKey(upgradeType))
        {
            player.upgradeSelectionCounts[upgradeType]++;
        }

        switch (upgradeType)
        {
            case UpgradeType.MaxHp:
                float hpDifference = player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]] - player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp] - 1];
                player.curHp += hpDifference;                
                break;
            case UpgradeType.MaxArmor:
                float armorDifference = player.playerData.maxArmor[player.upgradeSelectionCounts[UpgradeType.MaxArmor]] - player.curArmor;
                player.curArmor = player.playerData.maxArmor[player.upgradeSelectionCounts[UpgradeType.MaxArmor]] - armorDifference;
                break;
        }
    }
    
    public void PlayerLevelUp()
    {
        System.Random random = new System.Random();
        Dictionary<UpgradeType, int> weightedOptions = new Dictionary<UpgradeType, int>();
       
        foreach (UpgradeType option in _upgradeOptions)
        {
            if (player.upgradeSelectionCounts.TryGetValue(option, out int count))
            {
                if (count >= 4 ||
                    (count < 1 && ((player.playerAbilityCount > 6 && player.IsPlayerAbility(option)) ||
                                   (player.bulletAbilityCount > 3 && player.IsBulletAbility(option)) ||
                                   (player.weaponAbilityCount > 4 && player.IsWeaponAbility(option)))))
                {
                    continue;
                }
                weightedOptions[option] = 1 + 2 * count;
            }
            else
            {
                weightedOptions[option] = 1;
            }
        }

        _selectedUpgradeTypes.Clear();
        while (_selectedUpgradeTypes.Count < 3)
        {
            int totalWeight = weightedOptions.Values.Sum();
            int randomValue = random.Next(totalWeight);
            int cumulativeWeight = 0;

            foreach (var option in weightedOptions)
            {
                cumulativeWeight += option.Value;
                if (randomValue < cumulativeWeight)
                {
                    if (_selectedUpgradeTypes.Contains(option.Key)) continue;
                    _selectedUpgradeTypes.Add(option.Key);
                    
                    break;
                }
            }
        }
        
        GameWorld.Instance.UIManager.ShowLevelUP(_selectedUpgradeTypes);
    }
    
    

    public void GetExpItem(Item item)
    {
        bool levelChange = false;
        
        player.curExp += item.expData.expAmount[GameWorld.Instance.RoundManager.phase] * player.playerData.expGainAmount[player.upgradeSelectionCounts[UpgradeType.ExpGainAmount]];
        if (player.curExp >= player.playerData.maxExp[player.level - 1])
        {
            player.curExp = player.playerData.maxExp[player.level - 1] - player.curExp;
            player.level++;
            levelChange = true;
        }

        if (levelChange) PlayerLevelUp();
    }

    public void GetHealItem(float healAmount)
    {
        player.curHp += healAmount;
        if (player.curHp > player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]]) 
            player.curHp = player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]];
    }
    
    public void GetArmorItem()
    {
        player.curArmor = player.playerData.maxArmor[player.upgradeSelectionCounts[UpgradeType.MaxArmor]];
    }
}
