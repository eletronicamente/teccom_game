using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void CarregarJogo()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void SairDoJogo()
    {
        Debug.Log("Sair do jogo...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void CarregarMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
