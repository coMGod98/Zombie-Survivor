using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public List<Monster> allMonsterList;
    public GameObject[] monsterPrefabArray;
    private List<GameObject>[] _poolMonsterList;
    
    public GameObject monsterBulletPrefab;
    private List<GameObject> _poolMonsterBullet;
    
    public Transform monsterSpawnParent;
    
    public LayerMask playerMask;    
    private Dictionary<int, Monster> _objectIdToMonster;
    private Dictionary<int, float> _randomWeightValues;
    private Collider[] _bulletResults = new Collider[100];
    private Collider[] _rushResults = new Collider[100];
    private Collider[] _skillResults = new Collider[100];
    
    private void Awake()
    {
        allMonsterList = new List<Monster>();
        _poolMonsterList = new List<GameObject>[monsterPrefabArray.Length];
        _poolMonsterBullet = new List<GameObject>();
        _objectIdToMonster = new Dictionary<int, Monster>();
        
        for (int i = 0; i<_poolMonsterList.Length; i++)
        {
            _poolMonsterList[i] = new List<GameObject>();
        }

        _randomWeightValues = new Dictionary<int, float>()
        {
            { 1, 40.0f },
            { 2, 40.0f },
            { 3, 20.0f },
            { 4, 10.0f }
        };
    }

    public bool IsMonster(int objectId, out Monster outMonster)
    {
        outMonster = null;
        if (_objectIdToMonster.TryGetValue(objectId, out Monster value))
        {
            outMonster = value;
            return true;
        }
        return false;
    }

    private int GetRandomPick()
    {
        float totalWeight = 0.0f;
        foreach (float weight in _randomWeightValues.Values)
        {
            totalWeight += weight;
        }

        float randomvalue = Random.value * totalWeight;
        float cumulativeWeight = 0.0f;

        foreach (KeyValuePair<int, float> item in _randomWeightValues)
        {
            cumulativeWeight += item.Value;
            if (randomvalue < cumulativeWeight)
            {
                return item.Key;
            }
        }
        return 0;
    }
    
    public void MonsterAI()
    {
        if (allMonsterList.Count == 0) return;
        for (int i = allMonsterList.Count - 1; i >= 0; --i)
        {
            Monster monster = allMonsterList[i];
            
            if (monster.IsDead)
            {
                MonsterDead(monster);
                continue;
            }
            
            monster.attackElapsedTime += Time.deltaTime;
            MonsterAttack(monster);

            if (monster.monsterType == MonsterType.Boss || monster.monsterType == MonsterType.EliteRush ||
                monster.monsterType == MonsterType.NormalLong)
            {
                monster.skillPrevElapsedTime = monster.skillElapsedTime;
                monster.skillElapsedTime += Time.deltaTime;
                MonsterUseSkill(monster);
            }
        }
    }
    
    public void MonsterInflictDamage(Monster monster, float damage)
    {
        if (monster.IsDead) return;
        monster.curHp -= damage;
        GameWorld.Instance.UIManager.ShowDamageText(monster.transform.position + Vector3.up * 1.0f, damage);
    }
    
    private void MonsterDead(Monster monster)
    {
        monster.monsterAnimator.SetTrigger("TDead");
        allMonsterList.Remove(monster);
        GameWorld.Instance.UIManager.RemoveMonsterHpBar(monster);
        StartCoroutine(DisApearing(monster));   
        
        GameWorld.Instance.ItemManager.ItemSpawn(0, monster, monster.transform.position);
        float randomValue = Random.value;
        if (randomValue < 0.05f)
        {
            int randomPick = GetRandomPick();
            float radius = 0.5f;
            float angle = Random.Range(0.0f, 360.0f);
            float x = radius * Mathf.Sin(angle);
            float z = radius * Mathf.Cos(angle);
                    
            Vector3 randomVector = new Vector3(x, 0.0f, z);
            Vector3 randomPosition = monster.transform.position + randomVector;
                    
            GameWorld.Instance.ItemManager.ItemSpawn(randomPick, monster, randomPosition);
        }
    }
    
    private IEnumerator DisApearing(Monster monster)
    {
        yield return new WaitForSeconds(4.0f);
        
        monster.gameObject.SetActive(false);
    }
    

    private void MonsterAttack(Monster monster)
    {
        if (!monster.IsAttackable || monster.IsUsingSkill) return;
        monster.attackElapsedTime = 0.0f;
        GameWorld.Instance.PlayerManager.PlayerInflictDamage(monster.monsterData.attackDmg[GameWorld.Instance.RoundManager.phase]);
    }

    private void MonsterUseSkill(Monster monster)
    {
        switch (monster.monsterType)
        {
            case MonsterType.NormalLong:
                if (monster.IsSkillAvailable)
                {
                    monster.monsterAnimator.SetBool("IsAttack", true);
                    monster.skillElapsedTime = 0.0f;
                }
                else
                {
                    monster.monsterAnimator.SetBool("IsAttack", false);
                }
                
                if (monster.skillElapsedTime > 1.0f && monster.skillPrevElapsedTime < 1.0f)
                {
                    MonsterBulletSpawn(monster);
                }
                break;
            case MonsterType.EliteRush:
                if (monster.IsSkillAvailable)
                {
                    StartCoroutine(Rush(monster));
                    monster.skillElapsedTime = 0.0f;
                }
                
                break;
            case MonsterType.Boss:
                if (monster.IsSkillAvailable)
                {
                    int randomSkill = Random.Range(0, 2);
                    switch (randomSkill)
                    {
                        case 0:
                            StartCoroutine(Scream(monster));
                            break;
                        case 1:
                            StartCoroutine(Jump(monster));
                            break;
                    }
                    monster.skillElapsedTime = 0.0f;
                }
                break;
        }
    }

    private void MonsterBulletSpawn(Monster monster)
    {
        Vector3 monsterBulletSpawnPosition = monster.transform.position + Vector3.up;
        Quaternion monsterBulletRotation = monster.transform.rotation;
        
        bool getBullet = false;
        foreach (GameObject obj in _poolMonsterBullet)
        {
            if(!obj.activeSelf)
            {
                getBullet = true;
                obj.SetActive(true);
                obj.transform.position = monsterBulletSpawnPosition;
                obj.transform.rotation = monsterBulletRotation;
                StartCoroutine(MonsterBulletMove(obj, monster.monsterData.attackDmg[GameWorld.Instance.RoundManager.phase] * 2.0f));

                break;
            }
        }
        
        if (!getBullet)
        {
            GameObject obj = Instantiate(monsterBulletPrefab, monsterBulletSpawnPosition, monsterBulletRotation, monsterSpawnParent);
            _poolMonsterBullet.Add(obj);
            StartCoroutine(MonsterBulletMove(obj, monster.monsterData.attackDmg[GameWorld.Instance.RoundManager.phase] * 2.0f));
        }
    }
    
    IEnumerator MonsterBulletMove(GameObject obj, float dmg)
    {
        float moveAmout = 0.0f;
        while (obj.gameObject.activeSelf)
        {
            float moveDistance = 4.0f * Time.deltaTime;
            
            obj.transform.Translate(obj.transform.forward * moveDistance, Space.World);
            moveAmout += moveDistance;
            
            if (moveAmout >= 20.0f)
            {
                obj.gameObject.SetActive(false);
                yield break;
            }
            
            int hitCount = Physics.OverlapSphereNonAlloc(obj.transform.position, 1.0f, _bulletResults, playerMask);
            for (int j = 0; j < hitCount; ++j)
            {
                Collider col = _bulletResults[j];
                if (col.gameObject.GetInstanceID() == GameWorld.Instance.PlayerManager.player.gameObject.GetInstanceID())
                {
                    GameWorld.Instance.PlayerManager.PlayerInflictDamage(dmg);
                    obj.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }

    private IEnumerator Rush(Monster monster)
    {
        monster.monsterAnimator.SetBool("IsUseSkill", true);
        monster.meshFilter.mesh = CreateRectangleMesh(2, 10);
        monster.meshRenderer.enabled = true;
        
        float waitTime = 3.0f;
        float elapsedTime = 0.0f;
        while (elapsedTime < waitTime)
        {
            if (monster.IsDead)
            {
                monster.meshRenderer.enabled = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        monster.meshRenderer.enabled = false;
        monster.monsterAnimator.SetBool("IsRush", true);

        float moveDist = 10.0f;
        bool isHit = false;
        
        while (moveDist > 0.0f)
        {
            if (monster.IsDead)
            {
                monster.meshRenderer.enabled = false;
                yield break;
            }
            
            float moveAmount = Mathf.Min(7.0f * Time.deltaTime, moveDist);
            monster.transform.Translate(monster.transform.forward * moveAmount, Space.World);
            moveDist -= moveAmount;

            if (isHit)
            {
                yield return null;
                continue;
            }

            int hitCount = Physics.OverlapSphereNonAlloc(monster.transform.position, 2.0f, _rushResults, playerMask);
            for(int i = 0; i < hitCount; ++i)
            {
                Collider col = _rushResults[i];
                if (col.gameObject.GetInstanceID() == GameWorld.Instance.PlayerManager.player.gameObject.GetInstanceID())
                {
                    GameWorld.Instance.PlayerManager.PlayerInflictDamage(monster.monsterData.attackDmg[GameWorld.Instance.RoundManager.phase] * 2.0f);
                    isHit = true;
                    
                    break;
                }
            }
            
            yield return null;
        }
        monster.monsterAnimator.SetBool("IsUseSkill", false);
        monster.monsterAnimator.SetBool("IsRush", false);
    }

    private IEnumerator Scream(Monster monster)
    {
        monster.monsterAnimator.SetBool("IsUseSkill", true);
        monster.meshFilter.mesh = CreateSectorMesh(9.0f, 120, 10);
        monster.meshRenderer.enabled = true;
        
        float waitTime = 3.5f;
        float elapsedTime = 0.0f;
        while (elapsedTime < waitTime)
        {
            if (monster.IsDead)
            {
                monster.meshRenderer.enabled = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        monster.meshRenderer.enabled = false;
        monster.monsterAnimator.SetBool("IsScream", true);

        float prevElapsedTime = 0.0f;
        elapsedTime = 0.0f;
        while (elapsedTime < 1.8f)
        {
            if (monster.IsDead)
            {
                monster.meshRenderer.enabled = false;
                yield break;
            }
            prevElapsedTime = elapsedTime;
            elapsedTime += Time.deltaTime;
            if (prevElapsedTime < 1.3 && elapsedTime >= 1.3)
            {
                int hitCount = Physics.OverlapSphereNonAlloc(monster.transform.position, 9.0f, _skillResults, playerMask);
                for (int i = 0; i < hitCount; ++i)
                {
                    Collider col = _skillResults[i];
                    Vector3 directionToPlayer = (col.transform.position - monster.transform.position).normalized;
                    float angleToPlayer = Vector3.Angle(monster.transform.forward, directionToPlayer);
                    if (angleToPlayer <= 120.0f / 2)
                    {
                        if (col.gameObject.GetInstanceID() == GameWorld.Instance.PlayerManager.player.gameObject.GetInstanceID())
                        {
                            GameWorld.Instance.PlayerManager.PlayerInflictDamage(monster.monsterData.attackDmg[GameWorld.Instance.RoundManager.phase] * 2.0f);
                            Debug.Log("hit");
                            break;
                        }
                    }
                }
            }

            yield return null;
        }
        monster.monsterAnimator.SetBool("IsUseSkill", false);
        monster.monsterAnimator.SetBool("IsScream", false);
    }

    private IEnumerator Jump(Monster monster)
    {
        monster.monsterAnimator.SetBool("IsUseSkill", true);
        monster.meshFilter.mesh = CreateCircleMesh(8, 1000);
        monster.meshRenderer.enabled = true;
        
        float waitTime = 3.0f;
        float elapsedTime = 0.0f;
        while (elapsedTime < waitTime)
        {
            if (monster.IsDead)
            {
                monster.meshRenderer.enabled = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        monster.meshRenderer.enabled = false;
        monster.monsterAnimator.SetBool("IsJump", true);
        
        float prevElapsedTime = 0.0f;
        elapsedTime = 0.0f;
        while (elapsedTime < 2.0f)
        {
            if (monster.IsDead)
            {
                monster.meshRenderer.enabled = false;
                yield break;
            }
            
            prevElapsedTime = elapsedTime;
            elapsedTime += Time.deltaTime;
            if (prevElapsedTime < 1.5 && elapsedTime >= 1.5)
            {
                int hitCount = Physics.OverlapSphereNonAlloc(monster.transform.position, 8.0f, _skillResults, playerMask);
                for (int i = 0; i < hitCount; ++i)
                {
                    Collider col = _skillResults[i];
                    if (col.gameObject.GetInstanceID() == GameWorld.Instance.PlayerManager.player.gameObject.GetInstanceID())
                    {
                        GameWorld.Instance.PlayerManager.PlayerInflictDamage(monster.monsterData.attackDmg[GameWorld.Instance.RoundManager.phase] * 2.0f);
                        Debug.Log("hit");
                        break;
                    }
                
                }
            }
            
            yield return null;
        }
        monster.monsterAnimator.SetBool("IsUseSkill", false);
        monster.monsterAnimator.SetBool("IsJump", false);
    }
    
    private Mesh CreateRectangleMesh(float width, float height)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        // 직사각형의 네 꼭짓점 설정
        vertices[0] = new Vector3(-width / 2, 0.01f, 0);
        vertices[1] = new Vector3(width / 2, 0.01f, 0);
        vertices[2] = new Vector3(-width / 2, 0.01f, height);
        vertices[3] = new Vector3(width / 2, 0.01f, height);

        // 삼각형 인덱스 설정
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        return mesh;
    }
    
    private Mesh CreateCircleMesh(float radius, int segments)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = new Vector3(0, 0.01f, 0);
        float angle = 0f;
        for (int i = 1; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            vertices[i] = new Vector3(x, 0.01f, z);
            angle += 360f / segments;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2 > segments) ? 1 : i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }
    
    private Mesh CreateSectorMesh(float radius, float angle, int segments)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = new Vector3(0, 0.01f, 0);
        float currentAngle = -angle / 2;
        float angleStep = angle / segments;

        for (int i = 1; i <= segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * radius;
            vertices[i] = new Vector3(x, 0.01f, z);
            currentAngle += angleStep;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }
        
    public void MonsterMove()
    {
        if(allMonsterList.Count == 0) return;
        foreach (Monster monster in allMonsterList)
        {
            if (monster.IsBeingPushedBack || monster.IsUsingSkill) continue;
            
            Vector3 moveDir = GameWorld.Instance.PlayerManager.player.transform.position - monster.transform.position;
            float moveDist = moveDir.magnitude;
            moveDir.Normalize();
            
            if (moveDist <= 0.7f) continue;
            
            float rotAngle = Vector3.Angle(monster.transform.forward, moveDir);
            float rotDir = Vector3.Dot(monster.transform.right, moveDir) < 0.0f ? -1.0f : 1.0f;
            
            float rotAmount = 720.0f * Time.deltaTime;
            if (rotAngle < rotAmount) rotAmount = rotAngle;
            monster.transform.Rotate(Vector3.up * (rotDir * rotAmount));
            
            float moveAmount = monster.monsterData.moveSpeed * Time.deltaTime;
            if (moveDist - 0.7f < moveAmount) moveAmount = moveDist - 0.7f;
            monster.transform.Translate(moveDir * moveAmount, Space.World);
        }
    }
    
    public void MonsterSpawn(int index)
    {
        bool getMonster = false;
        
        float radius = Random.Range(10.0f, 20.0f);
        float angle = Random.Range(0.0f, 360.0f);
        float x = radius * Mathf.Sin(angle);
        float z = radius * Mathf.Cos(angle);
        
        Vector3 randomVector = new Vector3(x, 0.0f, z);
        Vector3 randomPosition = GameWorld.Instance.PlayerManager.player.transform.position + randomVector;
        
        randomPosition.x = Mathf.Clamp(randomPosition.x, -120, 120);
        randomPosition.z = Mathf.Clamp(randomPosition.z, -120, 120);
        
        foreach (GameObject obj in _poolMonsterList[index])
        {
            if (!obj.activeSelf)
            {
                getMonster = true;
                obj.SetActive(true);
                Monster monster = obj.GetComponent<Monster>();
                monster.transform.position = randomPosition;
                monster.transform.rotation = Quaternion.identity;
                monster.monsterType = (MonsterType) index;
                monster.monsterData = GameWorld.Instance.DataManager.monsterDic[monster.monsterType];
                monster.phase = GameWorld.Instance.RoundManager.phase;
                monster.MonsterInit();
                allMonsterList.Add(monster);
                
                GameWorld.Instance.UIManager.CreateMonsterHpBar(monster);
                break;
            }
        }
        
        if (!getMonster)
        {
            GameObject obj = Instantiate(monsterPrefabArray[index], randomPosition, Quaternion.identity, monsterSpawnParent);
            _poolMonsterList[index].Add(obj);
            Monster newMonster = obj.GetComponent<Monster>();
            newMonster.monsterType = (MonsterType) index;
            newMonster.monsterData = GameWorld.Instance.DataManager.monsterDic[newMonster.monsterType];
            newMonster.phase = GameWorld.Instance.RoundManager.phase;
            newMonster.MonsterInit();
            allMonsterList.Add(newMonster);
            _objectIdToMonster.Add(obj.GetInstanceID(), newMonster);
            if (newMonster.monsterType == MonsterType.Boss || newMonster.monsterType == MonsterType.EliteRush ||
                newMonster.monsterType == MonsterType.NormalLong)
            {
                newMonster.meshFilter = obj.GetComponentInChildren<MeshFilter>();
                newMonster.meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
                newMonster.meshRenderer.material = new Material(Shader.Find("Standard"));
                newMonster.meshRenderer.material.color = new Color(1, 0, 0, 0.5f);
            }
            
            GameWorld.Instance.UIManager.CreateMonsterHpBar(newMonster);
        }
    }
}
