using UnityEngine;
using UnityEngine.UI;

namespace SCPrototype
{
    public class GameScreen : MonoBehaviour
    {
        public Image SideTurnViewBackground;
        public Text SideTurnViewText;

        private bool SideTurn;

        void Awake()
        {
            SideTurn = true;
            SwitchTurnView();
        }

        public void SwitchTurnView()
        {
            if (SideTurn)
            {
                SideTurnViewBackground.color = Color.white;
                SideTurnViewText.color = Color.black;
                SideTurnViewText.text = "White";
            }
            else
            {
                SideTurnViewBackground.color = Color.black;
                SideTurnViewText.color = Color.white;
                SideTurnViewText.text = "Black";
            }

            SideTurn = !SideTurn;
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}