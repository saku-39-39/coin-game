using UnityEngine;
using System;

public class CoinBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 platformWorldScale = new Vector3(2f, 0.5f, 2f);
    [SerializeField] private Vector3 compensatingScale = new Vector3(0.5f, 2.0f, 0.5f);
    [SerializeField] private float catchZoneMinZ = -1f;
    [SerializeField] private float catchZoneMaxZ = 0f;
    [SerializeField] private float destroyYPosition = -10f;

    public static event Action OnCoinCollected;
    private CoinDropperGame gameManager;
    private bool isCaught = false;
    private bool isOnPlatform = false;
    private Rigidbody coinRigidbody;
    private Collider coinCollider;  
    private Transform currentIntermediateTargetTransform = null;


    void Start()
    {
        gameManager = FindObjectOfType<CoinDropperGame>();
        coinRigidbody = GetComponent<Rigidbody>();
        coinCollider = GetComponent<Collider>();
    }


    void FixedUpdate()
    {
        coinRigidbody.isKinematic = false;
        if (coinRigidbody == null || isCaught) return;
        CheckCatchZone();
        if (transform.position.y < destroyYPosition)
        {
            Debug.Log("コインが画面外に消えました。");
            Destroy(gameObject);
        }

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Platform")) return;

        isOnPlatform = true;

        Transform intermediateParent = FindNamedChild(collision.transform, "IntermediateObject");

        if (intermediateParent == null)
        {
            Debug.LogError("Intermediate Objectが見つかりません。親子付けが失敗します。", this);
            return;
        }
        if (coinRigidbody != null)
        {
            coinRigidbody.velocity = Vector3.zero;
            coinRigidbody.angularVelocity = Vector3.zero;
            if (coinCollider != null) { coinCollider.enabled = false; }
            transform.SetParent(intermediateParent);
            currentIntermediateTargetTransform = intermediateParent;
            coinRigidbody.isKinematic = true;
        }
    }


    private void OnTriggerExit(Collider collision)
    {
        if (!collision.CompareTag("Platform")) return;
        isOnPlatform = false;
        transform.SetParent(null);

        if (coinRigidbody != null && coinCollider != null)
        {
            coinRigidbody.isKinematic = false;
            coinRigidbody.AddForce(Vector3.down * coinRigidbody.mass * 5f, ForceMode.VelocityChange);
        }
    }
    private void CheckCatchZone()
    {
        if (!isOnPlatform)
        {
            bool inZRange = transform.position.z >= catchZoneMinZ && transform.position.z <= catchZoneMaxZ;
            if (inZRange)
            {
                bool fallingFastEnough = coinRigidbody != null && coinRigidbody.velocity.y < -0.1f;

                if (fallingFastEnough)
                {
                    CatchCoin(); 
                }
            }
        }
    }
    private void CatchCoin()
    {
        isCaught = true;  
        if (gameManager != null)
        {
            OnCoinCollected?.Invoke();
        }
        Destroy(gameObject);  
    }

    private Transform FindNamedChild(Transform parent, string nameToFind)
    {
        foreach (Transform child in parent)
        {
            if (child.name == nameToFind)
            {
                return child;
            }
        }
        return null;
    }
}

