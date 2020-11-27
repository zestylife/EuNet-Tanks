using EuNet.Unity;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using zFrame.UI;

[ExecutionOrder(-20)]
public class InputManager : MonoBehaviour
{
    [SerializeField] Joystick _joystick;
    [SerializeField] Button _fireButton;

    private Vector2 _cachedMovement = Vector2.zero;

    private void Start()
    {
        _joystick.OnValueChanged.AddListener(OnJoystick);
        _fireButton.OnPointerDownAsObservable().Subscribe(x =>
        {
            Actor actor = GameManager.Instance.ControlActor;
            if(actor)
            {
                actor.SetChargeFire();
            }
        });

        _fireButton.OnPointerUpAsObservable().Subscribe(x =>
        {
            Actor actor = GameManager.Instance.ControlActor;
            if (actor)
            {
                actor.Fire();
            }
        });
    }

    private void FixedUpdate()
    {
        //ActorInput();
    }

    private void ActorInput()
    {
        float turn = Input.GetAxis("Horizontal1");
        float movement = Input.GetAxis("Vertical1");

        OnChangedMovementInput(new Vector2(turn, movement));
    }

    private void OnJoystick(Vector2 value)
    {
        OnChangedMovementInput(value);
    }

    private void OnChangedMovementInput(Vector2 value)
    {
        value.x = Mathf.Clamp(value.x * 2f, -1f, 1f);
        value.y = Mathf.Clamp(value.y * 2f, -1f, 1f);

        if ((_cachedMovement - value).sqrMagnitude >= 0.01f)
        {
            var actor = GameManager.Instance.ControlActor;
            if (actor == null)
                return;

            actor.SetMove(value.y, value.x);
            _cachedMovement = value;
        }
    }
}
