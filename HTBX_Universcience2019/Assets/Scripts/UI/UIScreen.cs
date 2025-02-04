﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CRI.HitBox.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour, IHideable
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        protected bool _ready = false;

        protected bool _visible = false;

        public bool visible
        {
            get
            {
                return _visible;
            }
        }

        protected virtual void Awake()
        {
            Hide();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual IEnumerator Start()
        {
            _ready = false;
            if (GetComponentInParent<UIScreenMenu>() != null)
            {
                while (!GetComponentInParent<UIScreenMenu>().loaded)
                    yield return null;
            }
            _ready = true;
        }

        public virtual void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _visible = false;
        }

        public virtual void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _visible = true;
        }

        protected virtual void Update()
        {
        }

        public bool HasNext(out int nextStyle)
        {
            nextStyle = 0;
            return false;
        }

        public bool HasPrevious()
        {
            return false;
        }
    }
}