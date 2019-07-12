using UnityEngine;
using UnityEngine.UI;

public class LeakReporter : MonoBehaviour
{
    #region GameObject取得
    [SerializeField] private GameObject score;
    [SerializeField] private Text scoreText;
    #endregion

    #region Private Field
    private Stack stack;
    #endregion

    private void Start()
    {
        stack = GameObject.Find("Manager").GetComponent<Stack>();
    }

    //下に落ちたらゲーム終了
    private void OnCollisionEnter2D(Collision2D collision)
    {
        stack.phase = Phase.End;
        scoreText.text = stack.scoreLog.text;
        score.SetActive(true);
    }
}
