using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BoardPreset m_boardPrest;
    [SerializeField]
    private Button m_restartButton;
    [SerializeField]
    private GameObject m_scoreObj;

    private int myScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.Instance.ProvideService(new BoardManager(this, m_boardPrest));

        ServiceLocator.Instance.InitializeServices();

        var bm = ServiceLocator.Instance.GetService<BoardManager>();
        m_restartButton.GetComponent<Button>().onClick.AddListener(() => { AddScore(0,true); bm.RestartGame(); });
        bm.OnAddScore = (int v) => AddScore(v);

        AddScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        ServiceLocator.Instance.UpdateServices();
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.ShutdownServices();
    }

    private void AddScore(int scoreToAdd, bool isRestart = false) 
    {
        myScore = isRestart ? 0 : (myScore + scoreToAdd);
        m_scoreObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(myScore.ToString());
    }
}
