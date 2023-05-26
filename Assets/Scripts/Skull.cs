using UnityEngine;
using System.Collections.Generic;

public class Skull : MonoBehaviour
{
    [SerializeField] 
    private SpriteRenderer _touchAreaSprite;
    [SerializeField] 
    private int _trajectoryPointsCount = 30;
    [SerializeField] 
    private Transform _dotsRoot;
    [SerializeField] 
    private GameObject _trajectoryPointPrefab;
    [SerializeField] 
    private float _forceMultiplier = 8;

    [SerializeField] 
    private LineRenderer _catapultLineFront;
    [SerializeField] 
    private LineRenderer _catapultLineBack;

    [SerializeField]
    private AudioClip _throwSkull;
    [SerializeField]
    private AudioClip _rubber;

    private List<SpriteRenderer> _trajectoryPoints;
    private Rigidbody2D _rigidbody;
    
    private float _pullRadius;
    private Vector3 _center;
    private bool _isBallThrown;
    private bool _isPressed;
    private bool _isResetting;

    private Vector3 _originalPositionOfLineFront;
    private Vector3 _originalPositionOfLineBack;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _trajectoryPoints = new List<SpriteRenderer>();

        for (var i = 0; i < _trajectoryPointsCount; i++)
        {
            var dot = Instantiate(_trajectoryPointPrefab, _dotsRoot, true);
            var dotSprite = dot.GetComponent<SpriteRenderer>();
            dotSprite.enabled = false;
            _trajectoryPoints.Insert(i, dotSprite);
        }

        // Вычисляем максимальную точку оттягивания резинки
        // (по сути радиус окружности вокруг рогатки с небольшой погрешностью)
        _pullRadius = _touchAreaSprite.bounds.size.x / 2.0f - 0.3f;
    }

    private void Start()
    {
        _center = transform.position;
        ResetPosition();
        SetupLineRenderer();
    }

    private void Update()
    {
        if (_isBallThrown)
        {
            if (_isResetting)
            {
                return;
            }
            
            if (_rigidbody.velocity.magnitude <= 0.1f)
            {
                _isResetting = true;
                ResetPosition();
                ResetLineRenderer();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckRangeRubber();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_isPressed)
            {
                _isPressed = false;

                if (!_isBallThrown)
                {
                    AudioSource.PlayClipAtPoint(_throwSkull, transform.position);
                    ThrowBall();
                }
            }
        }

        if (_isPressed)
        {
            var hookPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = hookPosition - _center;

            direction = Vector2.ClampMagnitude(direction, _pullRadius);
            transform.position = _center + direction;

            Vector3 velocity = GetForceFrom(transform.position, _center);
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
            SetTrajectoryPoints(transform.position, velocity);

            // Отрисовываем резинку
            UpdateLineRenderer();
        }
    }

    private void HideDots()
    {
        for (var i = 0; i < _trajectoryPointsCount; i++)
        {
            _trajectoryPoints[i].enabled = false;
        }
    }

    private void SetTrajectoryPoints(Vector3 startPosition, Vector3 velocity)
    {
        var angle = Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x);
        float time = 0;

        for (var i = 0; i < _trajectoryPointsCount; i++)
        {
            var x = velocity.magnitude * time * Mathf.Cos(angle * Mathf.Deg2Rad);
            var y = velocity.magnitude * time * Mathf.Sin(angle * Mathf.Deg2Rad) -
                     (Physics2D.gravity.magnitude * time * time / 2.0f);
            var position = new Vector3(startPosition.x + x, startPosition.y + y, 2);
            
            _trajectoryPoints[i].transform.position = position;
            _trajectoryPoints[i].enabled = true;
            time += 0.1f;
        }
    }

    private void CheckRangeRubber()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Если резинка рогатки оттянута в пределах радиуса рогатки
        if (mousePosition.x <= (_center.x + _pullRadius) && mousePosition.x >= (_center.x - _pullRadius) &&
            mousePosition.y <= (_center.y + _pullRadius) && mousePosition.y >= (_center.y - _pullRadius))
        {
            _isPressed = true;
            AudioSource.PlayClipAtPoint(_rubber, transform.position);
        }
    }

    private void ThrowBall()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(GetForceFrom(transform.position, _center), ForceMode2D.Impulse);
        _isBallThrown = true;

        _catapultLineFront.enabled = false;
        _catapultLineBack.enabled = false;
        
        AudioSource.PlayClipAtPoint(_throwSkull, transform.position);
        HideDots();
    }

    private Vector2 GetForceFrom(Vector3 fromPosition, Vector3 toPosition)
    {
        return (new Vector2(toPosition.x, toPosition.y) - new Vector2(fromPosition.x, fromPosition.y)) * _forceMultiplier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверка выхождения черепа за пределы карты
        if (collision.gameObject.CompareTag(GlobalConstants.OUT_OF_MAP_TAG))
        {
            ResetPosition();
            ResetLineRenderer();
        }
    }

    private void ResetPosition()
    {
        transform.position = _center;
        _rigidbody.freezeRotation = true;
        transform.rotation = Quaternion.identity;

        _rigidbody.freezeRotation = false;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.isKinematic = true;

        _isPressed = _isBallThrown = _isResetting = false;
    }

    private void SetupLineRenderer()
    {
        _catapultLineFront.sortingLayerName = "Default";
        _catapultLineFront.sortingOrder = 5;

        _catapultLineBack.sortingLayerName = "Default";
        _catapultLineBack.sortingOrder = 2;

        _originalPositionOfLineFront = _catapultLineFront.GetPosition(0);
        _originalPositionOfLineBack = _catapultLineBack.GetPosition(0);
    }
    
    private void UpdateLineRenderer()
    {
        _catapultLineFront.SetPosition(1, transform.GetChild(transform.childCount - 1).position);
        _catapultLineBack.SetPosition(1, transform.GetChild(transform.childCount - 1).position);
    }
    
    private void ResetLineRenderer()
    {
        _catapultLineFront.SetPosition(1, _originalPositionOfLineFront);
        _catapultLineBack.SetPosition(1, _originalPositionOfLineBack);

        _catapultLineFront.enabled = true;
        _catapultLineBack.enabled = true;
    }
}