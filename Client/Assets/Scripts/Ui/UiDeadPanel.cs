using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UiDeadPanel : MonoBehaviour
{
    [SerializeField] private Button _respawnButton;

    private CanvasGroup _group;
    private Sequence _sequence;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        _sequence = DOTween.Sequence()
            .SetAutoKill(false)
            .AppendInterval(1f)
            .Append(_group.DOFade(1f, 2f));
    }

    private void Start()
    {
        _respawnButton.OnClickAsObservable().Subscribe(OnClickRespawnButton);
    }

    private void OnEnable()
    {
        _group.alpha = 0f;
        _sequence.Restart();
    }

    private void OnClickRespawnButton(Unit unit)
    {
        GameManager.Instance.RespawnMyPlayer();
        gameObject.SetActive(false);
    }
}
