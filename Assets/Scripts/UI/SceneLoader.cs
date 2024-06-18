using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int _levelIndex = 0;
   
    public void LoadLevel()
    {
        SceneManager.LoadScene(_levelIndex);
    }
}
