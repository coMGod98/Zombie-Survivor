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

    public bool IsAttackable => attackElapsedTime >= monsterData.attackCoolTime;
    
    public bool IsUsingSkill => monsterData.skillDuration > skillElapsedTime;
    public bool IsSkillCoolTimeDone => !IsUsingSkill && monsterData.skillCoolTime <= skillElapsedTime;


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
