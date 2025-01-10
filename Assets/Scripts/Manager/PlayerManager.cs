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

    private int _playerAbilityCount;
    private int _bulletAbilityCount;
    private int _weaponAbilityCount;

    private List<UpgradeType> _upgradeOptions;

    private List<UpgradeType> _selectedUpgradeTypes;
    public List<UpgradeType> previousSelectedUpgradeTypes;
    public Dictionary<UpgradeType, int> upgradeSelectionCounts;
    
    public void Awake()
    {
        _camera = Camera.main;
        player.playerData = GameWorld.Instance.DataManager.playerData;
        
        _playerAbilityCount = 0;
        _bulletAbilityCount = 0;
        _weaponAbilityCount = 0;
        _upgradeOptions = new List<UpgradeType>()
        {
            UpgradeType.ExpGainAmount, UpgradeType.MaxHp, UpgradeType.MaxArmor, UpgradeType.MoveSpeed, UpgradeType.DetectObjRadius,
            UpgradeType.BulletSpeed, UpgradeType.BulletFireCoolTime, UpgradeType.BulletDamage, UpgradeType.BulletReloadTime, UpgradeType.BulletMaxCount,
            UpgradeType.WeaponCooldown, UpgradeType.AllAttackRange, UpgradeType.BulletFireCount, UpgradeType.BulletPenetrationMaxCount, UpgradeType.BulletSpreadCount,
            UpgradeType.BombBulletChance, UpgradeType.FireBulletChance, UpgradeType.Missile, UpgradeType.Knife, UpgradeType.Shield, UpgradeType.FragGrenade, UpgradeType.SmokeGrenade, UpgradeType.Mine
        };
        _selectedUpgradeTypes = new List<UpgradeType>();
        previousSelectedUpgradeTypes = new List<UpgradeType>();
        
        upgradeSelectionCounts = new Dictionary<UpgradeType, int>();
    }
    
    public void PlayerAI()
    {
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
        if (Input.GetMouseButton(0) && player.IsAttackable && player.bulletCurrentCount > 0)
        {
            player.bulletFireElapsedTime = 0.0f;
            player.bulletReloadElapsedTime = 0.0f;
            player.playerAnimator.SetBool("IsReload", false);
            GameWorld.Instance.UIManager.reloadTimeImage.gameObject.SetActive(false);
            
            GameWorld.Instance.BulletManager.BulletSpawn(0);
            if (Random.value < player.playerData.fireBulletChance[player.upgradeSelectionCounts[UpgradeType.FireBulletChance]])
            
            {
                GameWorld.Instance.BulletManager.BulletSpawn( 2);
            }
        }
        else
        {
            if (player.bulletCurrentCount < player.playerData.bulletMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletMaxCount]] &&
                player.bulletFireElapsedTime >= 1.0f)
            {
                player.bulletReloadElapsedTime += Time.deltaTime;
                player.playerAnimator.SetBool("IsReload", true);
                GameWorld.Instance.UIManager.reloadTimeImage.gameObject.SetActive(true);
            }
        }

        if (player.IsMissileUsable && player.targetMonster.Count > player.playerData.missileCount[player.upgradeSelectionCounts[UpgradeType.Missile]])
        {
            player.missileUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(0, player.playerData.missileCount[player.upgradeSelectionCounts[UpgradeType.Missile]]);
        }
        if (player.IsKnifeUsable)
        {
            player.knifeUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(1, player.playerData.knifeCount[player.upgradeSelectionCounts[UpgradeType.Knife]]);
        }
        if (player.IsShieldUsable)
        {
            player.shieldUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(2, player.playerData.shieldCount[player.upgradeSelectionCounts[UpgradeType.Shield]]);
        }
        if (player.IsFragUsable && player.targetMonster.Count > player.playerData.fragCount[player.upgradeSelectionCounts[UpgradeType.FragGrenade]])
        {
            player.fragUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(3, player.playerData.fragCount[player.upgradeSelectionCounts[UpgradeType.FragGrenade]]);
        }
        if (player.IsSmokeUsable && player.targetMonster.Count > player.playerData.smokeCount[player.upgradeSelectionCounts[UpgradeType.SmokeGrenade]])
        {
            player.smokeUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(4, player.playerData.smokeCount[player.upgradeSelectionCounts[UpgradeType.SmokeGrenade]]);
        }
        if (player.IsMineUsable)
        {
            player.mineUseElapsedTime = 0.0f;
            GameWorld.Instance.WeaponManager.WeaponSpawn(5, player.playerData.mineCount[player.upgradeSelectionCounts[UpgradeType.Mine]]);
        }
    }
    
    public void PlayerMove()
    {
        // 플레이어 회전
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
        
        // 플레이어 이동
        _axis = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputDir = new Vector3(_axis.x, 0, _axis.y).normalized;
        
        // 이동 방향의 y값을 카메라의 y값에 더해줌
        Quaternion euler = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
        _moveDirection = euler * inputDir;
        
        player.transform.position += _moveDirection * (player.playerData.moveSpeed[player.upgradeSelectionCounts[UpgradeType.MoveSpeed]] * Time.deltaTime);
        
        // 보고있는 방향을 기준으로 이동 방향이 반대라면 -1, 같다면 1
        var forwardDot = Vector3.Dot(player.transform.forward, _moveDirection);
        var rightDot = Vector3.Dot(player.transform.right, _moveDirection);
        player.playerAnimator.SetFloat("Y", forwardDot);
        player.playerAnimator.SetFloat("X", rightDot);
    }

    public void ApplyUpgrade(UpgradeType upgradeType)
    {
        if (upgradeSelectionCounts.ContainsKey(upgradeType))
        {
            upgradeSelectionCounts[upgradeType]++;
        }

        switch (upgradeType)
        {
            case UpgradeType.MaxHp:
                float hpDifference = player.playerData.maxHp[upgradeSelectionCounts[UpgradeType.MaxHp]] - player.curHp;
                player.curHp = player.playerData.maxHp[upgradeSelectionCounts[UpgradeType.MaxHp]] - hpDifference;                
                break;
            case UpgradeType.MaxArmor:
                float armorDifference = player.playerData.maxArmor[upgradeSelectionCounts[UpgradeType.MaxArmor]] - player.curArmor;
                player.curArmor = player.playerData.maxArmor[upgradeSelectionCounts[UpgradeType.MaxArmor]] - armorDifference;
                break;
        }
    }
    
    
    // 현재 문제 똑같은 업그레이드만 나옴
    private void PlayerLevelUp()
    {
        System.Random random = new System.Random();
        Dictionary<UpgradeType, int> weightedOptions = new Dictionary<UpgradeType, int>();
       
        foreach (UpgradeType option in _upgradeOptions)
        {
            int weight = 1;
            if (previousSelectedUpgradeTypes.Contains(option))
            {
                weight += 2;
            }
            weightedOptions[option] = weight;
        }
        
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
                    
                    if (IsPlayerAbility(option.Key) && _playerAbilityCount < 7)
                    {
                        _selectedUpgradeTypes.Add(option.Key);
                        _playerAbilityCount++;
                    }
                    else if (IsBulletAbility(option.Key) && _bulletAbilityCount < 3)
                    {
                        _selectedUpgradeTypes.Add(option.Key);
                        _bulletAbilityCount++;
                    }
                    else if (IsWeaponAbility(option.Key) && _weaponAbilityCount < 4)
                    {
                        _selectedUpgradeTypes.Add(option.Key);
                        _weaponAbilityCount++;
                    }
                    
                    if (upgradeSelectionCounts.ContainsKey(option.Key))
                    {
                        upgradeSelectionCounts[option.Key]++;
                    }
                    else
                    {
                        upgradeSelectionCounts[option.Key] = 1;
                    }
                    break;
                }
            }
        }
        
        GameWorld.Instance.UIManager.ShowLevelUP(_selectedUpgradeTypes);
    }
    
    private bool IsPlayerAbility(UpgradeType option)
    {
        return option == UpgradeType.BulletSpeed || option == UpgradeType.BulletFireCoolTime || option == UpgradeType.BulletDamage ||
               option == UpgradeType.BulletReloadTime || option == UpgradeType.BulletMaxCount || option == UpgradeType.WeaponCooldown ||
               option == UpgradeType.AllAttackRange;
    }

    private bool IsBulletAbility(UpgradeType option)
    {
        return option == UpgradeType.BulletFireCount || option == UpgradeType.BulletPenetrationMaxCount || option == UpgradeType.BulletSpreadCount ||
               option == UpgradeType.BombBulletChance || option == UpgradeType.FireBulletChance;
    }

    private bool IsWeaponAbility(UpgradeType option)
    {
        return option == UpgradeType.Missile || option == UpgradeType.Knife || option == UpgradeType.Shield ||
               option == UpgradeType.FragGrenade || option == UpgradeType.SmokeGrenade || option == UpgradeType.Mine;
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
        if (player.curHp > player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]]) player.curHp = player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]];
    }
    
    public void GetArmorItem()
    {
        player.curArmor = player.playerData.maxArmor[player.upgradeSelectionCounts[UpgradeType.MaxArmor]];
    }
}
