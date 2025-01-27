using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private int phase;
    [SerializeField] private float elapsedTime;
    private float spawnTimer = 15.0f;
    [SerializeField] private float eliteSpawnTimer = 0.0f;
    [SerializeField] private float bossSpawnTimer = 0.0f;
    private float spawnInterval = 20.0f;
    private float eliteSpawnInterval = 120.0f;
    private float bossSpawnInterval = 300.0f;
    
    public int Phase => phase;
    public float ElapsedTime => elapsedTime;
    
    private int eliteSpawnCount = 1;
    private bool isLastBossSpawned = false;
    
    private void Awake()
    {
        phase = 0;
        elapsedTime = 0.0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        eliteSpawnTimer += Time.deltaTime;
        bossSpawnTimer += Time.deltaTime;
        
        if (elapsedTime >= 660.0f)
        {
            if (GameWorld.Instance.MonsterManager.allMonsterList.Count > 0)
            {
                GameWorld.Instance.GameOver();
            }
            else
            {
                GameWorld.Instance.GameClear();
            }
        }
        else
        {
            if (isLastBossSpawned) return;
            
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0.0f;
                SpawnMonsters();
            }

            if (eliteSpawnTimer >= eliteSpawnInterval)
            {
                eliteSpawnTimer = 0.0f;
                SpawnEliteMonster();
                eliteSpawnCount += 2;
            }

            if (bossSpawnTimer >= bossSpawnInterval)
            {
                bossSpawnTimer = 0.0f;
                SpawnBossMonster();
            }
        }
        
        if (elapsedTime >= 300.0f && phase == 0)
        {
            phase++;
            eliteSpawnCount = 1;
        }
    }
    
    private void SpawnMonsters()
    {
        int[] monsterIndices = GetMonsterIndices();
        foreach (int index in monsterIndices)
        {
            int spawnCount = (index == 3 || index == 4) ? 5 : 10;
            for (int i = 0; i < spawnCount; i++)
            {
                GameWorld.Instance.MonsterManager.MonsterSpawn(index);
            }
        }
    }

    private void SpawnEliteMonster()
    {
        for (int i = 0; i < eliteSpawnCount; i++)
        {
            if (phase == 0)
            {
                GameWorld.Instance.MonsterManager.MonsterSpawn(5);
            }
            else if (phase == 1)
            {
                GameWorld.Instance.MonsterManager.MonsterSpawn(6);
            }
        }
    }

    private void SpawnBossMonster()
    {
        if (phase == 0)
        {
            GameWorld.Instance.MonsterManager.MonsterSpawn(6);
        }
        else
        {
            isLastBossSpawned = true;
            GameWorld.Instance.MonsterManager.MonsterSpawn(7);
        }
    }

    private int[] GetMonsterIndices()
    {
        if (phase == 0)
        {
            if (elapsedTime < 60.0f) // 0~1분
            {
                return new int[] { 0 };
            }
            else if (elapsedTime < 120.0f) // 1~2분
            {
                return new int[] { 0, 3 };
            }
            else if (elapsedTime < 180.0f) // 2~3분
            {
                return new int[] { 1, 3 };
            }
            else if (elapsedTime < 240.0f) // 3~4분
            {
                return new int[] { 1, 3, 4 };
            }
            else if (elapsedTime < 300.0f) // 4~5분
            {
                return new int[] { 2, 3, 4 };
            }
        }
        else if (phase == 1)
        {
            if (elapsedTime < 360.0f) // 5~6분
            {
                return new int[] { 0 };
            }
            else if (elapsedTime < 420.0f) // 6~7분
            {
                return new int[] { 0, 3 };
            }
            else if (elapsedTime < 480.0f) // 7~8분
            {
                return new int[] { 1, 3 };
            }
            else if (elapsedTime < 540.0f) // 8~9분
            {
                return new int[] { 1, 3, 4 };
            }
            else if (elapsedTime < 600.0f) // 9~10분
            {
                return new int[] { 2, 3, 4 };
            }
        }
        return new int[] { };
    }
}
