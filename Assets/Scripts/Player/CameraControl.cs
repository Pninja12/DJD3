using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float      _resetRotationSpeed;
    [SerializeField] private float      _maxLookUpAngle;
    [SerializeField] private float      _maxLookDownAngle;
    [SerializeField] private float      _closeZoom;
    [SerializeField] private float      _farZoom;
    [SerializeField] private float      _aim;
    [SerializeField] private float      _zoomDeceleration;
    [SerializeField] private Transform  _deocclusionPivot;
    [SerializeField] private LayerMask  _deocclusionLayerMask;
    [SerializeField] private float      _deocclusionThreshold;
    [SerializeField] private float      _deocclusionSpeed;
    [SerializeField] private UIManager ui;

    private Transform _cameraTransform;
    private Vector3     _rotation;
    private Vector3     _position;
    private float       _zoomAcceleration;
    private float       _zoomVelocity;
    private float       _zoomPosition;
    private Vector3     _deocclusionVector;
    private Vector3     _deocclusionPoint;
    private float       _currentZoom;
    void Start()
    {
        _cameraTransform    = GetComponentInChildren<Camera>().transform;
        _rotation           = transform.localEulerAngles;
        _zoomVelocity       = 0f;
        _zoomPosition       = _cameraTransform.localPosition.z;
        _deocclusionVector  = new Vector3(0f, 0f, _deocclusionThreshold);
    }

    public void Rotate(float angle)
    {
        if (!(angle < 0.0001) || !(angle > -0.0001))
        {
            // Get the current x rotation
            float currentXRotation = transform.rotation.eulerAngles.x;

            // Set the x rotation to 0
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.Rotate(0f, angle, 0f);
            transform.rotation = Quaternion.Euler(currentXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    public void DefiniteRotate(float angle)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);
    }

    void Update()
    {
        if (!ui.GetPause())
        {
            UpdateRotation();
            UpdateHeight();
            UpdateZoom();

            PreventOcclusion();
        }
        
    }

    private void UpdateRotation()
    {
        if (Input.GetButton("Camera"))
        {
            _position = _cameraTransform.localPosition;

            _position.z = _aim;


            _cameraTransform.localPosition = _position;
            //_zoomPosition = _position.z;
        }
        else
        {
            //_zoomPosition = _position.z;
            _position = _cameraTransform.localPosition;

            //_position.z = _closeZoom;


            _cameraTransform.localPosition = _position;
            //_zoomPosition = _position.z;

            _rotation = transform.localEulerAngles;
            _rotation.y += Input.GetAxis("Mouse X");

            transform.localEulerAngles = _rotation;
        }
            
    }

    private void ResetRotation()
    {
        if (_rotation.y != 0f)
        {
            if (_rotation.y < 180f)
                _rotation.y = Mathf.Max(0f, _rotation.y - _resetRotationSpeed * Time.deltaTime);
            else
                _rotation.y = Mathf.Min(360f, _rotation.y + _resetRotationSpeed * Time.deltaTime);

            transform.localEulerAngles = _rotation;
        }
    }

    private void UpdateHeight()
    {
        _rotation = transform.localEulerAngles;
        _rotation.x -= Input.GetAxis("Mouse Y");

        if (_rotation.x < 180f)
            _rotation.x = Mathf.Min(_maxLookDownAngle, _rotation.x);
        else
            _rotation.x = Mathf.Max(_maxLookUpAngle, _rotation.x);

        transform.localEulerAngles = _rotation;
    }

    private void UpdateZoom()
    {
        UpdateZoomVelocity();
        if(!Input.GetButton("Camera"))
            UpdateZoomPosition();
    }

    private void UpdateZoomVelocity()
    {
        _zoomAcceleration = Input.GetAxis("Zoom");

        if (_zoomAcceleration != 0f)
            _zoomVelocity += _zoomAcceleration * Time.deltaTime;
        else if (_zoomVelocity > 0f)
        {
            _zoomVelocity -= _zoomDeceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Max(0f, _zoomVelocity/2); //Trocar número para ser mais lento
        }
        else
        {
            _zoomVelocity += _zoomDeceleration * Time.deltaTime;
            _zoomVelocity = Mathf.Min(0f, _zoomVelocity/2); //trocar número para ser mais lento
        }
    }

    private void UpdateZoomPosition()
    {
        if (_zoomVelocity != 0f)
        {
            _position = _cameraTransform.localPosition;
            _position.z += _zoomVelocity * Time.deltaTime;

            if (_position.z > -_closeZoom || _position.z < -_farZoom)
            {
                _position.z = Mathf.Clamp(_position.z, -_farZoom, -_closeZoom);
                _zoomVelocity = 0f;
            }

            _cameraTransform.localPosition = _position;
            _zoomPosition = _position.z;
        }
    }

    private void PreventOcclusion()
    {
        _deocclusionPoint = _cameraTransform.position - _cameraTransform.TransformDirection(_deocclusionVector);

        if (Physics.Linecast(_deocclusionPivot.position, _deocclusionPoint, out RaycastHit hitInfo, _deocclusionLayerMask.value))
        {
            if (hitInfo.collider.CompareTag("WorldBoundary"))
                _cameraTransform.position = hitInfo.point + _cameraTransform.TransformDirection(_deocclusionVector);
            else
            {
                _position = _cameraTransform.localPosition;
                _position.z += _deocclusionSpeed * Time.deltaTime;

                _cameraTransform.localPosition = _position;
            }
        }
        else
            RevertDeocclusion();
    }

    private void RevertDeocclusion()
    {
        _position = _cameraTransform.localPosition;

        if (_position.z > _zoomPosition && !Input.GetButton("Camera"))
        {
            _position.z = Mathf.Max(_position.z - _deocclusionSpeed * Time.deltaTime, _zoomPosition);
            _deocclusionPoint = transform.TransformPoint(_position) - _cameraTransform.TransformDirection(_deocclusionVector);

            if (!Physics.Linecast(_deocclusionPivot.position, _deocclusionPoint))
                _cameraTransform.localPosition = _position;
        }
    }
}
