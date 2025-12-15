using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BootManager : MonoBehaviour
{
    public Image bootImage; // XP boot image
    public float bootDuration = 4f; // seconds on boot screen
    private void Start()
    {
        if (bootImage) bootImage.gameObject.SetActive(true);
        StartCoroutine(BootSequence());
    }


    private IEnumerator BootSequence()
    {
        yield return new WaitForSeconds(bootDuration);
        // after boot -> login scene
        SceneManager.LoadScene("Login");
    }
}