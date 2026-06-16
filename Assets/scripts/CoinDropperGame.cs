using UnityEngine;
using UnityEngine.UI;
public class CoinDropperGame : MonoBehaviour
{
    [SerializeField] private int initialCoins = 100;
    [SerializeField] private float platformSpeed = 2f;
    [SerializeField] private float platformRange = 2f;
    [SerializeField] private Transform platform; 
    [SerializeField] private Rigidbody coinPrefab; 
    [SerializeField] private Transform spawnPointTransform;
    [SerializeField] private Text coinCountText; 
    [SerializeField] private Text gameOverText; 
    [SerializeField] private float catchZoneMinZ = -1f;
    [SerializeField] private float catchZoneMaxZ = 1f;

    private int coinCount;
    private bool isGameOver = false;
    private Vector3 platformInitialPos; 
    void Start()
    {
        coinCount = initialCoins;
        platformInitialPos = platform.position;
        gameOverText.text = "";
        UpdateCoinDisplay();       
        CoinBehavior.OnCoinCollected += CatchCoin;

    }
    void Update()
    {
        if (isGameOver) return;
        MovePlatform();
        HandleCoinInput();
    }
    private void MovePlatform()
    {
        float newX = platformInitialPos.x + Mathf.Sin(Time.time * platformSpeed) * platformRange;
        platform.position = new Vector3(newX, platformInitialPos.y, platformInitialPos.z);
    }

    private void HandleCoinInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coinCount > 0)
            {
                SpawnCoin();
                coinCount--; 
                UpdateCoinDisplay();
            }
            else
            {
                GameOver();
            }
        }
    }

    private void SpawnCoin()
    {
        Rigidbody newCoin = Instantiate(coinPrefab, spawnPointTransform.position, Quaternion.identity);
        newCoin.velocity = Vector3.zero;
    }

    private bool IsSpawnPointReady()
    {
        return spawnPointTransform != null;
    }

    private void UpdateCoinDisplay()
    {
        coinCountText.text = "所持コイン: " + coinCount;
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverText.text = "ゲームオーバー";
        gameOverText.color = Color.red;
    }

    public void CatchCoin()
    {
        coinCount++;
        UpdateCoinDisplay();
    }
}
