using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoginManager : MonoBehaviour
{
    public Button accountButton;
    public Text welcomeText; // put on the left
    public float welcomeDuration = 3f;


    private void Start()
    {
        if (welcomeText) welcomeText.gameObject.SetActive(false);
        if (accountButton != null) accountButton.onClick.AddListener(OnAccountClicked);
    }


    private void OnAccountClicked()
    {
        if (welcomeText) welcomeText.gameObject.SetActive(true);
        StartCoroutine(GoToDesktop());
    }


    private IEnumerator GoToDesktop()
    {
        yield return new WaitForSeconds(welcomeDuration);
        SceneManager.LoadScene("Desktop");
    }
}