using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace CosmicMemory.Helper
{
    public sealed class YandexMobileAdsInterstitial : MonoBehaviour
    {
        #region Singleton
        public static YandexMobileAdsInterstitial Instance;
        #endregion

        #region Fields
        private readonly TimeSpan _limitShow = TimeSpan.FromMinutes(1f);
        private DateTime _lastShow = DateTime.Now;

        private string message = "";

        private InterstitialAdLoader interstitialAdLoader;
        private Interstitial interstitial;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            this.interstitialAdLoader = new InterstitialAdLoader();
            this.interstitialAdLoader.OnAdLoaded += this.HandleAdLoaded;
            this.interstitialAdLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

            RequestInterstitial();
        }

        private void RequestInterstitial()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            // Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
            string adUnitId = "demo-interstitial-yandex";

            if (this.interstitial != null)
            {
                this.interstitial.Destroy();
            }

            this.interstitialAdLoader.LoadAd(this.CreateAdRequest(adUnitId));
            this.DisplayMessage("Interstitial is requested");
        }

        public void ShowInterstitial()
        {
            if (this.interstitial == null)
            {
                this.DisplayMessage("Interstitial is not ready yet");
                return;
            }

            if (DateTime.Now - _lastShow < _limitShow)
            {
                this.DisplayMessage($"Время показа ещё не пришло {DateTime.Now - _lastShow}");
                return;
            }

            this.interstitial.Show();
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        private void DisplayMessage(String message)
        {
            this.message = message + (this.message.Length == 0 ? "" : "\n--------\n" + this.message);
            MonoBehaviour.print(message);
        }

        #region Interstitial callback handlers

        public void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            this.DisplayMessage("HandleAdLoaded event received");

            this.interstitial = args.Interstitial;

            this.interstitial.OnAdClicked += this.HandleAdClicked;
            this.interstitial.OnAdShown += this.HandleAdShown;
            this.interstitial.OnAdFailedToShow += this.HandleAdFailedToShow;
            this.interstitial.OnAdImpression += this.HandleImpression;
            this.interstitial.OnAdDismissed += this.HandleAdDismissed;
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            this.DisplayMessage($"HandleAdFailedToLoad event received with message: {args.Message}");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleAdClicked event received");
        }

        public void HandleAdShown(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleAdShown event received");
            _lastShow = DateTime.Now;
        }

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleAdDismissed event received");

            DestroyInterstitial();

            RequestInterstitial();
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            this.DisplayMessage($"HandleImpression event received with data: {data}");
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            this.DisplayMessage($"HandleAdFailedToShow event received with message: {args.Message}");

            DestroyInterstitial();

            RequestInterstitial();
        }

        private void DestroyInterstitial()
        {
            if (interstitial != null)
            {
                this.interstitial.Destroy();
                this.interstitial = null;
            }
        }
        #endregion
    }
}
