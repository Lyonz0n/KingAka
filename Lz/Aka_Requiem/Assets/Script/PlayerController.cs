using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] private float speed = 5f;
    [Header("Sword Settings")]
    [SerializeField] private GameObject _swordPrefab;
    [SerializeField] private Transform _throwOffset;
    public Rigidbody2D rb;
    // MyPlayerControls is the C# class that Unity generated.
    // It encapsulates the data from the .inputactions asset we created
    // and automatically looks up all the maps and actions for us.
    PlayerControls controls;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    private float horizontalInput;
    private Vector3 initialPosition;
    private Vector3 swordPosition;
    private bool _Throwing;
    private bool _startThrowing;
    private float throwStartTime;
    private bool _swordCount;
    private bool _swordrecall;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        horizontalInput = movementInput.x;

        if (horizontalInput != 0)
        {
            FlipSprite(horizontalInput);
        }
        if (_startThrowing)
        {


            if (!_swordCount)
            {
                //charger

            }
            else
            {
                _startThrowing = false;
            }
        }

        if (_Throwing)
        {
            float throwDuration = Time.time - throwStartTime;
            float throwForce = CalculateThrowForce(throwDuration);
            if (throwForce > 0f && !_swordCount)
            {
                FireBullet();
                _swordCount = true;
            }
            else
            {
                _Throwing = false;
            }
        }
        if (_swordrecall)
        {
            RecallProjectile();

        }
    }
    private void FixedUpdate()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput, movementInput, ref movementInputSmoothVelocity, 0.1f);
        rb.velocity = smoothedMovementInput * speed;
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        // 'Use' code here.
        if (context.started)
        {
            throwStartTime = Time.time;
            _startThrowing = true;

        }
        if (context.performed)
        {

        }
        if (context.canceled)
        {
            _Throwing = true;
        }
    }
    private void FireBullet()
    {
        float throwDuration = Time.time - throwStartTime;
        float throwForce = CalculateThrowForce(throwDuration);
        GameObject sword = Instantiate(_swordPrefab, _throwOffset.position, Quaternion.identity);
        Rigidbody2D rigidbody = sword.GetComponent<Rigidbody2D>();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Obtenir la direction de lancement à partir de la position du joueur et de la position de la souris
        Vector2 throwDirection = (mousePosition - transform.position).normalized;
        rigidbody.velocity = throwDirection * throwForce;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
    public void OnRecall(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _swordrecall = true;
        }
    }
    public void RecallProjectile()
    {
        initialPosition = transform.position;
        // Rappeler le projectile vers la position initiale du joueur
        Rigidbody2D rigidbody = _swordPrefab.GetComponent<Rigidbody2D>();
        swordPosition = _swordPrefab.transform.position;
        Vector2 directionToPlayer = (initialPosition - swordPosition).normalized;
        rigidbody.velocity = directionToPlayer * 5f;

    }
    private float CalculateThrowForce(float throwTime)
    {
        float maxThrowForce = 10f; // Force maximale de lancer
        float throwTimeScale = 0.5f; // Échelle de temps pour ajuster la vitesse de la fonction exponentielle
        float throwForce = maxThrowForce * (1 - Mathf.Exp(-(Time.time - throwTime) * throwTimeScale));
        return throwForce;
    }
    private void FlipSprite(float horizontalMovement)
    {
        if (horizontalMovement < 0)
        {
            transform.localScale = new Vector3(5, 5, 1);
        }
        else if (horizontalMovement > 0)
        {
            transform.localScale = new Vector3(-5, 5, 1);
        }
    }
}
