using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class MainMenu : MonoBehaviour
    {
        public Scene game;

        public void StartGame()
        {
            SceneManager.LoadScene("Scene");
        }
    }
}
