using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public Animator playerAnimator;
    public AudioSource playerSound;

    public PlayerData playerData;

    public bool IsFire;
    
    public Dictionary<UpgradeType, int> upgradeSelectionCounts;
    public int playerAbilityCount;
    public int bulletAbilityCount;
    public int weaponAbilityCount;
    
    public int level;
    public float curExp;
    public float curHp;
    public float curArmor;
    public int bulletCurrentCount;
    public float bulletFireElapsedTime;
    public float bulletReloadElapsedTime;
    public float missileUseElapsedTime;
    public float knifeUseElapsedTime;
    public float shieldUseElapsedTime;
    public float fragUseElapsedTime;
    public float smokeUseElapsedTime;
    public float mineUseElapsedTime;
    
    public List<Monster> targetMonster;
    
    public bool IsDead => curHp <= 0;

    public bool IsAttackable => bulletFireElapsedTime >= playerData.bulletFireCoolTime[upgradeSelectionCounts[UpgradeType.BulletFireCoolTime]];
    public bool IsMissileUsable => missileUseElapsedTime >= playerData.missileUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]
    && upgradeSelectionCounts[UpgradeType.Missile] > 0;
    public bool IsKnifeUsable => knifeUseElapsedTime >= playerData.knifeUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]
    && upgradeSelectionCounts[UpgradeType.Knife] > 0; 
    public bool IsShieldUsable => shieldUseElapsedTime >= playerData.shieldUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]
    && upgradeSelectionCounts[UpgradeType.Shield] > 0;
    public bool IsFragUsable => fragUseElapsedTime >= playerData.fragUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]
    && upgradeSelectionCounts[UpgradeType.FragGrenade] > 0;
    public bool IsSmokeUsable => smokeUseElapsedTime >= playerData.smokeUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]
    && upgradeSelectionCounts[UpgradeType.SmokeGrenade] > 0;
    public bool IsMineUsable => mineUseElapsedTime >= playerData.mineUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]
    && upgradeSelectionCounts[UpgradeType.Mine] > 0;
    
    void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerSound = GetComponent<AudioSource>();
        
        targetMonster = new List<Monster>();
        
        upgradeSelectionCounts = new Dictionary<UpgradeType, int>
        {
            { UpgradeType.ExpGainAmount, 0 },
            { UpgradeType.MaxHp, 0 },
            { UpgradeType.MaxArmor, 0 },
            { UpgradeType.MoveSpeed, 0 },
            { UpgradeType.DetectObjRadius , 0 },
            { UpgradeType.BulletSpeed, 0 },
            { UpgradeType.BulletFireCoolTime, 0 },
            { UpgradeType.BulletDamage, 0 },
            { UpgradeType.BulletReloadTime, 0 },
            { UpgradeType.BulletMaxCount, 0 },
            { UpgradeType.WeaponCooldown, 0 },
            { UpgradeType.AllAttackRange, 0 },
            { UpgradeType.BulletFireCount, 0 },
            { UpgradeType.BulletPenetrationMaxCount, 0 },
            { UpgradeType.BulletSpreadCount, 0 },
            { UpgradeType.BombBulletChance, 0 },
            { UpgradeType.FireBulletChance, 0 },
            { UpgradeType.Missile, 0 },
            { UpgradeType.Knife, 0 },
            { UpgradeType.Shield, 0 },
            { UpgradeType.FragGrenade, 0 },
            { UpgradeType.SmokeGrenade, 0 },
            { UpgradeType.Mine, 0 }
        };
        
        playerData = GameWorld.Instance.DataManager.playerData;
        
        level = 1;
        curExp = 0.0f;
        curHp = playerData.maxHp[upgradeSelectionCounts[UpgradeType.MaxHp]];
        curArmor = 0.0f;
        
        bulletFireElapsedTime = 0.0f;
        bulletReloadElapsedTime = 0.0f;
        bulletCurrentCount = playerData.bulletMaxCount[upgradeSelectionCounts[UpgradeType.BulletMaxCount]];
        
        missileUseElapsedTime = 40.0f;
        knifeUseElapsedTime = 40.0f;
        shieldUseElapsedTime = 40.0f;
        fragUseElapsedTime = 40.0f;
        smokeUseElapsedTime = 40.0f;
        mineUseElapsedTime = 40.0f;
    }
    
    public void IncreaseAbilityCount(UpgradeType upgradeType)
    {
        if (IsPlayerAbility(upgradeType) && upgradeSelectionCounts[upgradeType] < 1)
            playerAbilityCount++;
        else if (IsBulletAbility(upgradeType) && upgradeSelectionCounts[upgradeType] < 1)
            bulletAbilityCount++;
        else if (IsWeaponAbility(upgradeType) && upgradeSelectionCounts[upgradeType] < 1)
            weaponAbilityCount++;
    }
    
    public bool IsPlayerAbility(UpgradeType option)
    {
        return option == UpgradeType.BulletSpeed || option == UpgradeType.BulletFireCoolTime || option == UpgradeType.BulletDamage ||
               option == UpgradeType.BulletReloadTime || option == UpgradeType.BulletMaxCount || option == UpgradeType.WeaponCooldown ||
               option == UpgradeType.AllAttackRange;
    }

    public bool IsBulletAbility(UpgradeType option)
    {
        return option == UpgradeType.BulletFireCount || option == UpgradeType.BulletPenetrationMaxCount || option == UpgradeType.BulletSpreadCount ||
               option == UpgradeType.BombBulletChance || option == UpgradeType.FireBulletChance;
    }

    public bool IsWeaponAbility(UpgradeType option)
    {
        return option == UpgradeType.Missile || option == UpgradeType.Knife || option == UpgradeType.Shield ||
               option == UpgradeType.FragGrenade || option == UpgradeType.SmokeGrenade || option == UpgradeType.Mine;
    }
}
