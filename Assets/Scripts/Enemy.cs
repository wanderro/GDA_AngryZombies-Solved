using System;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public Action DiedEvent;
    
    [SerializeField]
    private float _fallingAngle = 30f;
    [SerializeField]
    private float _fallingVelocity = 0.7f;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField] 
    private AudioClip _deathSound;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GlobalConstants.SKULL_TAG))
        {
            Die();
            return;
        }

        if (IsFellDown())
        {
            Die();
            return;
        }

        var collisionRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        if (IsHit(collisionRigidbody))
        {
            Die();
        }
    }

    private bool IsHit(Rigidbody2D collisionRigidbody)
    {
        return collisionRigidbody != null && collisionRigidbody.velocity.magnitude >= _fallingVelocity;
    }

    private bool IsFellDown()
    {
        var rotation = _rigidbody.rotation;
        return rotation <= -_fallingAngle || rotation >= _fallingAngle;
    }

    private void Die()
    {
        DiedEvent?.Invoke();
        PlayDeathSound();
        CreateExplosion();
        Destroy(gameObject);
    }
    
    public void PlayDeathSound()
    {
        AudioSource.PlayClipAtPoint(_deathSound, transform.position);
    }
    
    public void CreateExplosion()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    }
}