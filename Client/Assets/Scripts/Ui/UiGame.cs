using EuNet.Unity;
using System;
using System.Linq;
using System.Text;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UiGame : MonoBehaviour
{
    [SerializeField] private Text _playerInfoText;
    
    private void Start()
    {
        gameObject
            .AddComponent<ObservableUpdateTrigger>()
            .UpdateAsObservable()
            .Sample(TimeSpan.FromMilliseconds(500))
            .Subscribe(OnUpdateUI);
    }

    private void OnUpdateUI(Unit unit)
    {
        var room = GameClient.Instance.Room;
        if(room != null)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var slot in room.GetFilledSlots<ClientRoomSlot>())
            {
                var udp = slot.Session.UdpChannel;
                if(slot.UserId == GameClient.Instance.UserInfo.Id)
                    builder.AppendLine($"[Myself] {slot.Name} P[{udp.Ping}ms] L[{udp.LocalEndPoint}] R[{udp.RemoteEndPoint}] T[{udp.TempEndPoint}]");
                else builder.AppendLine($"[{slot.Member.State}] {slot.Name} P[{udp.Ping}ms] L[{udp.LocalEndPoint}] R[{udp.RemoteEndPoint}] T[{udp.TempEndPoint}]");
            }

            _playerInfoText.text = builder.ToString();
        }
    }
}
