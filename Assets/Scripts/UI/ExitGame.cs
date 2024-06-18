using UnityEngine;

public class ExitGame : MonoBehaviour
{
    private bool _quitOnAwake;
    void Awake()
    {
        if (_quitOnAwake) Application.Quit();
    }
    public void QuitApp()
    {
        Application.Quit();
    }
}
