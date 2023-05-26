using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _zombiesCountLabel;

    public void SetScore(int count)
    {
        _zombiesCountLabel.text = count.ToString();
    }
}
