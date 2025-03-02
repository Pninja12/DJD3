using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _gravityAcceleration;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _maxBackwardSpeed;    
    [SerializeField] private float _maxStrafeSpeed;
    [SerializeField] private float _jumpSpeed;

    private CharacterController _controller;
    private Vector3 _velocityHor;
    private Vector3 _velocityVer;
    private Vector3 _motion;
    private bool    _jump;

    void Start()
    {
        _controller     = GetComponent<CharacterController>();
        _velocityHor    = Vector3.zero;
        _velocityVer    = Vector3.zero;
        _motion         = Vector3.zero;
        _jump           = false;

        HideCursor();
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateRotation();
        CheckForJump();
    }

    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X");

        transform.Rotate(0f, rotation, 0f);
    }

    private void CheckForJump()
    {
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            _jump = true;
    }

    void FixedUpdate()
    {
        UpdateVelocityHor();
        UpdateVelocityVer();
        UpdatePosition();
    }

    private void UpdateVelocityHor()
    {
        float forwardAxis   = Input.GetAxis("Forward");
        float strafeAxis    = Input.GetAxis("Strafe");

        _velocityHor.x = strafeAxis * _maxStrafeSpeed;

        if (forwardAxis > 0f)
        {
            _velocityHor.z = forwardAxis * _maxForwardSpeed;

            if (_velocityHor.magnitude > _maxForwardSpeed)
                _velocityHor = _velocityHor.normalized * _maxForwardSpeed;
        }
        else if (forwardAxis < 0f)
        {
            _velocityHor.z = forwardAxis * _maxBackwardSpeed;

            if (_velocityHor.magnitude > _maxBackwardSpeed)
                _velocityHor = _velocityHor.normalized * _maxBackwardSpeed;
        }
        else
            _velocityHor.z = 0f;
    }

    private void UpdateVelocityVer()
    {
        if (_jump)
        {
            _velocityVer.y = _jumpSpeed;
            _jump = false;
        }
        else if (_controller.isGrounded)
            _velocityVer.y = -0.1f;
        else if (_velocityVer.y > -_maxFallSpeed)
        {
            _velocityVer.y += _gravityAcceleration * Time.fixedDeltaTime;
            _velocityVer.y = Mathf.Max(_velocityVer.y, -_maxFallSpeed);
        }
    }

    private void UpdatePosition()
    {
        _motion = (_velocityHor + _velocityVer) * Time.fixedDeltaTime;
        _motion = transform.TransformVector(_motion);

        _controller.Move(_motion);
    }
}
