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
    private Text _ammoText = null;
    [SerializeField]
    private Text _gameOverText = null;
    private bool _isGameOver = false;
    [SerializeField]
    private Text _restartText = null;
    [SerializeField]
    private GameManager _gameManager = null;
    private Camera _mainCamera = null;

    [SerializeField]
    private Image _thrusterBar = null;


    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _isGameOver = false;
        _mainCamera = Camera.main;
        if (!_mainCamera)
        {
            Debug.LogError("_mainCamera is Null");
        }
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
        // Camera shakes on health boost pickup

        //StartCoroutine(CameraShake());
    }

    public void UpdateThrusterBar(float currentThrusterValue, float maxThrusterValue)
    {

        _thrusterBar.fillAmount = currentThrusterValue / maxThrusterValue;        
        if(_thrusterBar.fillAmount == 1)
        {
            _thrusterBar.gameObject.SetActive(false);
        }
        else
        {
            _thrusterBar.gameObject.SetActive(true);
        }
    }

    public void UpdateAmmo(int ammoCount)
    {
        _ammoText.text = "Ammo: " + ammoCount;
        if(ammoCount < 5)
        {
            _ammoText.color = Color.red;
        }
        else
        {
            _ammoText.color = Color.white;
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

    public IEnumerator CameraShake()
    {
        // From http://wiki.unity3d.com/index.php/Camera_Shake
        // with some alterations
        //Debug.Log("Shaking the camera");
        float shakeDuration = 2.0f;
        float startDuration = 1f;
        float smoothAmount = 100f;
        float startAmount = 2f;
        float shakeAmount = 1f;
        while (shakeDuration > 0.01f)
        {
            Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
            rotationAmount.z = 0;//Don't change the Z; it looks funny.

            float shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime*2);//Lerp the time, so it is less and tapers off towards the end.
            _mainCamera.transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);

            yield return null;
        }
        _mainCamera.transform.localRotation = Quaternion.identity;
    }
}
