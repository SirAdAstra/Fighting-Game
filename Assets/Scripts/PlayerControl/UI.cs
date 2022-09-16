using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject playerHP;
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private GameObject enemyHP;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject uses;
    private TextMeshProUGUI usesText;

    private void Start()
    {
        usesText = uses.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, 10f, enemyMask);
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i].gameObject.tag == "Enemy")
            {
                enemyPanel.SetActive(true);
                enemyHP.GetComponent<RectTransform>().sizeDelta = new Vector2(enemyColliders[i].gameObject.GetComponent<AIController>().health, 23);
            }
            else
            {
                enemyPanel.SetActive(false);
            }
        }
        
        if (enemyColliders.Length == 0)
        {
            enemyPanel.SetActive(false);
        }

        playerHP.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<BattleModule>().health, 23);
        usesText.text = gameObject.GetComponent<BattleModule>().uses.ToString();
    }

}
