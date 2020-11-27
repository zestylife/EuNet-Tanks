using Common;
using EuNet.Core;
using EuNet.Unity;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Actor : MonoBehaviour , INetViewHandler, INetSerializable , INetViewPeriodicSync , IActorRpc
{
    private NetView _view;
    public  NetView View => _view;
    private ActorRpc _actorRpc;
    private SyncVector3 _netPos;
    private SyncQuaternion _netRot;

    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _turnSpeed = 180f;
    [SerializeField] private AudioSource _movementAudio;
    [SerializeField] private AudioClip _engineIdling;
    [SerializeField] private AudioClip _engineDriving;
    [SerializeField] private float _pitchRange = 0.2f;

    private Rigidbody _rigidbody;
    private float _movementInputValue;
    private float _turnInputValue;
    private float _originalPitch;
    private ParticleSystem[] _particles;

    [SerializeField] private Transform _fireTransform;
    [SerializeField] private Slider _aimSlider;
    [SerializeField] private AudioSource _shootingAudio;
    [SerializeField] private AudioClip _chargingClip;
    [SerializeField] private AudioClip _fireClip;
    [SerializeField] private float _minLaunchForce = 15f;
    [SerializeField] private float _maxLaunchForce = 30f;
    [SerializeField] private float _maxChargeTime = 0.75f;

    private float _currentLaunchForce;
    private float _chargeSpeed;

    [SerializeField] private float _startingHealth = 100f;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Color _fullHealthColor = Color.green;
    [SerializeField] private Color _zeroHealthColor = Color.red;
    [SerializeField] private GameObject _explosionPrefab;

    private AudioSource _explosionAudio;
    private ParticleSystem _explosionParticles;
    private float _hp;
    private bool _isDead;

    [SerializeField] private Text _nicknameText;
    [SerializeField] private Renderer[] _tankRenderers;

    private Color _color;
    private bool _isChargeFire;
    public string Nickname => _nicknameText.text;

    private void Awake()
    {
        _view = GetComponent<NetView>();
        _actorRpc = new ActorRpc(_view);

        _rigidbody = GetComponent<Rigidbody>();

        _explosionParticles = Instantiate(_explosionPrefab).GetComponent<ParticleSystem>();
        _explosionAudio = _explosionParticles.GetComponent<AudioSource>();
        _explosionParticles.gameObject.SetActive(false);

        _netPos = _rigidbody.position;
        _netRot = _rigidbody.rotation;
    }

    private void Start()
    {
        _originalPitch = _movementAudio.pitch;
        _chargeSpeed = (_maxLaunchForce - _minLaunchForce) / _maxChargeTime;

        ActorManager.Instance.Add(this);
    }

    private void OnEnable()
    {
        _rigidbody.isKinematic = false;
        _movementInputValue = 0f;
        _turnInputValue = 0f;
        _particles = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < _particles.Length; ++i)
        {
            _particles[i].Play();
        }

        _currentLaunchForce = _minLaunchForce;
        _aimSlider.value = _minLaunchForce;

        _hp = _startingHealth;
        _isDead = false;
        SetHealthUI();
    }

    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        _rigidbody.isKinematic = true;

        // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
        for (int i = 0; i < _particles.Length; ++i)
        {
            _particles[i].Stop();
        }
    }

    private void OnDestroy()
    {
        ActorManager.Instance?.Remove(this);
    }

    private void Update()
    {
        if (_isChargeFire)
        {
            _currentLaunchForce += _chargeSpeed * Time.deltaTime;
            _currentLaunchForce = Mathf.Min(_currentLaunchForce, _maxLaunchForce);

            _aimSlider.value = _currentLaunchForce;
        }
        else
        {
            _aimSlider.value = _minLaunchForce;
        }

        EngineAudio();

        _nicknameText.transform.rotation = Camera.main.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (_view.IsMine())
        {
            var moveDelta = transform.forward * _movementInputValue * _speed * Time.deltaTime;
            _rigidbody.MovePosition(_rigidbody.position + moveDelta);

            float turnDelta = _turnInputValue * _turnSpeed * Time.deltaTime;
            Quaternion turnRot = Quaternion.Euler(0f, turnDelta, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * turnRot);
        }
        else
        {
            _rigidbody.position = _netPos.Update(Time.deltaTime);
            _rigidbody.rotation = _netRot.Update(Time.deltaTime);

            _netPos.Velocity = transform.forward * _movementInputValue * _speed;
        }
    }

    private void EngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(_movementInputValue) < 0.1f && Mathf.Abs(_turnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (_movementAudio.clip == _engineDriving)
            {
                // ... change the clip to idling and play it.
                _movementAudio.clip = _engineIdling;
                _movementAudio.pitch = Random.Range(_originalPitch - _pitchRange, _originalPitch + _pitchRange);
                _movementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (_movementAudio.clip == _engineIdling)
            {
                // ... change the clip to driving and play.
                _movementAudio.clip = _engineDriving;
                _movementAudio.pitch = Random.Range(_originalPitch - _pitchRange, _originalPitch + _pitchRange);
                _movementAudio.Play();
            }
        }
    }

    public void SetChargeFire()
    {
        _isChargeFire = true;
    }

    public void SetMove(float move, float turn)
    {
        _actorRpc
            .ToOthers(DeliveryMethod.ReliableSequenced)
            .OnMove(move, turn, _rigidbody.position, _rigidbody.rotation);

        //Debug.Log($"SetMove {move} {turn}");
        OnMove(move, turn, _rigidbody.position, _rigidbody.rotation);
    }

    public Task OnMove(float move, float turn, Vector3 position, Quaternion rotation)
    {
        _movementInputValue = move;
        _turnInputValue = turn;

        _netRot.Set(_rigidbody.rotation, rotation, new Vector3(0f, _turnInputValue * _turnSpeed, 0f));
        _netPos.Set(_rigidbody.position, position, transform.forward * _movementInputValue * _speed);

        return Task.CompletedTask;
    }

    public void Fire()
    {
        if (_view.IsMine() == false)
            return;

        _actorRpc
            .ToOthers(DeliveryMethod.ReliableOrdered)
            .OnFire("Shell", _fireTransform.position, _fireTransform.rotation, _currentLaunchForce * _fireTransform.forward);

        OnFire("Shell", _fireTransform.position, _fireTransform.rotation, _currentLaunchForce * _fireTransform.forward);

        // Reset the launch force.  This is a precaution in case of missing button events.
        _currentLaunchForce = _minLaunchForce;
        _isChargeFire = false;
    }

    public Task OnFire(string shellResource, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        var res = Resources.Load(shellResource);
        if (res == null)
            return Task.CompletedTask;

        var shellInstance = Instantiate(res, position, rotation) as GameObject;
        var rigidbody = shellInstance.GetComponent<Rigidbody>();

        rigidbody.velocity = velocity;

        _shootingAudio.clip = _fireClip;
        _shootingAudio.Play();

        return Task.CompletedTask;
    }

    public void Damage(float amount)
    {
        if (_view.IsMine() == false)
            return;

        _hp -= amount;

        _actorRpc
            .ToOthers(DeliveryMethod.ReliableOrdered)
            .OnDamage(amount, _hp);

        OnDamage(amount, _hp);
    }

    public Task OnDamage(float damage, float currentHp)
    {
        _hp = currentHp;

        SetHealthUI();

        if (_hp <= 0f && !_isDead)
        {
            OnDeath();
        }

        return Task.CompletedTask;
    }

    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        _slider.value = _hp;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        _fillImage.color = Color.Lerp(_zeroHealthColor, _fullHealthColor, _hp / _startingHealth);
    }

    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        _isDead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        _explosionParticles.Play();

        // Play the tank explosion sound effect.
        _explosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive(false);

        GameManager.Instance.OnDeath(this);
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    private void SetColorAndName()
    {
        foreach (var renderer in _tankRenderers)
        {
            renderer.material.color = _color;
        }

        var slot = GameClient.Instance.Room.FindBySessionId(_view.OwnerSessionId);
        if (slot != null)
        {
            _nicknameText.text = slot.Name;
        }
    }

    public void OnViewInstantiate(NetDataReader reader)
    {
        _color = reader.ReadColor();
        SetColorAndName();
    }

    public void OnViewDestroy(NetDataReader reader)
    {
        
    }

    public void OnViewMessage(NetDataReader reader)
    {
        throw new System.NotImplementedException();
    }

    public bool OnViewPeriodicSyncSerialize(NetDataWriter writer)
    {
        writer.Write(_rigidbody.position);
        writer.Write(_rigidbody.rotation);
        writer.Write(_turnInputValue);
        writer.Write(_movementInputValue);

        return true;
    }

    public void OnViewPeriodicSyncDeserialize(NetDataReader reader)
    {
        var pos = reader.ReadVector3();
        var rot = reader.ReadQuaternion();
        _turnInputValue = reader.ReadSingle();
        _movementInputValue = reader.ReadSingle();

        _netRot.Set(_rigidbody.rotation, rot, new Vector3(0f, _turnInputValue * _turnSpeed, 0f));
        _netPos.Set(_rigidbody.position, pos, transform.forward * _movementInputValue * _speed);
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Write(_color);
    }

    public void Deserialize(NetDataReader reader)
    {
        _color = reader.ReadColor();

        SetColorAndName();
    }
}
