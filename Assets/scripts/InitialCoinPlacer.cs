using UnityEngine;

public class InitialCoinPlacer : MonoBehaviour
{
    [SerializeField] private Rigidbody coinPrefab;
    [SerializeField] private Transform platformArea;
    [SerializeField] private int initialCoinCount = 5;
    [SerializeField] private float spawnHeightOffset = 0.15f;
    [SerializeField] private float horizontalMargin = 0.5f;

    private void Start()
    {
        PlaceInitialCoins();
    }

    /// <summary>
    /// 初期状態のコインをPlatform上にランダムに配置する
    /// </summary>
    private void PlaceInitialCoins()
    {
        if (coinPrefab == null || platformArea == null)
        {
            Debug.LogError("InitialCoinPlacerの参照が不足しています。Inspectorの設定を確認してください。", this);
            return;
        }

        Collider platformCollider = platformArea.GetComponent<Collider>();
        if (platformCollider == null)
        {
            Debug.LogError("platformAreaにColliderがありません。", this);
            return;
        }

        Bounds bounds = platformCollider.bounds;
        float minX = bounds.min.x + horizontalMargin;
        float maxX = bounds.max.x - horizontalMargin;
        float minZ = bounds.min.z + horizontalMargin;
        float maxZ = bounds.max.z - horizontalMargin;
        float spawnY = bounds.max.y + spawnHeightOffset;

        for (int i = 0; i < initialCoinCount; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPos = new Vector3(randomX, spawnY, randomZ);

            Rigidbody newCoin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            ConfigureCoinRigidbody(newCoin);
        }
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
}
