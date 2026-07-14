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

    private int coinCount;
    private bool isGameOver = false;
    private Vector3 platformInitialPos;
    private Rigidbody platformRigidbody;

    private void Start()
    {
        if (!HasRequiredReferences())
        {
            enabled = false;
            return;
        }

        ConfigurePlatformRigidbody();
        coinCount = initialCoins;
        platformInitialPos = platform.position;
        gameOverText.text = "";
        UpdateCoinDisplay();
        CoinBehavior.OnCoinCollected += CatchCoin;
    }

    private void Update()
    {
        if (isGameOver) return;
        HandleCoinInput();
    }

    private void FixedUpdate()
    {
        if (isGameOver || platform == null) return;
        MovePlatform();
    }

    private void ConfigurePlatformRigidbody()
    {
        platformRigidbody = platform.GetComponent<Rigidbody>();
        if (platformRigidbody == null)
        {
            platformRigidbody = platform.gameObject.AddComponent<Rigidbody>();
        }

        platformRigidbody.isKinematic = true;
        platformRigidbody.useGravity = false;
        platformRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        platformRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private void MovePlatform()
    {
        float newX = platformInitialPos.x + Mathf.Sin(Time.fixedTime * platformSpeed) * platformRange;
        Vector3 targetPosition = new Vector3(newX, platformInitialPos.y, platformInitialPos.z);

        if (platformRigidbody != null)
        {
            platformRigidbody.MovePosition(targetPosition);
        }
        else
        {
            platform.position = targetPosition;
        }
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
        ConfigureCoinRigidbody(newCoin);
    }

    private void ConfigureCoinRigidbody(Rigidbody coinRigidbody)
    {
        coinRigidbody.isKinematic = false;
        coinRigidbody.useGravity = true;
        coinRigidbody.velocity = Vector3.zero;
        coinRigidbody.angularVelocity = Vector3.zero;
        coinRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        coinRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Collider coinCollider = coinRigidbody.GetComponent<Collider>();
        if (coinCollider != null)
        {
            coinCollider.enabled = true;
        }
    }

    private bool HasRequiredReferences()
    {
        if (platform == null || coinPrefab == null || spawnPointTransform == null || coinCountText == null || gameOverText == null)
        {
            return false;
        }

        return true;
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

    private void OnDestroy()
    {
        CoinBehavior.OnCoinCollected -= CatchCoin;
    }
}