using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterData monsterData; 
    public MonsterType monsterType;
    public Animator monsterAnimator;
    
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    public float attackElapsedTime;
    public float skillPrevElapsedTime;
    public float skillElapsedTime;

    public int phase;
    public float curHp;
    
    public bool IsDead => curHp <= 0;
    public bool IsBeingPushedBack { get; set; }

    public bool IsAttackable => attackElapsedTime >= monsterData.attackCoolTime && Vector3.Distance(transform.position, GameWorld.Instance.PlayerManager.player.transform.position) <= 1.0f;
    
    public bool IsUsingSkill => monsterData.skillDuration > skillElapsedTime;
    public bool IsSkillCoolTimeDone => !IsUsingSkill && monsterData.skillCoolTime <= skillElapsedTime;
    public bool IsSkillAvailable => IsSkillCoolTimeDone && Vector3.Distance(transform.position, GameWorld.Instance.PlayerManager.player.transform.position) <= monsterData.skillActivationRange;

    public void MonsterInit()
    {
        curHp = monsterData.maxHp[phase];
        attackElapsedTime = 10.0f;
        skillElapsedTime = 60.0f;
    }

    void Start()
    {
        monsterAnimator = GetComponentInChildren<Animator>();
    }
}
