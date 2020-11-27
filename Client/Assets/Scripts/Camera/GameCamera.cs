using EuNet.Unity;
using UnityEngine;

[ExecutionOrder(100)]
public class GameCamera : MonoBehaviour
{
    private const float MoveSmooth = 10.0f;

    void FixedUpdate()
    {
        var actor = GameManager.Instance.ControlActor;
        if(actor != null)
        {
            Vector3 resultPos = actor.transform.position + (new Vector3(-43f, 41f, -24f));

            transform.position = Vector3.Lerp(transform.position, resultPos, Time.deltaTime * MoveSmooth);
            transform.localRotation = Quaternion.Euler(40.0f, 60.0f, 0.0f);
        }
    }
}
