using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CoinBehavior : MonoBehaviour
{
    [SerializeField] private float catchZoneMinZ = -1f;
    [SerializeField] private float catchZoneMaxZ = 0f;
    [SerializeField] private float destroyYPosition = -10f;
    [SerializeField] private bool logWhenDestroyed = false;

    public static event Action OnCoinCollected;

    private bool isCaught = false;
    private int platformContactCount = 0;
    private Rigidbody coinRigidbody;
    private Collider coinCollider;

    private bool IsOnPlatform => platformContactCount > 0;

    private void Awake()
    {
        coinRigidbody = GetComponent<Rigidbody>();
        coinCollider = GetComponent<Collider>();
        ConfigureRigidbody();
    }

    private void FixedUpdate()
    {
        if (isCaught || coinRigidbody == null) return;

        CheckCatchZone();
        CheckDestroyBounds();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            platformContactCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            platformContactCount = Mathf.Max(0, platformContactCount - 1);
        }
    }

    private void ConfigureRigidbody()
    {
        coinRigidbody.isKinematic = false;
        coinRigidbody.useGravity = true;
        coinRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        coinRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (coinCollider != null)
        {
            coinCollider.enabled = true;
        }
    }

    private void CheckCatchZone()
    {
        if (IsOnPlatform) return;

        bool inZRange = transform.position.z >= catchZoneMinZ && transform.position.z <= catchZoneMaxZ;
        bool fallingFastEnough = coinRigidbody.velocity.y < -0.1f;

        if (inZRange && fallingFastEnough)
        {
            CatchCoin();
        }
    }

    private void CheckDestroyBounds()
    {
        if (transform.position.y >= destroyYPosition) return;

        if (logWhenDestroyed)
        {
            Debug.Log("コインが画面外に消えました。", this);
        }

        Destroy(gameObject);
    }

    private void CatchCoin()
    {
        isCaught = true;
        OnCoinCollected?.Invoke();
        Destroy(gameObject);
    }
}