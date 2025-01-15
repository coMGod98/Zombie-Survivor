using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterType
{
    NormalShort1,
    NormalShort2,
    NormalShort3,
    NormalFast,
    NormalLong,
    EliteShort,
    EliteRush,
    Boss,
}

public enum BulletType
{
    Bullet,
    SmallBullet,
    FireBullet,
}

public enum WeaponType
{
    Missile,
    Knife,
    Shield,
    FragGrenade,
    SmokeGrenade,
    Mine
}

public enum ItemType
{
    Exp,
    Heal,
    Armor,
    Magnet,
    Bomb
}

public enum ExpType
{
    TinyExp,
    SmallExp,
    MediumExp,
    LargeExp,
    HugeExp,
    GiantExp,
    MegaExp
}

public enum UpgradeType
{
    ExpGainAmount,
    MaxHp,
    MaxArmor,
    MoveSpeed,
    DetectObjRadius,
    BulletSpeed,
    BulletFireCoolTime,
    BulletDamage,
    BulletReloadTime,
    BulletMaxCount,
    WeaponCooldown,
    AllAttackRange,
    BulletFireCount,
    BulletPenetrationMaxCount,
    BulletSpreadCount,
    BombBulletChance,
    FireBulletChance,
    Missile,
    Knife,
    Shield,
    FragGrenade,
    SmokeGrenade,
    Mine
}

[Serializable]
public struct PlayerData
{
    public List<float> maxExp; // 레벨에 따른 경험치 필요량
    
    // 스탯
    public List<float> expGainAmount; // 원래 받는 경험치에 곱해서 경험치를 얻는다.
    public List<float> maxHp;
    public List<float> maxArmor;
    public List<float> moveSpeed;
    public List<float> detectObjRadius;
    
    public List<float> bulletFireCoolTime;  
    public List<float> bulletDamage;
    public List<float> bulletReloadTime;
    public List<int> bulletMaxCount;

    public List<float> weaponCooldown;
    public List<float> allAttackRange;
    
    // 총알
    public List<int> bulletFireCount;
    public List<int> bulletPenetrationMaxCount;
    public List<int> bulletSpreadCount;
    public List<float> bombBulletChance;
    public List<float> fireBulletChance; 
    
    // 무기
    public float missileUseCoolTime;
    public List<int> missileCount;
    public float knifeUseCoolTime;
    public List<int> knifeCount;
    public float shieldUseCoolTime;
    public List<int> shieldCount;
    public float fragUseCoolTime;
    public List<int> fragCount;
    public float smokeUseCoolTime;
    public List<int> smokeCount;
    public float mineUseCoolTime;
    public List<int> mineCount;
}

public struct BulletData
{
    public BulletType bulletType;
    public List<float> bulletSpeed;
    public float scale;
    public float hitRange;
    public float damageCoefficient;
}

[Serializable]
public struct WeaponData
{
    public WeaponType WeaponType;
    public float scale;
    public float hitRange;
    public List<float> weaponDamage;
}

[Serializable]
public struct MonsterData
{
    public MonsterType monsterType;
    public List<float> maxHp;
    public float moveSpeed;
    public List<float> attackDmg;
    public float attackCoolTime;
    public float skillActivationRange;
    public float skillDuration;
    public float skillCoolTime;

    public int expKey;
}

[Serializable]
public struct ExpData
{
    public int key;
    public ExpType expType;
    public List<float> expAmount;
}

[Serializable]
public struct UpgradeData
{
    public UpgradeType upgradeType;
    public int iconIndex;
    public string title;
    public List<string> description;
}

public class DataManager : MonoBehaviour
{
    public PlayerData playerData;
    public BulletData bulletData;
    public WeaponData weaponData;
    public ExpData expData;
    public UpgradeData upgradeData;
    public Dictionary<BulletType, BulletData> bulletDic;
    public Dictionary<WeaponType, WeaponData> weaponDic;
    public Dictionary<MonsterType, MonsterData> monsterDic;
    public Dictionary<int, ExpData> expDic;
    public Dictionary<UpgradeType, UpgradeData> upgradeDic;

    public void Init()
    {   
        SavePlayerData();
        SaveBulletData();
        SaveWeaponData();   
        SaveMonsterData();
        SaveExpData();
        SaveUpgradeData();  
    }

    void SavePlayerData()
    {
        playerData = new PlayerData();
        playerData.maxExp = new List<float>();
        for (int i = 0; i < 50; i++)
        {
            playerData.maxExp.Add(100.0f + i * 100.0f); // 100 200 300 400 500
        }

        playerData.expGainAmount = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.expGainAmount.Add(1.0f + i * 0.25f); // 1.0 1.25 1.5 1.75 2.0
        }

        playerData.maxHp = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.maxHp.Add(100.0f + i * 100.0f); // 100 200 300 400 500
        }

        playerData.maxArmor = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.maxArmor.Add(100.0f + i * 25.0f); // 100 125 150 175 200
        }

        playerData.moveSpeed = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.moveSpeed.Add(3.0f + i * 0.5f); // 3.0 3.5 4.0 4.5 5.0
        }

        playerData.detectObjRadius = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.detectObjRadius.Add(1.0f + i * 0.5f); // 1.0 1.5 2.0 2.5 3.0
        }

        playerData.bulletFireCoolTime = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.bulletFireCoolTime.Add(0.8f - i * 0.15f); // 0.8 0.65 0.5 0.35 0.2
        }

        playerData.bulletDamage = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.bulletDamage.Add(20.0f + i * 20.0f); // 20 40 60 80 100
        }

        playerData.bulletReloadTime = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.bulletReloadTime.Add(4.0f - i * 0.8f); // 4.0 3.2 2.4 1.6 0.8    
        }

        playerData.bulletMaxCount = new List<int>()
        {
            6, 8, 11, 15, 20
        };

        playerData.weaponCooldown = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.weaponCooldown.Add(1.0f - i * 0.15f); // 1.0 0.85 0.7 0.55 0.4
        }

        playerData.allAttackRange = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.allAttackRange.Add(1.0f + i * 0.125f); // 1.0 1.125 1.25 1.375 1.5
        }

        playerData.bulletFireCount = new List<int>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.bulletFireCount.Add(1 + i * 1); // 1 2 3 4 5
        }

        playerData.bulletPenetrationMaxCount = new List<int>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.bulletPenetrationMaxCount.Add(1 + i * 1); // 1 2 3 4 5
        }

        playerData.bulletSpreadCount = new List<int>()
        {
            0, 3, 5, 7, 9
        };

        playerData.bombBulletChance = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.bombBulletChance.Add(i * 0.1f); // 0.0 0.1 0.2 0.3 0.4
        }

        playerData.fireBulletChance = new List<float>();
        for(int i = 0; i < 5; i ++)
        {
            playerData.fireBulletChance.Add(i * 0.1f); // 0.0 0.1 0.2 0.3 0.4
        }

        playerData.missileUseCoolTime = 1.0f;
        playerData.missileCount = new List<int>()
        {
            0, 1, 1, 2, 3
        };
        playerData.knifeUseCoolTime = 1.0f;
        playerData.knifeCount = new List<int>()
        {
            0, 1, 3, 4, 5
        };
        playerData.shieldUseCoolTime = 1.0f;
        playerData.shieldCount = new List<int>()
        {
            0, 1, 1, 1, 1
        };
        playerData.fragUseCoolTime = 1.0f;
        playerData.fragCount = new List<int>()
        {
            0, 1, 1, 2, 3
        };
        playerData.smokeUseCoolTime = 1.0f;
        playerData.smokeCount = new List<int>()
        {
            0, 1, 1, 2, 3
        };
        playerData.mineUseCoolTime = 1.0f;
        playerData.mineCount = new List<int>()
        {
            0, 1, 1, 2, 2
        };
    }
    
    void SaveBulletData()
    {
        bulletDic = new Dictionary<BulletType, BulletData>();
        bulletData = new BulletData();
        
        // Bullet
        bulletData.bulletType = BulletType.Bullet;
        bulletData.bulletSpeed = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            bulletData.bulletSpeed.Add(7.0f + i * 2.0f); // 7 9 11 13 15
        }
        bulletData.scale = 1.0f;
        bulletData.hitRange = 1.0f;
        bulletData.damageCoefficient = 1.0f;
        
        bulletDic.Add(BulletType.Bullet, bulletData);
        
        // SmallBullet
        bulletData.bulletType = BulletType.SmallBullet;
        bulletData.bulletSpeed = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            bulletData.bulletSpeed.Add(7.0f + i * 2.0f); // 7 9 11 13 15
        }
        bulletData.scale = 0.5f;
        bulletData.hitRange = 0.5f;
        bulletData.damageCoefficient = 0.5f;
        
        bulletDic.Add(BulletType.SmallBullet, bulletData);
        
        // FireBullet
        bulletData.bulletType = BulletType.FireBullet;
        bulletData.bulletSpeed = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            bulletData.bulletSpeed.Add(5.0f + i * 2.0f); // 5 7 9 11 13
        }
        bulletData.scale = 1.0f;
        bulletData.hitRange = 2.0f;
        bulletData.damageCoefficient = 1.0f;
        
        bulletDic.Add(BulletType.FireBullet, bulletData);
    }
    
    void SaveWeaponData()
    {
        weaponDic = new Dictionary<WeaponType, WeaponData>();
        weaponData = new WeaponData();
        
        // Missile
        weaponData.WeaponType = WeaponType.Missile;
        weaponData.scale = 1.0f;
        weaponData.hitRange = 2.0f;
        weaponData.weaponDamage = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            weaponData.weaponDamage.Add(0.0f + i * 100.0f); // 0 100 200 300 400
        }
        
        weaponDic.Add(WeaponType.Missile, weaponData);
        
        // Knife
        weaponData.WeaponType = WeaponType.Knife;
        weaponData.scale = 1.0f;
        weaponData.hitRange = 1.0f;
        weaponData.weaponDamage = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            weaponData.weaponDamage.Add(0.0f + i * 40.0f); // 0 40 80 120 160
        }
        
        weaponDic.Add(WeaponType.Knife, weaponData);
        
        // Shield
        weaponData.WeaponType = WeaponType.Shield;
        weaponData.scale = 1.0f;
        weaponData.hitRange = 2.0f;
        weaponData.weaponDamage = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            weaponData.weaponDamage.Add(0.0f + i * 40.0f); // 0 40 80 120 160
        }
        
        weaponDic.Add(WeaponType.Shield, weaponData);
        
        // FragGrenade
        weaponData.WeaponType = WeaponType.FragGrenade;
        weaponData.scale = 1.0f;
        weaponData.hitRange = 2.0f;
        weaponData.weaponDamage = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            weaponData.weaponDamage.Add(0.0f + i * 80.0f); // 0 80 160 240 320
        }
        
        weaponDic.Add(WeaponType.FragGrenade, weaponData);
        
        // SmokeGrenade
        weaponData.WeaponType = WeaponType.SmokeGrenade;
        weaponData.scale = 1.0f;
        weaponData.hitRange = 2.0f;
        weaponData.weaponDamage = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            weaponData.weaponDamage.Add(0.0f + i * 40.0f); // 0 40 80 120 160
        }
        
        weaponDic.Add(WeaponType.SmokeGrenade, weaponData);
        
        // Mine
        weaponData.WeaponType = WeaponType.Mine;
        weaponData.scale = 1.0f;
        weaponData.hitRange = 3.0f;
        for (int i = 0; i < 5; i++)
        {
            weaponData.weaponDamage.Add(0.0f + i * 90.0f); // 0 90 180 270 360
        }
        
        weaponDic.Add(WeaponType.Mine, weaponData);
    }
    
    void SaveMonsterData()
    {
        monsterDic = new Dictionary<MonsterType, MonsterData>();
        MonsterData monsterData = new MonsterData();
        
        // NormalShort1
        monsterData.monsterType = MonsterType.NormalShort1;
        monsterData.maxHp = new List<float>()
        {
            50.0f, 300.0f
        };
        monsterData.moveSpeed = 1.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 0.0f;
        monsterData.skillDuration = 0.0f;
        monsterData.skillCoolTime = 0.0f;
        monsterData.expKey = 0;
        
        monsterDic.Add(MonsterType.NormalShort1, monsterData);
        
        // NormalShort2
        monsterData.monsterType = MonsterType.NormalShort2;
        monsterData.maxHp = new List<float>()
        {
            150.0f, 900.0f
        };
        monsterData.moveSpeed = 1.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 0.0f;
        monsterData.skillDuration = 0.0f;
        monsterData.skillCoolTime = 0.0f;
        monsterData.expKey = 1;
        
        monsterDic.Add(MonsterType.NormalShort2, monsterData);
        
        // NormalShort3
        monsterData.monsterType = MonsterType.NormalShort3;
        monsterData.maxHp = new List<float>()
        {
            250.0f, 1500.0f
        };
        monsterData.moveSpeed = 1.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 0.0f;
        monsterData.skillDuration = 0.0f;
        monsterData.skillCoolTime = 0.0f;
        monsterData.expKey = 2;
        
        monsterDic.Add(MonsterType.NormalShort3, monsterData);
        
        // NormalFast
        monsterData.monsterType = MonsterType.NormalFast;
        monsterData.maxHp = new List<float>()
        {
            100.0f, 600.0f
        };
        monsterData.moveSpeed = 3.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 0.0f;
        monsterData.skillDuration = 0.0f;
        monsterData.skillCoolTime = 0.0f;
        monsterData.expKey = 3;
        
        monsterDic.Add(MonsterType.NormalFast, monsterData);
        
        // NormalLong
        monsterData.monsterType = MonsterType.NormalLong;
        monsterData.maxHp = new List<float>()
        {
            100.0f, 600.0f
        };
        monsterData.moveSpeed = 2.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 10.0f;
        monsterData.skillDuration = 2.0f;
        monsterData.skillCoolTime = 15.0f;
        monsterData.expKey = 4;
        
        monsterDic.Add(MonsterType.NormalLong, monsterData);
        
        // EliteShort
        monsterData.monsterType = MonsterType.EliteShort;
        monsterData.maxHp = new List<float>()
        {
            500.0f, 3000.0f
        };
        monsterData.moveSpeed = 2.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 0.0f;
        monsterData.skillDuration = 0.0f;
        monsterData.skillCoolTime = 0.0f;
        monsterData.expKey = 5;
        
        monsterDic.Add(MonsterType.EliteShort, monsterData);
        
        // EliteRush
        monsterData.monsterType = MonsterType.EliteRush;
        monsterData.maxHp = new List<float>()
        {
            2000.0f, 9000.0f
        };
        monsterData.moveSpeed = 2.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 6.0f;
        monsterData.skillDuration = 5.3f;
        monsterData.skillCoolTime = 13.0f;
        monsterData.expKey = 6;
        
        monsterDic.Add(MonsterType.EliteRush, monsterData);
        
        // Boss
        monsterData.monsterType = MonsterType.Boss;
        monsterData.maxHp = new List<float>()
        {
            0.0f, 20000.0f
        };
        monsterData.moveSpeed = 3.0f;
        monsterData.attackDmg = new List<float>()
        {
            10.0f, 20.0f
        };
        monsterData.attackCoolTime = 1.0f;
        monsterData.skillActivationRange = 5.0f;
        monsterData.skillDuration = 6.2f;
        monsterData.skillCoolTime = 16.0f;
        monsterData.expKey = -1;
        
        monsterDic.Add(MonsterType.Boss, monsterData);
    }
    
    void SaveExpData()
    {
        expDic = new Dictionary<int, ExpData>();    
        expData = new ExpData();
        
        // TinyExp
        expData.key = 0;
        expData.expType = ExpType.TinyExp;
        expData.expAmount = new List<float>()
        {
            50.0f, 100.0f
        };
        
        expDic.Add(0, expData);
        
        // SmallExp
        expData.key = 1;
        expData.expType = ExpType.SmallExp;
        expData.expAmount = new List<float>()
        {
            10.0f, 20.0f
        };
        
        expDic.Add(1, expData);
        
        // MediumExp
        expData.key = 2;
        expData.expType = ExpType.MediumExp;
        expData.expAmount = new List<float>()
        {
            10.0f, 20.0f
        };
        
        expDic.Add(2, expData);
        
        // LargeExp
        expData.key = 3;
        expData.expType = ExpType.LargeExp;
        expData.expAmount = new List<float>()
        {
            10.0f, 20.0f
        };
        
        expDic.Add(3, expData);
        
        // HugeExp
        expData.key = 4;
        expData.expType = ExpType.HugeExp;
        expData.expAmount = new List<float>()
        {
            10.0f, 20.0f
        };
        
        expDic.Add(4, expData);
        
        // GiantExp
        expData.key = 5;
        expData.expType = ExpType.GiantExp;
        expData.expAmount = new List<float>()
        {
            10.0f, 20.0f
        };
        
        expDic.Add(5, expData);
        
        // MegaExp
        expData.key = 6;
        expData.expType = ExpType.MegaExp;
        expData.expAmount = new List<float>()
        {
            10.0f, 20.0f
        };
    }

    void SaveUpgradeData()
    {
        upgradeData = new UpgradeData();
        upgradeDic = new Dictionary<UpgradeType, UpgradeData>();
        
        // ExpGainAmount
        upgradeData.upgradeType = UpgradeType.ExpGainAmount;
        upgradeData.iconIndex = 0;
        upgradeData.title = "경험치 획득량 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("경험치 획득량을 25% 증가시킵니다.");
        };
        
        upgradeDic.Add(UpgradeType.ExpGainAmount, upgradeData);
        
        // MaxHP
        upgradeData.upgradeType = UpgradeType.MaxHp;
        upgradeData.iconIndex = 1;
        upgradeData.title = "최대 체력 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("최대 체력이 100 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.MaxHp, upgradeData);
        
        // MaxArmor
        upgradeData.upgradeType = UpgradeType.MaxArmor;
        upgradeData.iconIndex = 2;
        upgradeData.title = "최대 방어력 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("최대 방어력이 25 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.MaxArmor, upgradeData);
        
        // MoveSpeed
        upgradeData.upgradeType = UpgradeType.MoveSpeed;
        upgradeData.iconIndex = 3;
        upgradeData.title = "이동속도 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("이동속도가 50% 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.MoveSpeed, upgradeData);
        
        // DetectObjRadius
        upgradeData.upgradeType = UpgradeType.DetectObjRadius;
        upgradeData.iconIndex = 4;
        upgradeData.title = "아이템 획득 반경 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("아이템 획득 반경이 50% 증가합니다.");
        };        
        upgradeDic.Add(UpgradeType.DetectObjRadius, upgradeData);
        
        // BulletSpeed
        upgradeData.upgradeType = UpgradeType.BulletSpeed;
        upgradeData.iconIndex = 5;
        upgradeData.title = "총알 속도 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("총알의 속도가 2.0 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BulletSpeed, upgradeData);
        
        // BulletFireCoolTime
        upgradeData.upgradeType = UpgradeType.BulletFireCoolTime;
        upgradeData.iconIndex = 6;
        upgradeData.title = "총알 발사 속도 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("총알 발사 속도가 15% 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BulletFireCoolTime, upgradeData);
        
        // BulletDamage
        upgradeData.upgradeType = UpgradeType.BulletDamage;
        upgradeData.iconIndex = 7;
        upgradeData.title = "총알 데미지 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("총알 데미지가 20 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BulletDamage, upgradeData);
        
        // BulletReloadTime
        upgradeData.upgradeType = UpgradeType.BulletReloadTime;
        upgradeData.iconIndex = 8;
        upgradeData.title = "장전 속도 감소";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("장전 속도가 1초 감소합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BulletReloadTime, upgradeData);
        
        // BulletMaxCount
        upgradeData.upgradeType = UpgradeType.BulletMaxCount;
        upgradeData.iconIndex = 9;
        upgradeData.title = "최대 총알 수 증가";
        upgradeData.description = new List<string>()
        {
            "최대 총알의 수가 2 증가합니다.",
            "최대 총알의 수가 3 증가합니다.",
            "최대 총알의 수가 4 증가합니다.",
            "최대 총알의 수가 5 증가합니다.",
        };
        
        upgradeDic.Add(UpgradeType.BulletMaxCount, upgradeData);
        
        // WeaponCooldown
        upgradeData.upgradeType = UpgradeType.WeaponCooldown;
        upgradeData.iconIndex = 10;
        upgradeData.title = "무기 소환 쿨타임 감소";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("무기 소환 쿨타임이 15% 감소합니다.");
        };
        
        upgradeDic.Add(UpgradeType.WeaponCooldown, upgradeData);
        
        // AllAttackRange
        upgradeData.upgradeType = UpgradeType.AllAttackRange;
        upgradeData.iconIndex = 11;
        upgradeData.title = "모든 공격 범위 증가";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("공격 범위가 12.5% 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.AllAttackRange, upgradeData);
        
        // BulletFireCount
        upgradeData.upgradeType = UpgradeType.BulletFireCount;
        upgradeData.iconIndex = 12;
        upgradeData.title = "산탄 강화";
        upgradeData.description = new List<string>();
        upgradeData.description.Add("산탄을 사용할 수 있습니다.");
        for(int i = 1; i < 4; i++)
        {
            upgradeData.description.Add("한 번에 발사되는 총알의 수가 1 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BulletFireCount, upgradeData);
        
        // BulletPenetrationMaxCount
        upgradeData.upgradeType = UpgradeType.BulletPenetrationMaxCount;
        upgradeData.iconIndex = 13;
        upgradeData.title = "관통력 강화";
        upgradeData.description = new List<string>();
        for(int i = 0; i < 4; i++)
        {
            upgradeData.description.Add("총알의 관통력이 1 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BulletPenetrationMaxCount, upgradeData);
        
        // BulletSpreadCount
        upgradeData.upgradeType = UpgradeType.BulletSpreadCount;
        upgradeData.iconIndex = 14;
        upgradeData.title = "스플린터 샷 강화";
        upgradeData.description = new List<string>()
        {
            "스플린터 샷을 사용할 수 있습니다.",
            "스플린터 샷의 수가 2 증가합니다.",
            "스플린터 샷의 수가 2 증가합니다.",
            "스플린터 샷의 수가 2 증가합니다.",
        };
        
        upgradeDic.Add(UpgradeType.BulletSpreadCount, upgradeData);
        
        // BombBulletChance 
        upgradeData.upgradeType = UpgradeType.BombBulletChance;
        upgradeData.iconIndex = 15;
        upgradeData.title = "폭탄 총알 확률 증가";
        upgradeData.description = new List<string>();
        upgradeData.description.Add("폭탄 총알을 사용할 수 있습니다.");
        for(int i = 1; i < 4; i++)
        {
            upgradeData.description.Add("폭탄 총알의 확률이 10% 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.BombBulletChance, upgradeData);
        
        // FireBulletChance
        upgradeData.upgradeType = UpgradeType.FireBulletChance;
        upgradeData.iconIndex = 16;
        upgradeData.title = "화염 총알 확률 증가";
        upgradeData.description = new List<string>();
        upgradeData.description.Add("화염 총알을 사용할 수 있습니다.");
        for(int i = 1; i < 4; i++)
        {
            upgradeData.description.Add("화염 총알의 확률이 10% 증가합니다.");
        };
        
        upgradeDic.Add(UpgradeType.FireBulletChance, upgradeData);
        
        // Missile
        upgradeData.upgradeType = UpgradeType.Missile;
        upgradeData.iconIndex = 17;
        upgradeData.title = "미사일";
        upgradeData.description = new List<string>()
        {
            "미사일을 사용할 수 있습니다.",
            "미사일의 데미지가 증가합니다.",
            "미사일의 수와 데미지가 증가합니다.",
            "미사일의 수와 데미지가 증가합니다."
        };
        
        upgradeDic.Add(UpgradeType.Missile, upgradeData);
        
        // Knife
        upgradeData.upgradeType = UpgradeType.Knife;
        upgradeData.iconIndex = 18;
        upgradeData.title = "나이프";
        upgradeData.description = new List<string>()
        {
            "나이프를 사용할 수 있습니다.",
            "나이프의 수와 데미지가 증가합니다.",
            "나이프의 수와 데미지가 증가합니다.",
            "나이프의 수와 데미지가 증가합니다."
        };
        
        upgradeDic.Add(UpgradeType.Knife, upgradeData);
        
        // Shield
        upgradeData.upgradeType = UpgradeType.Shield;
        upgradeData.iconIndex = 19;
        upgradeData.title = "쉴드";
        upgradeData.description = new List<string>()
        {
            "쉴드를 사용할 수 있습니다",
            "쉴드의 데미지가 증가합니다.",
            "쉴드의 데미지가 증가합니다.",
            "쉴드의 데미지가 증가합니다."
        };
        
        upgradeDic.Add(UpgradeType.Shield, upgradeData);
        
        // FragGrenade
        upgradeData.upgradeType = UpgradeType.FragGrenade;
        upgradeData.iconIndex = 20;
        upgradeData.title = "수류탄";
        upgradeData.description = new List<string>()
        {
            "수류탄을 사용할 수 있습니다.",
            "수류탄의 데미지가 증가합니다.",
            "수류탄의 수와 데미지가 증가합니다.",
            "수류탄의 수와 데미지가 증가합니다."
        };
        
        upgradeDic.Add(UpgradeType.FragGrenade, upgradeData);
        
        // SmokeGrenade
        upgradeData.upgradeType = UpgradeType.SmokeGrenade;
        upgradeData.iconIndex = 21;
        upgradeData.title = "연막탄";
        upgradeData.description = new List<string>()
        {
            "연막탄을 사용할 수 있습니다.",
            "연막탄의 데미지가 증가합니다.",
            "연막탄의 수와 데미지가 증가합니다.",
            "연막탄의 수와 데미지가 증가합니다."
        };
        
        upgradeDic.Add(UpgradeType.SmokeGrenade, upgradeData);
        
        // Mine
        upgradeData.upgradeType = UpgradeType.Mine;
        upgradeData.iconIndex = 22;
        upgradeData.title = "지뢰";
        upgradeData.description = new List<string>()
        {
            "지뢰를 사용할 수 있습니다.",
            "지뢰의 데미지가 증가합니다.",
            "지뢰의 수와 데미지가 증가합니다.",
            "지뢰의 데미지가 증가합니다."
        };
        
        upgradeDic.Add(UpgradeType.Mine, upgradeData);
    }
}
