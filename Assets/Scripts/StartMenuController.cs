using UnityEngine;
using UnityEngine.SceneManagement;

// Panggil namespace UnityEditor HANYA jika sedang di dalam editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnExitClick()
    {
        // Kode ini akan dieksekusi secara berbeda tergantung di mana ia berjalan

#if UNITY_EDITOR
        // JIKA berjalan di dalam Editor Unity, lakukan ini:
        // Menggunakan 'UnityEditor' (tanpa '_')
        EditorApplication.isPlaying = false;
#else
        // JIKA berjalan di game yang sudah di-build (bukan di editor), lakukan ini:
        Application.Quit();
#endif
    }
}