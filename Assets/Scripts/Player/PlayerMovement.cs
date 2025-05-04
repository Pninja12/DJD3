using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _gravityAcceleration;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _maxBackwardSpeed;    
    [SerializeField] private float _maxStrafeSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private Transform _camera;
    [SerializeField] private CameraControl _scriptCamera;
    [SerializeField] private float _rotationalSpeed = 2;
    [SerializeField] private float _crouchHeight;
    [SerializeField] private float _defaultHeight;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    private CharacterController _controller;
    private Vector3 _velocityHor;
    private Vector3 _velocityVer;
    private Vector3 _motion;
    private bool    _jump;
    private bool    _cameraLock;
    private bool    _arrivedAtCamera;
    private float _rotateTo = 0;
    private float   _rotateWhereWeAre;
    private bool _isCrouching = false;
    

    void Start()
    {
        _controller     = GetComponent<CharacterController>();
        _velocityHor    = Vector3.zero;
        _velocityVer    = Vector3.zero;
        _motion         = Vector3.zero;
        _jump           = false;
        _cameraLock     = false;
        _arrivedAtCamera = false;

        HideCursor();
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        CheckForJump();
        UpdateRotation();

        if (Input.GetKeyDown(crouchKey))
        {
            // Start crouching
            StartCrouch();
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            // Stop crouching
            StopCrouch();
        }
    }

    private void UpdateRotation()
    {
        //Se clicar no botao direito do rato:
        if (Input.GetButton("Camera"))
        {
            float rotation = Input.GetAxis("Mouse X");
            transform.Rotate(0f, rotation, 0f);
            //Lock para contar apenas a primeira vez
            if (!_cameraLock)
            {
                
                _arrivedAtCamera = false;
            }
            _rotateTo = _camera.eulerAngles.y;

            //Se a camera for diferente de onde o jogador est� a olhar
            if (transform.eulerAngles.y != _rotateTo && !_arrivedAtCamera)
            {

                //Vista 2D, rodamos a camera para 0, tamb�m rodando o jogador
                _rotateWhereWeAre = transform.eulerAngles.y - _rotateTo;

                //Estabilizar o n�mero caso se torne negativo
                if (_rotateWhereWeAre < 0)
                    _rotateWhereWeAre = 360 + _rotateWhereWeAre;

                if (_rotateWhereWeAre < 180)
                {
                    //Caso a velocidade ultrapasse o destino
                    if (transform.eulerAngles.y - _rotationalSpeed < _rotateTo && transform.eulerAngles.y != _rotateTo)
                    {
                        /* transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z);
                        _scriptCamera.DefiniteRotate(_rotateTo); */
                        /* transform.Rotate(0f, -(transform.eulerAngles.y - _rotateTo), 0f);
                        _scriptCamera.Rotate(transform.eulerAngles.y - _rotateTo); */

                        float conta = _rotateTo - transform.eulerAngles.y;
                        /* transform.Rotate(0f, -(transform.eulerAngles.y - _rotateTo), 0f);
                        _scriptCamera.Rotate(conta);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z); */
                        if (conta > _rotationalSpeed / 2)
                        {
                            transform.Rotate(0f, -(_rotationalSpeed / 2), 0f);
                            _scriptCamera.Rotate(_rotationalSpeed / 2);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z);
                            _scriptCamera.DefiniteRotate(_rotateTo);
                        }
                    }
                    else
                    {
                        transform.Rotate(0f, -_rotationalSpeed, 0f);
                        _scriptCamera.Rotate(_rotationalSpeed);
                    }
                }
                else
                {
                    //Caso a velocidade ultrapasse o destino
                    if (transform.eulerAngles.y + _rotationalSpeed > _rotateTo && transform.eulerAngles.y != _rotateTo)
                    {
                        /* transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z);
                        _scriptCamera.DefiniteRotate(_rotateTo); */
                        /* transform.Rotate(0f, transform.eulerAngles.y - _rotateTo, 0f);
                        _scriptCamera.Rotate(-(transform.eulerAngles.y - _rotateTo)); */

                        float conta = -(_rotateTo - transform.eulerAngles.y);
                        /* transform.Rotate(0f, transform.eulerAngles.y - _rotateTo, 0f);
                        _scriptCamera.Rotate(conta);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z); */

                        if (conta > _rotationalSpeed / 2)
                        {
                            transform.Rotate(0f, _rotationalSpeed / 2, 0f);
                            _scriptCamera.Rotate(-(_rotationalSpeed / 2));
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z);
                            _scriptCamera.DefiniteRotate(_rotateTo);
                        }
                    }

                    else
                    {
                        transform.Rotate(0f, _rotationalSpeed, 0f);
                        _scriptCamera.Rotate(-_rotationalSpeed);
                    }

                }

                if (transform.eulerAngles.y == _rotateTo)
                {
                    _arrivedAtCamera = true;
                }
            }

            _cameraLock = true;
        }
        else
        {
            _cameraLock = false;
        }
        if (!Input.GetButton("Camera") && (Input.GetButton("Strafe") || Input.GetButton("Forward")))
        {
            float rotation = Input.GetAxis("Mouse X");
            transform.Rotate(0f, rotation, 0f);
            //Lock para contar apenas a primeira vez
            _rotateTo = _camera.eulerAngles.y;
            _arrivedAtCamera = false;


            //Se a camera for diferente de onde o jogador est� a olhar
            if (transform.eulerAngles.y != _rotateTo && !_arrivedAtCamera)
            {

                //Vista 2D, rodamos a camera para 0, tamb�m rodando o jogador
                _rotateWhereWeAre = transform.eulerAngles.y - _rotateTo;

                //Estabilizar o n�mero caso se torne negativo
                if (_rotateWhereWeAre < 0)
                    _rotateWhereWeAre = 360 + _rotateWhereWeAre;

                
                if (_rotateWhereWeAre < 180)
                {
                    //Caso a velocidade ultrapasse o destino
                    if (transform.eulerAngles.y - _rotationalSpeed < _rotateTo && transform.eulerAngles.y != _rotateTo)
                    {
                        float conta = _rotateTo - transform.eulerAngles.y;
                        /* transform.Rotate(0f, -(transform.eulerAngles.y - _rotateTo), 0f);
                        _scriptCamera.Rotate(conta);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z); */
                        if (conta > _rotationalSpeed / 2)
                        {
                            transform.Rotate(0f, -(_rotationalSpeed / 2), 0f);
                            _scriptCamera.Rotate(_rotationalSpeed / 2);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z);
                            _scriptCamera.DefiniteRotate(_rotateTo);
                        }
                        
                    }
                    else
                    {
                        transform.Rotate(0f, -_rotationalSpeed, 0f);
                        _scriptCamera.Rotate(_rotationalSpeed);
                    }
                }
                else if (_rotateWhereWeAre > 180)
                {
                    //Caso a velocidade ultrapasse o destino
                    if (transform.eulerAngles.y + _rotationalSpeed > _rotateTo && transform.eulerAngles.y != _rotateTo)
                    {
                        float conta = -(_rotateTo - transform.eulerAngles.y);
                        /* transform.Rotate(0f, transform.eulerAngles.y - _rotateTo, 0f);
                        _scriptCamera.Rotate(conta);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z); */

                        if (conta > _rotationalSpeed / 2)
                        {
                            transform.Rotate(0f, _rotationalSpeed/2, 0f);
                            _scriptCamera.Rotate(-(_rotationalSpeed/2));
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateTo, transform.rotation.eulerAngles.z);
                            _scriptCamera.DefiniteRotate(_rotateTo);
                        }
                        
                    }

                    else
                    {
                        transform.Rotate(0f, _rotationalSpeed, 0f);
                        _scriptCamera.Rotate(-_rotationalSpeed);
                    }

                }

                if (transform.eulerAngles.y == _rotateTo)
                {
                    _arrivedAtCamera = true;
                }
            }
        }
            
            
    }

    private void CheckForJump()
    {
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            _jump = true;
    }

    private void StartCrouch()
    {
        if(!_isCrouching)
        {
            _controller.height = _crouchHeight;
            _isCrouching = true;
        }
    }

    private void StopCrouch()
    {
        if(_isCrouching)
        {
            _controller.height = _defaultHeight;
            _isCrouching = false;
        }
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
