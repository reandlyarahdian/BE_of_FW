using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

namespace Watermelon
{
    public class UIWheel : UIPage
    {
        [SerializeField] RectTransform safeAreaTransform;
        [SerializeField] WheelManager wheelManager;

        [Space]
        [SerializeField] UIFadeAnimation backgroundFade;
        [SerializeField] UIScaleAnimation wheelScale;

        [Space]
        [SerializeField] UIScaleAnimation rewardLabel;
        [SerializeField] TextMeshProUGUI rewardAmountText;

        [Header("Coins Label")]
        [SerializeField] UIScaleAnimation coinsPanelScalable;
        [SerializeField] CurrencyUIPanelSimple coinsPanelUI;

        [Header("Buttons")]
        [SerializeField] UIScaleAnimation spinFade;
        [SerializeField] UIScaleAnimation extraButtonAnimation;
        [SerializeField] Button spinButton;
        [SerializeField] Button extraButton;

        private int coinsHash = FloatingCloud.StringToHash("Coins");
        private int currentReward;

        public bool isClicked = false;

        private void Update()
        {
            extraButton.interactable = isActiveated;
        }

        public override void Initialise()
        {
            coinsPanelUI.Initialise();
            extraButton.onClick.AddListener(ExtraButton);
            
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
            extraButton.interactable = false;

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            coinsPanelScalable.Show(fadeDuration);

            wheelScale.Show(fadeDuration);

            spinFade.Show(fadeDuration);

            extraButtonAnimation.Show(fadeDuration);
            spinButton.interactable = true;

            isClicked = false;

            UIController.OnPageOpened(this);
        }

        public void ShowRewardLabel(float rewardAmounts, bool immediately = false, float duration = 0.3f, Action onComplted = null)
        {
            rewardLabel.Show(immediately: immediately);

            if (immediately)
            {
                rewardAmountText.text = "+" + rewardAmounts;
                onComplted?.Invoke();

                return;
            }

            rewardAmountText.text = "+" + 0;

            Tween.DoFloat(0, rewardAmounts, duration, (float value) =>
            {

                rewardAmountText.text = "+" + (int)value;
            }).OnComplete(delegate
            {

                onComplted?.Invoke();
            });
        }

        public bool isActiveated;

        public void WheelButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            UIController.HidePage<UIWheel>(() =>
            {
                UIController.ShowPage<UIComplete>();
            });
        }

        public void ExtraButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            if (!isClicked)
            {

                AdsManager.ShowRewardBasedVideo((bool success) =>
                {
                    if (success)
                    {
                        int rewrdMulti = 2;

                        ShowRewardLabel(wheelManager.CollectableRewordEx() * rewrdMulti, false, 0.3f, delegate
                        {
                            FloatingCloud.SpawnCurrency(coinsHash, rewardLabel.RectTransform, coinsPanelScalable.RectTransform, 10, "", () =>
                            {
                                CurrenciesController.Add(CurrencyType.Coins, wheelManager.CollectableRewordEx() * rewrdMulti);

                                ClientManager.Instance.WheelsPoints(wheelManager.CollectableRewordEx() * rewrdMulti);
                            });
                        });

                        float fadeDuration = 0.3f;
                        spinFade.Show(fadeDuration);
                        extraButtonAnimation.Show(fadeDuration);

                        extraButton.interactable = false;
                    }
                    else
                    {
                        WheelButton();
                    }
                });

                isClicked = true;
                isActiveated = false;
            }

            extraButton.interactable = false;
        }
    }
}