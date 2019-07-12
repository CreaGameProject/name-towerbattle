using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Phase {
    Contoroll,
    PreJudge,
    Fall,
    Judge,
    PreWait,
    Wait,
    End
};
public class Stack : MonoBehaviour
{
    #region GameObject取得
    [SerializeField] private GameObject[] stackObject;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject scorePanel;
    #endregion

    #region Public Field
    public Phase phase;
    public Text scoreLog;
    public List<Rigidbody2D> stackObjectList;
    #endregion

    #region Private Field
    private GameObject nowObject;
    [SerializeField] private float[] sizes;
    private float startY;
    private float range;
    #endregion

    private void Start()
    {
       NewGame();
    }


    void Update()
    {
        //フェイズによって分岐
        switch (phase)
        {
            case Phase.Contoroll:
                Controll(); break;
            case Phase.PreJudge:
                StartCoroutine("JustWait");
                break;
            case Phase.Judge:
                Judge();
                break;
            case Phase.PreWait:
                StartCoroutine("Wait");
                break;
            case Phase.End:
                if (Input.GetKeyDown(KeyCode.Return)) NewGame();
                break;
            default:break;
        }
    }

    //新規ゲーム開始→(Startともう一度ボタンにアタッチ)
    public void NewGame()
    {
        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("stackObject"))
        {
            Destroy(gameObject);
        }
        scorePanel.SetActive(false);

        startY = 8f;
        camera.transform.position = Vector3.back * 10f;

        phase = Phase.Contoroll;
        stackObjectList = new List<Rigidbody2D>();
        Generate();
    }

    //新オブジェクト生成
    private void Generate()
    {
        int n = Random.Range(0, stackObject.Length);
        nowObject = Instantiate(stackObject[n], Vector3.up * startY, new Quaternion());
        nowObject.transform.localScale *= sizes[Random.Range(0, sizes.Length)];
        nowObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        scoreLog.text = "記録 :"+stackObjectList.Count.ToString();
        stackObjectList.Add(nowObject.GetComponent<Rigidbody2D>());
    }

    //左右移動、回転、落下
    private void Controll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            nowObject.transform.Rotate(Vector3.back * 30f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            nowObject.transform.Translate(Vector3.right * 0.1f, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            nowObject.transform.Translate(Vector3.left * 0.1f, Space.World);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            nowObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            phase = Phase.PreJudge;
            range = 0;
        }
    }

    //落下直後の判断誤作動を防ぐため1秒待機
    private IEnumerator JustWait()
    {
        phase = Phase.Fall;
        yield return new WaitForSeconds(1f);
        phase = Phase.Judge;
    }

    //動いていないか判断
    private void Judge()
    {
        bool moving = false;
        range += 0.0000005f;
        foreach(Rigidbody2D rb in stackObjectList)
        {
            moving |= (rb.velocity.magnitude > range);
        }
        if (!moving)
        {
            phase = Phase.PreWait;
            float height = -10;
            foreach(Rigidbody2D rb in stackObjectList)
            {
                height = height > rb.transform.position.y ? height : rb.transform.position.y;
            }
            height = height < 0 ? 0 : height;
            startY = height + 8f;
            camera.transform.position = new Vector3(0, height, -10);
        }
    }

    //1秒後に次のオブジェクトが出現
    private IEnumerator Wait()
    {
        phase = Phase.Wait;
        yield return new WaitForSeconds(1f);
        phase = Phase.Contoroll;
        Generate();
    }
}
