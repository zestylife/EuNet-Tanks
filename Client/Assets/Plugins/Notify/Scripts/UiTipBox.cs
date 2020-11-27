using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiTipBox : MonoBehaviour
{
    [SerializeField] private Text _label;

    private string _text;
    private CanvasGroup _group;
    private Sequence _sequence;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        _sequence = DOTween.Sequence()
            .SetAutoKill(false)
            .Append(_group.DOFade(1f, 1f))
            .AppendInterval(2f)
            .Append(_group.DOFade(0f, 1f))
            .AppendCallback(() => { gameObject.SetActive(false); });
    }

    public void ShowTip(string tip)
    {
        if (gameObject.activeInHierarchy == true)
        {
            gameObject.SetActive(false);
        }

        _text = tip;

        _label.text = _text;
        _group.alpha = 0f;

        gameObject.SetActive(true);
        _sequence.Restart();
    }
}
