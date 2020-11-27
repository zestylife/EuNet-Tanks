using EuNet.Rpc;
using System.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public interface IActorRpc : IViewRpc
    {
        Task OnMove(float move, float turn, Vector3 position, Quaternion rotation);
        Task OnFire(string shellResource, Vector3 position, Quaternion rotation, Vector3 velocity);
        Task OnDamage(float damage, float currentHp);
    }
}
