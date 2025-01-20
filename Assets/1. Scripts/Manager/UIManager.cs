using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private Texture2D cursor;
    [SerializeField] private Texture2D crosshair;
    
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider armorSlider;
    [SerializeField] private Slider expSlider;
    
    [SerializeField] private TextMeshProUGUI elaspedTime;
    
    [SerializeField] private TextMeshProUGUI maxHp;
    [SerializeField] private TextMeshProUGUI curHp;
    [SerializeField] private TextMeshProUGUI maxArmor;
    [SerializeField] private TextMeshProUGUI curArmor;

    [SerializeField] private TextMeshProUGUI maxBulletCount;
    [SerializeField] private TextMeshProUGUI curBulletCount;
    
    [SerializeField] public Image reloadTimeImage;
    [SerializeField] private Image reloadTimeFillImg;
    
    [SerializeField] private GameObject monsterHpBarPrefab;
    private List<GameObject> _poolHpBars;
    private Dictionary<Monster, Slider> monsterHpBars;
    
    [SerializeField] private GameObject dmgTextPrefab;
    private List<GameObject> _poolDmgText;
    
    [SerializeField] private GameObject LevelUPPanel;
    [SerializeField] private GameObject upgradeBtnPrefab;
    [SerializeField] private GameObject upgradeBtnSpawnParent;
    private List<GameObject> _poolUpgradeBtn;
    [SerializeField] private List<Sprite> icons;
    
    [SerializeField] private GameObject ESCPanel;
    [SerializeField] private GameObject OptionPanel;
    [SerializeField] private GameObject VictoryPanel; 
    [SerializeField] private GameObject DefeatPanel;
    
    public GameObject uiPool;

    private void Awake()
    {
        _camera = Camera.main;
        _poolHpBars = new List<GameObject>();
        _poolDmgText = new List<GameObject>();
        _poolUpgradeBtn = new List<GameObject>();
        monsterHpBars = new Dictionary<Monster, Slider>();
    }
    
    private void Start()
    {
        Vector2 hotSpot = new Vector2(crosshair.width / 2.0f, crosshair.height / 2.0f);
        //Cursor.SetCursor(crosshair, hotSpot, CursorMode.Auto);
        
        reloadTimeImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateUI();
        ReloadUI();
        UpdateMonsterHpBar();
    }
    
    private void UpdateUI()
    {
        Player player = GameWorld.Instance.PlayerManager.player;
        hpSlider.value = player.curHp / player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]];
        armorSlider.value = player.curArmor / player.playerData.maxArmor[player.upgradeSelectionCounts[UpgradeType.MaxArmor]];
        expSlider.value = player.curExp / player.playerData.maxExp[player.level - 1];
        
        TimeSpan timeSpan = TimeSpan.FromSeconds(660 - GameWorld.Instance.RoundManager.elapsedTime);
        elaspedTime.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        
        maxHp.text = player.playerData.maxHp[player.upgradeSelectionCounts[UpgradeType.MaxHp]].ToString();
        curHp.text = player.curHp.ToString();
        maxArmor.text = player.playerData.maxArmor[player.upgradeSelectionCounts[UpgradeType.MaxArmor]].ToString();
        curArmor.text = player.curArmor.ToString();
        
        maxBulletCount.text = player.playerData.bulletMaxCount[player.upgradeSelectionCounts[UpgradeType.BulletMaxCount]].ToString();
        curBulletCount.text = player.bulletCurrentCount.ToString();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowEscapePanel();
        }
    }
    
    public void ShowEscapePanel()
    {
        if (OptionPanel.activeSelf)
        {
            OptionPanel.SetActive(false);
            ESCPanel.SetActive(true);
        }
        else
        {
            ESCPanel.SetActive(!ESCPanel.activeSelf);
            if (ESCPanel.activeSelf)
            {
                Time.timeScale = 0.0f;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else if (!VictoryPanel.activeSelf && !DefeatPanel.activeSelf)
            {
                Time.timeScale = 1.0f;
                Vector2 hotSpot = new Vector2(crosshair.width / 2.0f, crosshair.height / 2.0f);
                Cursor.SetCursor(crosshair, hotSpot, CursorMode.Auto);
            }
        }
    }
    
    public void ShowOptionPanel()
    {
        ESCPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        ESCPanel.SetActive(false);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ShowWinPanel()
    {
        VictoryPanel.SetActive(true);
    }
    
    public void ShowLosePanel()
    {
        DefeatPanel.SetActive(true);
    }

    public void ShowLevelUP(List<UpgradeType> upgradeOptions)
    {
        Time.timeScale = 0.0f;
        LevelUPPanel.SetActive(true);
        
        for (int i = 0; i < upgradeOptions.Count; i++)
        {
            GameObject btnUpgradeObj = null;
            foreach (GameObject obj in _poolUpgradeBtn)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    btnUpgradeObj = obj;
                    break;
                }
            }

            if (btnUpgradeObj == null)
            {
                btnUpgradeObj = Instantiate(upgradeBtnPrefab, upgradeBtnSpawnParent.transform);
                _poolUpgradeBtn.Add(btnUpgradeObj);
            }

            Button btn = btnUpgradeObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            UpgradeType upgradeType = upgradeOptions[i];
            btn.onClick.AddListener(() =>
            {
                LevelUPPanel.SetActive(false);
                Time.timeScale = 1.0f;
                foreach (GameObject obj in _poolUpgradeBtn)
                {
                    obj.SetActive(false);
                }
                GameWorld.Instance.PlayerManager.ApplyUpgrade(upgradeType);
                GameWorld.Instance.PlayerManager.OnLevelUpComplete();
            });
            LevelUPBtn levelUPBtn = btnUpgradeObj.GetComponent<LevelUPBtn>();
            levelUPBtn.upgradeData = GameWorld.Instance.DataManager.upgradeDic[upgradeType];
            levelUPBtn.icon.sprite = icons[levelUPBtn.upgradeData.iconIndex];
            levelUPBtn.title.text = levelUPBtn.upgradeData.title;
            levelUPBtn.description.text = levelUPBtn.upgradeData.description[GameWorld.Instance.PlayerManager.player.upgradeSelectionCounts[upgradeType]];
        }
    }

    private void ReloadUI()
    {
        Player player = GameWorld.Instance.PlayerManager.player;
        if (reloadTimeImage.gameObject.activeSelf)
        {
            Vector3 worldPos = GameWorld.Instance.PlayerManager.player.transform.position + new Vector3(0, 2.5f, 0);
            Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);
            reloadTimeImage.transform.position = screenPos;
            
            float reloadTime = player.playerData.bulletReloadTime[player.upgradeSelectionCounts[UpgradeType.BulletReloadTime]];;
            reloadTimeFillImg.fillAmount = Mathf.Lerp(1f, 0f,player.bulletReloadElapsedTime / reloadTime);
            
            if (player.bulletReloadElapsedTime >= reloadTime)
            {
                reloadTimeImage.gameObject.SetActive(false);
            }
        }
    }
    
    private void UpdateMonsterHpBar()
    {
        foreach (Monster monster in monsterHpBars.Keys)
        {
            if (monsterHpBars.TryGetValue(monster, out Slider slider))
            {
                float heightOffset = 2.5f;
                switch (monster.monsterType)
                {
                    case MonsterType.EliteShort:
                        heightOffset = 3.0f;
                        break;
                    case MonsterType.EliteRush:
                        heightOffset = 3.5f;
                        break;
                    case MonsterType.Boss:
                        heightOffset = 4.0f;
                        break;
                }
                
                Vector3 worldPos = monster.transform.position + new Vector3(0, heightOffset, 0);
                Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);
                slider.transform.position = screenPos;
                slider.value = monster.curHp / monster.monsterData.maxHp[monster.phase];
            }
        }
    }
    
    public void RemoveMonsterHpBar(Monster monster)
    {
        if (monsterHpBars.ContainsKey(monster))
        {
            monsterHpBars[monster].gameObject.SetActive(false);
            monsterHpBars.Remove(monster);
        }
    }
    
    public void CreateMonsterHpBar(Monster monster)
    {
        Slider hpSlider = null;
        foreach (GameObject obj in _poolHpBars)
        {
            if(!obj.activeSelf)
            {
                obj.SetActive(true);
                hpSlider = obj.GetComponent<Slider>();
                break;
            }
        }
        if (hpSlider == null)
        {
            GameObject obj = Instantiate(monsterHpBarPrefab, uiPool.transform);
            hpSlider = obj.GetComponent<Slider>();
            _poolHpBars.Add(obj);
        }   
        hpSlider.value = monster.curHp / monster.monsterData.maxHp[monster.phase];
        monsterHpBars.Add(monster, hpSlider);
    }

    public void ShowDamageText(Vector3 position, float damage)
    {
        GameObject dmgTextObj = null;
        foreach (GameObject obj in _poolDmgText)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                dmgTextObj = obj;
                break;
            }
        }
        if (dmgTextObj == null)
        {
            dmgTextObj = Instantiate(dmgTextPrefab, uiPool.transform);
            _poolDmgText.Add(dmgTextObj);
        }
        dmgTextObj.transform.position = _camera.WorldToScreenPoint(position);
        TextMeshProUGUI dmgText = dmgTextObj.GetComponent<TextMeshProUGUI>();
        dmgText.text = damage.ToString();

        StartCoroutine(FloatingText(dmgTextObj));
    }
    
    private IEnumerator FloatingText(GameObject dmgTextObj)
    {
        float dist = 0.0f;
        float moveSpeed = 50.0f;
        Vector3 moveDir = Vector3.up;
        while (dist < 50.0f)
        {
            dmgTextObj.transform.localPosition += moveDir * (moveSpeed * Time.deltaTime);
            dist += moveSpeed * Time.deltaTime;
            
            yield return null;
        }
        dmgTextObj.SetActive(false);
    }
}
