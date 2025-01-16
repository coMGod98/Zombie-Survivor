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
    }
    
    public void GameOver()
    {
        Time.timeScale = 0.0f;
        _uiManager.ShowLosePanel();
    }
    
    public void GameClear()
    {
        Time.timeScale = 0.0f;
        _uiManager.ShowWinPanel();
    }
    
    private void OnEnable()
    {
        Time.timeScale = 1.0f;
    }
}
