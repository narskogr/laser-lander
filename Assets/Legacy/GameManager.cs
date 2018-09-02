using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject lander;

    void Update()
    {
        if (!lander)
        {
            StartCoroutine(Reload(SceneManager.GetActiveScene().name));
        }
    }

    IEnumerator Reload(string SceneName)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneName);
    }
}
