using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 moveDirection;
    public float moveDistance;
    public List<Monster> hitMonsterList;
    
    public BulletData bulletData;
    public BulletType bulletType; 

    public void BulletInit()
    {
        hitMonsterList.Clear();
        moveDistance = 0.0f;
    }
    
    void Awake()
    {
        hitMonsterList = new List<Monster>();
    }
}
