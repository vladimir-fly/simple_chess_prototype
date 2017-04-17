using UnityEngine;
using UnityEngine.UI;

namespace SCPrototype
{
    public class GameScreen : MonoBehaviour
    {
        public Image SideTurnViewBackground;
        public Text SideTurnViewText;
        public Text Caption;

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

        public void ShowCaption(string message)
        {
            if (Caption == null || string.IsNullOrEmpty(message)) return;
            Caption.text = message;
            Debug.Log(string.Format("[ShowCaption] Message: {0}", message));
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}