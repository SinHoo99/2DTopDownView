using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ���׷��̵� �ɼ��� �ǹ������� �����ϵ��� Enum�� ��������!
public enum UpgradeOption
{
    MaxHealth,
    AttackPower,
    Speed,
    Knockback,
    AttackDelay,
    NumberOfProjectiles,
    COUNT // COUNT�� ���� ���̴� enum�� �ƴ϶� �� ���� ����ִ����� ���� ����
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterStat defaultStat;
    [SerializeField] private CharacterStat rangedStat;


    public static GameManager Instance;
    [SerializeField] private string playerTag;

    public ObjectPool Objectpool { get; private set; }   
    public Transform Player {  get; private set; }
    public ParticleSystem EffectParticle;

    private HealthSystem playerHealthSystem;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Slider hpGaugeSlider;
    [SerializeField] private GameObject gameOverUI;


    [SerializeField] private int currentWaveIndex = 0;
    private int currentSpawnCount = 0;
    private int waveSpawnCount = 0;
    private int waveSpawnPosCount = 0;

    public float spawnInterval = .5f;
    public List<GameObject> enemyPrefebs = new List<GameObject>();

    [SerializeField] private Transform spawnPositionsRoot;
    private List<Transform> spawnPositions = new List<Transform>();


    [SerializeField] private List<GameObject> rewards = new List<GameObject>();
    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        Instance = this;
        Player = GameObject.FindGameObjectWithTag(playerTag).transform;
        Objectpool = GetComponent<ObjectPool>();
        EffectParticle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>();

        playerHealthSystem = Player.GetComponent<HealthSystem>();
        playerHealthSystem.OnDamage += UpdateHealthUI;
        playerHealthSystem.OnHeal += UpdateHealthUI;
        playerHealthSystem.OnDeath += GameOver;

        for (int i = 0; i < spawnPositionsRoot.childCount; i++)
        {
            spawnPositions.Add(spawnPositionsRoot.GetChild(i));
        }
    }

    private void Start()
    {

        StartCoroutine(StartNextWave());
    }
    IEnumerator StartNextWave()
    {
        while (true)
        {
            if (currentSpawnCount == 0)
            {
                UpdateWaveUI();
                // new WaitForSeconds�� GC�� ���ϰ� �ϱ� ���� ĳ���ϱ⵵ �մϴ�.
                yield return new WaitForSeconds(2f);

                ProcessWaveConditions();

                // yield return Coroutine���� ���� �ڷ�ƾ�� ���� ������ ��ٸ� �� �־��.
                // ��ø �ڷ�ƾ(Nested Coroutine)�̶�� �մϴ�.
                yield return StartCoroutine(SpawnEnemiesInWave());

                currentWaveIndex++;
            }

            // yield return null�� 1������ �ڶ�� �ǹ̿���!
            yield return null;
        }
    }

    void ProcessWaveConditions()
    {
        // % �� ������ ��������?
        // ������ ���� ���� ���ǹ��� �־, �ֱ⼺�� �ִ� ��� Ȱ���ϱ⵵ �ؿ�.

        // 20 ������������ �̺�Ʈ�� �߻��ؿ�.
        if (currentWaveIndex % 20 == 0)
        {
            RandomUpgrade();
        }

        if (currentWaveIndex % 10 == 0)
        {
            IncreaseSpawnPositions();
        }

        if (currentWaveIndex % 5 == 0)
        {
            CreateReward();
        }

        if (currentWaveIndex % 3 == 0)
        {
            IncreaseWaveSpawnCount();
        }
    }
    IEnumerator SpawnEnemiesInWave()
    {
        for (int i = 0; i < waveSpawnPosCount; i++)
        {
            int posIdx = Random.Range(0, spawnPositions.Count);
            for (int j = 0; j < waveSpawnCount; j++)
            {
                SpawnEnemyAtPosition(posIdx);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
    void SpawnEnemyAtPosition(int posIdx)
    {
        int prefabIdx = Random.Range(0, enemyPrefebs.Count); 
        GameObject enemy = Instantiate(enemyPrefebs[prefabIdx], spawnPositions[posIdx].position, Quaternion.identity);
        enemy.GetComponent<CharacterStatHandler>().AddStatModifier(defaultStat);
        enemy.GetComponent<CharacterStatHandler>().AddStatModifier(rangedStat);

        // ������ ���� OnEnemyDeath�� ����ؿ�.
        enemy.GetComponent<HealthSystem>().OnDeath += OnEnemyDeath;
        currentSpawnCount++;
    }

    void IncreaseSpawnPositions()
    {
        // ���׿����� ����Ͻ���? (���� ? ������ ���� �� : ������ ������ ��)ó�� ������ �ۼ��ſ�!
        waveSpawnPosCount = waveSpawnPosCount + 1 > spawnPositions.Count ? waveSpawnPosCount : waveSpawnPosCount + 1;
        waveSpawnCount = 0;
    }

    void IncreaseWaveSpawnCount()
    {
        waveSpawnCount ++;
    }

    private void UpgradeStatInit()
    {
        defaultStat.statsChangeType = StatsChangeType.Add;
        defaultStat.attackSO = Instantiate(defaultStat.attackSO);

        rangedStat.statsChangeType = StatsChangeType.Add;
        rangedStat.attackSO = Instantiate(rangedStat.attackSO);
    }

    private void RandomUpgrade()
    {
        UpgradeOption option = (UpgradeOption)Random.Range(0, (int)UpgradeOption.COUNT);
        switch (option)
        {
            case UpgradeOption.MaxHealth:
                defaultStat.maxHealth += 2;
                break;

            case UpgradeOption.AttackPower:
                defaultStat.attackSO.power += 1;
                break;

            case UpgradeOption.Speed:
                defaultStat.speed += 0.1f;
                break;

            case UpgradeOption.Knockback:
                defaultStat.attackSO.isOnKnockback = true;
                defaultStat.attackSO.knockbackPower += 1;
                defaultStat.attackSO.knockbackTime = 0.1f;
                break;

            case UpgradeOption.AttackDelay:
                defaultStat.attackSO.delay -= 0.05f;
                break;

            case UpgradeOption.NumberOfProjectiles:
                RangedAttackSO rangedAttackData = rangedStat.attackSO as RangedAttackSO;
                if (rangedAttackData != null) rangedAttackData.numberofProjectilesPerShot += 1;
                break;

            default:
                break;
        }
    }

    private void CreateReward()
    {
        // 5�ܰ踶�� ������ ����
        int selectedRewardIndex = Random.Range(0, rewards.Count);
        int randomPositionIndex = Random.Range(0, spawnPositions.Count);

        GameObject obj = rewards[selectedRewardIndex];
        Instantiate(obj, spawnPositions[randomPositionIndex].position, Quaternion.identity);
    }

    private void OnEnemyDeath()
    {
        currentSpawnCount--;
    }

    private void UpdateHealthUI()
    {
        hpGaugeSlider.value = playerHealthSystem.CurrentHealth / playerHealthSystem.MaxHealth;
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    private void UpdateWaveUI()
    {
        waveText.text = $"{currentWaveIndex + 1}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
