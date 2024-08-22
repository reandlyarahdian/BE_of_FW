using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Watermelon
{
    public class UIWheel : UIPage
    {
        [SerializeField] RectTransform safeAreaTransform;
        [SerializeField] WheelManager wheelManager;

        [Space]
        [SerializeField] UIFadeAnimation backgroundFade;
        [SerializeField] UIScaleAnimation wheelScale;

        [Header("Coins Label")]
        [SerializeField] UIScaleAnimation coinsPanelScalable;
        [SerializeField] CurrencyUIPanelSimple coinsPanelUI;

        [Header("Buttons")]
        [SerializeField] UIScaleAnimation spinFade;
        [SerializeField] UIScaleAnimation homeButtonScaleAnimation;
        [SerializeField] Button spinButton;
        [SerializeField] Button homeButton;

        private TweenCase noThanksAppearTween;

        private int coinsHash = FloatingCloud.StringToHash("Coins");
        private int currentReward;

        public override void Initialise()
        {
            coinsPanelUI.Initialise();
            homeButton.onClick.AddListener(HomeButton);
            NotchSaveArea.RegisterRectTransform(safeAreaTransform);
        }

        public override void PlayHideAnimation()
        {
            if (!isPageDisplayed)
                return;

            backgroundFade.Hide(0.25f);
            coinsPanelScalable.Hide();

            canvas.enabled = false;
            isPageDisplayed = false;

            UIController.OnPageClosed(this);
        }

        public override void PlayShowAnimation()
        {
            if (isPageDisplayed)
                return;

            isPageDisplayed = true;
            canvas.enabled = true;

            coinsPanelUI.Activate();

            spinButton.interactable = false;
            homeButton.interactable = false;

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            coinsPanelScalable.Show();

            wheelScale.Show();

            spinFade.Show(1.05f, 0.75f);
            spinButton.interactable = true;

            homeButtonScaleAnimation.Show(1.05f, 0.25f, 1f);
            homeButton.interactable = true;

            UIController.OnPageOpened(this);
        }
        public void HomeButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            UIController.HidePage<UIComplete>(() =>
            {
                GameController.ReturnToMenu();
            });

            LivesManager.AddLife();
        }

        public void WheelButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            UIController.HidePage<UIWheel>(() =>
            {
                GameController.LoadNextLevel();
            });
        }
    }
}