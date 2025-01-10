using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public Animator playerAnimator;
    public PlayerData playerData;
    
    public Dictionary<UpgradeType, int> upgradeSelectionCounts;
    
    public int level; // maxExp랑 연결
    
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
    public bool IsMissileUsable => missileUseElapsedTime >= playerData.missileUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]];
    public bool IsKnifeUsable => knifeUseElapsedTime >= playerData.knifeUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]]; 
    public bool IsShieldUsable => shieldUseElapsedTime >= playerData.shieldUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]];
    public bool IsFragUsable => fragUseElapsedTime >= playerData.fragUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]];
    public bool IsSmokeUsable => smokeUseElapsedTime >= playerData.smokeUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]];
    public bool IsMineUsable => mineUseElapsedTime >= playerData.mineUseCoolTime * playerData.weaponCooldown[upgradeSelectionCounts[UpgradeType.WeaponCooldown]];
    
    void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
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
    }
}
