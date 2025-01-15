using UnityEngine;

public class GameWorld : Singleton<GameWorld>
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private MonsterManager _monsterManager;
    [SerializeField] private BulletManager _bulletManager;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private FXManager _fxManager;
    [SerializeField] private ItemManager _itemManager;
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private RoundManager _roundManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private SoundManager _soundManager;
    
    public PlayerManager PlayerManager => _playerManager;
    public MonsterManager MonsterManager => _monsterManager;  
    public BulletManager BulletManager => _bulletManager;
    public WeaponManager WeaponManager => _weaponManager;
    public FXManager FXManager => _fxManager;
    public ItemManager ItemManager => _itemManager;
    public DataManager DataManager => _dataManager;
    public RoundManager RoundManager => _roundManager;
    public UIManager UIManager => _uiManager;
    public SoundManager SoundManager => _soundManager;

    private void Awake()
    {
        _dataManager.Init();
    }

    void Update()
    {
        _playerManager.PlayerAI();
        _playerManager.PlayerMove();
        _playerManager.PlayerAttack();
        _monsterManager.MonsterMove();
        _monsterManager.MonsterAI();
        _bulletManager.BulletMove();
        _bulletManager.BulletDetectMonster();
        _itemManager.ItemUpdate();
        
        if(Input.GetKeyDown(KeyCode.F9))
        {
            _monsterManager.MonsterSpawn(0);
        }
        if(Input.GetKeyDown(KeyCode.F8))
        {
            _monsterManager.MonsterSpawn(1);
        }
        if(Input.GetKeyDown(KeyCode.F7))
        {
            _monsterManager.MonsterSpawn(2);
        }
        if(Input.GetKeyDown(KeyCode.F6))
        {
            _monsterManager.MonsterSpawn(3);
        }
        if(Input.GetKeyDown(KeyCode.F5))
        {
            _monsterManager.MonsterSpawn(4);
        }
        if(Input.GetKeyDown(KeyCode.F4))
        {
            _monsterManager.MonsterSpawn(5);
        }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            _monsterManager.MonsterSpawn(6);
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            _monsterManager.MonsterSpawn(7);
        }
    }
}
