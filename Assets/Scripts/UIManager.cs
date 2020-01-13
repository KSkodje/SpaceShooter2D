using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText = null;
    [SerializeField]
    private Image _livesImg = null;
    [SerializeField]
    private Sprite[] _livesSprites = null;
    [SerializeField]
    private Text _gameOverText = null;
    private bool _isGameOver = false;
    [SerializeField]
    private Text _restartText = null;
    [SerializeField]
    private GameManager _gameManager = null;


    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _isGameOver = false;
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _isGameOver = true;
        StartCoroutine(GameOverFlicker());
    }

    private IEnumerator GameOverFlicker()
    {
        while (_isGameOver)
        {
            yield return new WaitForSeconds(.5f);
            GameObject gameover = _gameOverText.gameObject;
            if (gameover.activeInHierarchy == true)
            {
                gameover.SetActive(false);
            }
            else
            {
                gameover.SetActive(true);
            }
        }
    }
}
