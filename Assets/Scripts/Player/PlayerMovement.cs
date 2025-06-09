using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float _gravityAcceleration;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _maxForwardSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _maxBackwardSpeed;
    [SerializeField] private float _maxStrafeSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _rotationalSpeed = 2;
    [SerializeField] private float _crouchHeight;
    [SerializeField] private float _defaultHeight;
    [SerializeField] private KeyCode _crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;

    [Header("Camera Settings")]
    [SerializeField] private Transform _camera;
    [SerializeField] private CameraControl _scriptCamera;

    [Header("UI/Life Settings")]
    [SerializeField] private UIManager ui;
    [SerializeField] private Gun _gun;
    [SerializeField] private byte _ammoToReceive = 3;
    [SerializeField] private byte _life = 3;
    [SerializeField] private float damageCooldown = 5.0f;

    [Header("Animation Settings")]
    [SerializeField] private AnimationsPlay anim;
    

    private CharacterController _controller;
    private Vector3 _velocityHor;
    private Vector3 _originalCenter;
    private Vector3 _velocityVer;
    private Vector3 _motion;
    private bool _jump;
    private bool _cameraLock;
    private bool _arrivedAtCamera;
    private float _rotateTo = 0;
    private float _rotateWhereWeAre;
    private bool _isCrouching = false;
    private float lastDamage = -Mathf.Infinity;

    //Add pelo carvalho
    public Image hpBar;
    //
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _velocityHor = Vector3.zero;
        _velocityVer = Vector3.zero;
        _motion = Vector3.zero;
        _jump = false;
        _cameraLock = false;
        _arrivedAtCamera = false;
        
        HideCursor();
        //Add pelo carvalho
        anim = gameObject.GetComponent<AnimationsPlay>();
        
        ChangeUILife();
        //

        _defaultHeight = _controller.height;
        _originalCenter = _controller.center;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        ChangeUILife();
      
        if (!ui.GetPause())
        {
            CheckForJump();
            UpdateRotation();

            if (Input.GetKeyDown(_crouchKey))
            {
                // Start crouching
                StartCrouch();
            }
            else if (Input.GetKeyUp(_crouchKey))
            {
                // Stop crouching
                StopCrouch();
            }
            if (Input.GetKeyDown(_sprintKey))
            {
                // change velocity
                _maxForwardSpeed += _sprintSpeed;

                //Add pelo carvalho    
                anim.Run();
                //
            }
            else if (Input.GetKeyUp(_sprintKey))
            {
                // change velocity
                _maxForwardSpeed -= _sprintSpeed;
                //Add pelo carvalho    
                anim.StopRun();
                //
            }
        }

        Cheats();

        if (_life == 0)
        {
            ui.GetComponent<UIManager>().DeadPanel();
            anim.Dead();
            //Scene _currentScene = SceneManager.GetActiveScene();
            //SceneManager.LoadScene(_currentScene.name);
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
        }


    }

    private void CheckForJump()
    {
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _jump = true;
            anim.Jump();
        } 
        //Add pelo carvalho
        else
        {
            anim.StopJump();
        }    
        //
    }

    private void StartCrouch()
    {
        if (!_isCrouching)
        {
            _controller.height = _crouchHeight;

            float _centerOffset = (_defaultHeight - _crouchHeight) / 2f;
            _controller.center = new Vector3(0, _originalCenter.y - _centerOffset, 0);

            _isCrouching = true;
            //Add pelo carvalho
            anim.Crouch();
            //
            
        }
    }

    private void StopCrouch()
    {
        if (_isCrouching)
        {
            _controller.height = _defaultHeight;
            _controller.center = _originalCenter;


            _isCrouching = false;
            //Add pelo carvalho
            anim.StopCrouch();
            //
           
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
        float forwardAxis = Input.GetAxis("Forward");
        float strafeAxis = Input.GetAxis("Strafe");

        _velocityHor.x = strafeAxis * _maxStrafeSpeed;

        if (forwardAxis > 0f)
        {
            //Add pelo carvalho
            anim.Walk();
            //
            _velocityHor.z = forwardAxis * _maxForwardSpeed;

            if (_velocityHor.magnitude > _maxForwardSpeed)
                _velocityHor = _velocityHor.normalized * _maxForwardSpeed;

        }
        else if (forwardAxis < 0f)
        {
            //Add pelo carvalho
            anim.Walk();
            //
            _velocityHor.z = forwardAxis * _maxBackwardSpeed;

            if (_velocityHor.magnitude > _maxBackwardSpeed)
                _velocityHor = _velocityHor.normalized * _maxBackwardSpeed;

        }
        //Add pelo Carvalho
        else if (_velocityHor.magnitude == 0)
        {
            anim.StopWalk();
        }
        //

        else
        {
            _velocityHor.z = 0f;
            //Add pelo carvalho
            anim.StopWalk();
            // 
        }
            
            
    }

    private void UpdateVelocityVer()
    {
        if (_jump)
        {
            _velocityVer.y = _jumpSpeed;
            _jump = false;
        }
        else if (_controller.isGrounded)
            _velocityVer.y = -1f;
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

    private void OnTriggerStay(Collider collided)
    {
        if (collided.CompareTag("TakeDown") && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Entered enemy's takedown zone!");

            // Get a reference to the enemy script (on parent or root of the trigger)
            PatrolAI enemy = collided.GetComponentInParent<PatrolAI>(); // or .GetComponent<Enemy>() if same GameObject
            if (enemy == null)
            {
                Transform parent = collided.transform.parent;
                foreach (Transform child in parent)
                {
                    Debug.Log("Child: " + child.name);

                    // Example: Find a child with a specific component
                    if (child.GetComponent<PatrolAI>() != null)
                    {
                        enemy = child.GetComponentInParent<PatrolAI>();
                    }
                }
            }

            if (enemy != null && enemy.GetState() != EnemyState.FollowingPlayer)
            {
                enemy.Death(10);
            }
        }

        if (collided.gameObject.layer == LayerMask.NameToLayer("Enemy") && _life > 0 && Time.time - lastDamage >= damageCooldown)
        {
            _life--;
            lastDamage = Time.time;
            
        }

        if (collided.gameObject.layer == LayerMask.NameToLayer("Loot") && Input.GetKeyDown(KeyCode.F))
        {
            print("Received ammo!");
            _gun.AddAmo(_ammoToReceive);
            //Add pelo Carvalho
            anim.Heal();
            //
        }
    }

    void Cheats()
    {
        if (Input.GetKeyDown(KeyCode.B))
            _gun.AddAmo(20);
            
    }
    //Add pelo carvalho
    public void ChangeUILife()
    {
        if (_life == 3)
        {
            hpBar.fillAmount = 1f;
        }
        if (_life == 2)
        {
            hpBar.fillAmount = 0.66f;
        }
        if (_life == 1)
        {
            hpBar.fillAmount = 0.33f;
        }
        if (_life == 0)
        {
            hpBar.fillAmount = 0f;
            anim.Dead();
        }

    }
    //
}