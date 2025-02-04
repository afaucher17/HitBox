﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CRI.HitBox.Lang;
using CRI.HitBox.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CRI.HitBox.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UICountdownPage : MonoBehaviour, IHideable
    {
        [SerializeField]
        protected CanvasGroup _canvasGroup;
        [SerializeField]
        protected Text _countdownText;
        [SerializeField]
        protected int _countdownTime = 3;
        [SerializeField]
        protected bool startGame = true;

        private bool _countdownStarted = false;

        private Coroutine _coroutine = null;

        /// <summary>
        /// The path of the countdown audio clip.
        /// </summary>
        private StringCommon _countdownClipPath;
        /// <summary>
        /// The path of the go audio clip.
        /// </summary>
        private StringCommon _goClipPath;
        /// <summary>
        /// The text at the end of the countdon.
        /// </summary>
        private StringCommon _countdownEndText;

        private IEnumerator Start()
        {

            if (GetComponentInParent<UIScreenMenu>() != null)
            {
                while (!GetComponentInParent<UIScreenMenu>().loaded)
                    yield return null;
            }
            var settings = (CountdownSettings)ApplicationManager.instance.menuSettings.screenSettings
                .First(x => x.GetScreenType() == Settings.ScreenSettings.ScreenType.Countdown);
            _countdownClipPath = settings.countdownAudioPath;
            _goClipPath = settings.goAudioPath;
            _countdownTime = settings.countdownTime;
            _countdownEndText = settings.text;
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_countdownText == null)
                _countdownText = GetComponentInChildren<Text>();
            Hide();
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            if (_countdownStarted)
            {
                StopCoroutine(_coroutine);
                _countdownStarted = false;
            }
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            if (!_countdownStarted)
                _coroutine = StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            _countdownStarted = true;
            int countdown = _countdownTime;
            if (!string.IsNullOrEmpty(_countdownClipPath.key) && AudioManager.instance.isLoaded && GetComponentInParent<UIScreenMenu>() != null)
                AudioManager.instance.PlayClip(_countdownClipPath.key, _countdownClipPath.common);
            while (countdown >= 0)
            {
                _countdownText.text = (countdown > 0) ? countdown.ToString() : TextManager.instance.GetText(_countdownEndText);
                if (countdown == 0 && !string.IsNullOrEmpty(_goClipPath.key) && AudioManager.instance.isLoaded && GetComponentInParent<UIScreenMenu>() != null)
                {
                    while (AudioManager.instance.isPlaying)
                        yield return null;
                    AudioManager.instance.PlayClip(_goClipPath.key, _goClipPath.common);
                }
                yield return new WaitForSeconds(1.0f);
                countdown--;
            }
            if (GetComponentInParent<UIScreenMenu>() != null && startGame)
            {
                GetComponentInParent<UIScreenMenu>().GoToScoreScreen();
                ApplicationManager.instance.StartGame();
            }
            if (!string.IsNullOrEmpty(_countdownClipPath.key) && AudioManager.instance.isLoaded)
                AudioManager.instance.StopClip(_countdownClipPath.key, _countdownClipPath.common);
            _countdownStarted = false;
        }

        public bool HasNext(out int nextStyle)
        {
            nextStyle = 0;
            return false;
        }

        public bool HasPrevious()
        {
            return true;
        }
    }
}