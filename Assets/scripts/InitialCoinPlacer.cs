using UnityEngine;

public class InitialCoinPlacer : MonoBehaviour
{
    [SerializeField] private Rigidbody coinPrefab;
    [SerializeField] private Transform platformArea;
    [SerializeField] private int initialCoinCount = 5;
    [SerializeField] private float minX = -2f;
    [SerializeField] private float maxX = 2f;
    [SerializeField] private float minZ = -2f;
    [SerializeField] private float maxZ = 2f;
    [SerializeField] private float platformY = 0f;

    private void Start()
    {
        PlaceInitialCoins();
    }

    /// <summary>
    /// 初期状態のコインをランダムに配置する
    /// </summary>
    private void PlaceInitialCoins()
    {
        Bounds bounds = platformArea.GetComponent<Collider>().bounds;

        for (int i = 0; i < initialCoinCount; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 spawnPos = new Vector3(randomX, platformY, randomZ);

            Rigidbody newCoin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            newCoin.velocity = Vector3.zero;
            newCoin.isKinematic = true;
        }
    }
}