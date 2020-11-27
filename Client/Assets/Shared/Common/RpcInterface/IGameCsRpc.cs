using EuNet.Rpc;
using System.Threading.Tasks;

namespace Common
{
    public interface IGameCsRpc : IRpc
    {
        Task<int> Login(int version, string id, string authKey);
        Task<UserInfo> GetUserInfo(string name);

        // 자동으로 방의 빈 슬롯으로 입장함. 성공시 true를 반환하고 자동으로 방정보를 입력됨
        Task<bool> QuickJoinRoom(GameMode gameMode);
    }
}
