using Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiLogin : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private InputField _nicknameInput;

    string[] _names = new string[] { "Anderson", "Ashwoon", "Aikin", "Bateman", "Bongard", "Bowers", "Boyd", "Cannon", "Cast", "Deitz", "Dewalt", "Ebner", "Frick", "Hancock", "Haworth", "Hesch", "Hoffman", "Kassing", "Knutson", "Lawless", "Lawicki", "Mccord", "McCormack", "Miller", "Myers", "Nugent", "Ortiz", "Orwig", "Ory", "Paiser", "Pak", "Pettigrew", "Quinn", "Quizoz", "Ramachandran", "Resnick", "Sagar", "Schickowski", "Schiebel", "Sellon", "Severson", "Shaffer", "Solberg", "Soloman", "Sonderling", "Soukup", "Soulis", "Stahl", "Sweeney", "Tandy", "Trebil", "Trusela", "Trussel", "Turco", "Uddin", "Uflan", "Ulrich", "Upson", "Vader", "Vail", "Valente", "Van Zandt", "Vanderpoel", "Ventotla", "Vogal", "Wagle", "Wagner", "Wakefield", "Weinstein", "Weiss", "Woo", "Yang", "Yates", "Yocum", "Zeaser", "Zeller", "Ziegler", "Bauer", "Baxster", "Casal", "Cataldi", "Caswell", "Celedon", "Chambers", "Chapman", "Christensen", "Darnell", "Davidson", "Davis", "DeLorenzo", "Dinkins", "Doran", "Dugelman", "Dugan", "Duffman", "Eastman", "Ferro", "Ferry", "Fletcher", "Fietzer", "Hylan", "Hydinger", "Illingsworth", "Ingram", "Irwin", "Jagtap", "Jenson", "Johnson", "Johnsen", "Jones", "Jurgenson", "Kalleg", "Kaskel", "Keller", "Leisinger", "LePage", "Lewis", "Linde", "Lulloff", "Maki", "Martin", "McGinnis", "Mills", "Moody", "Moore", "Napier", "Nelson", "Norquist", "Nuttle", "Olson", "Ostrander", "Reamer", "Reardon", "Reyes", "Rice", "Ripka", "Roberts", "Rogers", "Root", "Sandstrom", "Sawyer", "Schlicht", "Schmitt", "Schwager", "Schutz", "Schuster", "Tapia", "Thompson", "Tiernan", "Tisler" };

    private void Start()
    {
        _loginButton.OnClickAsAsyncEnumerable().Subscribe(OnLoginAsync);
            
    }

    private async UniTaskVoid OnLoginAsync(AsyncUnit asyncUnit)
    {
        try
        {
            _loginButton.interactable = false;

            string nickname = _nicknameInput.text;
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = _names[UnityEngine.Random.Range(0, _names.Length)];
            }

            var result = await GameClient.Instance.Client.ConnectAsync(TimeSpan.FromSeconds(10));

            if (result == true)
            {
                var rpc = GameClient.Instance.Rpc;

                var loginResult = await rpc.Login(
                    1,
                    SystemInfo.deviceUniqueIdentifier,
                    "key");

                Debug.Log($"Login Result : {loginResult}");
                if (loginResult != 0)
                    return;

                var userInfo = await rpc.GetUserInfo(nickname);
                Debug.Log($"UserId : {userInfo.Id} \t UserName : {userInfo.Name}");
                GameClient.Instance.UserInfo = userInfo;

                var joinResult = await rpc.QuickJoinRoom(GameMode.Normal);
                Debug.Log($"Join : {joinResult}");
                if (joinResult == false)
                    throw new Exception("Fail to join room");

                // Game Scene 으로 이동
                await SceneManager.LoadSceneAsync("Game");
            }
            else
            {
                GameClient.Instance.Client.Disconnect();
                Debug.LogError("Fail to connect server");
                NotifyManager.Instance.ShowTip("Fail to connect server");
            }
        }
        catch(Exception ex)
        {
            GameClient.Instance.Client.Disconnect();
            Debug.LogException(ex);
            NotifyManager.Instance.ShowTip($"Fail to connect server : {ex.Message}");
        }
        finally
        {
            if (_loginButton != null)
                _loginButton.interactable = true;
        }
    }

    public async Task SetTestIpAddress()
    {
        var www = UnityWebRequest.Get("https://zestylife.github.io/EuNet/ip.html");
        await www.SendWebRequest();

        if (!www.isNetworkError && !www.isHttpError)
        {
            var address = www.downloadHandler.text.Trim();
            Debug.Log(address);
            GameClient.Instance.Client.ClientOption.TcpServerAddress = address;
            GameClient.Instance.Client.ClientOption.UdpServerAddress = address;
        }
    }
}
