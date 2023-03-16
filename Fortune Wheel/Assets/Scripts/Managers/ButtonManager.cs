using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class ButtonManager : MonoBehaviour
    {
        [Header("PANELS:")]
        [SerializeField] private Button collectButton;
        [SerializeField] private Button continueButton, giveUpButton, goBackButton, reviveButton;

        private UIManager uiManager;

        // Start is called before the first frame update
        private void Start()
        {
            uiManager = UIManager.Instance;
            
            collectButton.onClick.AddListener(uiManager.CollectAllRewards);
            continueButton.onClick.AddListener(uiManager.StartANewGame);
            giveUpButton.onClick.AddListener(uiManager.ResetGame);
            goBackButton.onClick.AddListener(uiManager.StartANewGame);
            reviveButton.onClick.AddListener(uiManager.Revive);
        }
    }
}
