using EuNet.Rpc;
using System.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public interface IGameManagerRpc : IViewRpc
    {
        Task OnRespawn(int viewId, Vector3 position);
    }
}
