﻿using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace CosmicMemory.Helper
{
    public sealed class YandexMobileAdsBannerDemoScript : MonoBehaviour
    {
        private string message = "";

        private Banner banner;

        private void Awake()
        {
            RequestBanner();
        }

        private void RequestBanner()
        {

            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            // Replace demo Unit ID 'demo-banner-yandex' with actual Ad Unit ID
            string adUnitId = "R-M-11968369-1";

            if (this.banner != null)
            {
                this.banner.Destroy();
            }
            // Set sticky banner width
            //BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
            // Or set inline banner maximum width and height
            //BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
            BannerAdSize bannerSize = BannerAdSize.InlineSize(40, GetScreenHeightDp());
            this.banner = new Banner(adUnitId, bannerSize, AdPosition.CenterRight);

            this.banner.OnAdLoaded += this.HandleAdLoaded;
            this.banner.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            this.banner.OnReturnedToApplication += this.HandleReturnedToApplication;
            this.banner.OnLeftApplication += this.HandleLeftApplication;
            this.banner.OnAdClicked += this.HandleAdClicked;
            this.banner.OnImpression += this.HandleImpression;

            this.banner.LoadAd(this.CreateAdRequest());
            this.DisplayMessage("Banner is requested");
        }

        // Example how to get screen width for request
        private int GetScreenWidthDp()
        {
            int screenWidth = (int)Screen.safeArea.width;
            return ScreenUtils.ConvertPixelsToDp(screenWidth);
        }

        private int GetScreenHeightDp()
        {
            int screenHeight = (int)Screen.safeArea.height;
            return ScreenUtils.ConvertPixelsToDp(screenHeight);
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

        private void DisplayMessage(String message)
        {
            this.message = message + (this.message.Length == 0 ? "" : "\n--------\n" + this.message);
            MonoBehaviour.print(message);
        }

        #region Banner callback handlers

        public void HandleAdLoaded(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleAdLoaded event received");
            this.banner.Show();
        }

        public void HandleAdFailedToLoad(object sender, AdFailureEventArgs args)
        {
            this.DisplayMessage("HandleAdFailedToLoad event received with message: " + args.Message);
        }

        public void HandleLeftApplication(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleLeftApplication event received");
        }

        public void HandleReturnedToApplication(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleReturnedToApplication event received");
        }

        public void HandleAdLeftApplication(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleAdLeftApplication event received");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            this.DisplayMessage("HandleAdClicked event received");
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            this.DisplayMessage("HandleImpression event received with data: " + data);
        }

        #endregion
    }
}
