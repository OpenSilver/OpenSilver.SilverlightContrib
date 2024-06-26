﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Collections.Generic;

namespace SilverlightContrib.Utilities.ClipboardHelper
{

    /// <summary>
    /// Clipboard Helper class for adding clipboard support (only available in Internet Explorer)
    /// </summary>
    public class ClipboardHelper : IDisposable
    {
        #region Fields and Properties
        private HtmlWindow _window;
        private bool _ctrlPressed = false;
        private string _currentValue = "";
        private bool _browserSupported = true;
        private UIElement[] _attachedUIElements;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardHelper"/> class.
        /// </summary>
        public ClipboardHelper()
        {
            CheckBrowserCompatibility();
            _window = HtmlPage.Window;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardHelper"/> class.
        /// </summary>
        /// <param name="AttachedUIElements">The attached UI elements. These elements have copy/paste functionality.</param>
        public ClipboardHelper(params UIElement[] AttachedUIElements)
        {
            CheckBrowserCompatibility();
            _window = HtmlPage.Window;
            if (_browserSupported)
            {
                if (AttachedUIElements != null)
                {
                    _attachedUIElements = AttachedUIElements;
                    foreach (UIElement element in AttachedUIElements)
                    {
                        element.KeyDown += new KeyEventHandler(UIElement_KeyDown);
                        element.KeyUp += new KeyEventHandler(UIElement_KeyUp);
                    }
                }
            }
            else
            {
                throw (new InvalidOperationException("Browser not supported"));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets data of type "text" on the clipboard.
        /// </summary>
        /// <param name="Value">string value.</param>
        public void SetData(string Value)
        {
            if (_browserSupported)
                _window.Eval(string.Format("window.clipboardData.setData('text','{0}')", Value));
        }

        /// <summary>
        /// Gets data of type "text" from the clipboard
        /// </summary>
        /// <returns>string</returns>
        public string GetData()
        {
            if (_browserSupported)
            {
                this._currentValue = (string)_window.Eval("window.clipboardData.getData('text')");
                return _currentValue;
            }
            return string.Empty;
        }


        /// <summary>
        /// Clears all of the data on the clipboard.
        /// </summary>
        public void ClearData()
        {
            if (_browserSupported)
                _window.Eval("window.clipboardData.clearData()");
        }

        #endregion

        #region Events

        private void UIElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (_browserSupported && e.Key == Key.Ctrl)
                _ctrlPressed = true;
        }

        private void UIElement_KeyUp(object sender, KeyEventArgs e)
        {
            if (_browserSupported)
            {
                if (e.Key == Key.Ctrl)
                    _ctrlPressed = false;
                if (e.Key == Key.V && _ctrlPressed)
                {
                    this.GetData();

                    if (sender.GetType() == typeof(Button))
                    {
                        Button buttonSender = (Button)sender;
                        buttonSender.Content = this.GetData();
                    }
                }
            }
        }

        #endregion   

        #region Browser Check

        private void CheckBrowserCompatibility()
        {
            if (HtmlPage.BrowserInformation.Name != "Microsoft Internet Explorer"
                && (HtmlPage.BrowserInformation.Platform != "Win32"
                || HtmlPage.BrowserInformation.Platform != "Win64")
                )
            {
                _browserSupported = false;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Detaches the key up and down events of the attached UIELements
        /// </summary>
        public void Dispose()
        {
            foreach (UIElement uiElement in _attachedUIElements)
            {
                uiElement.KeyDown -= new KeyEventHandler(UIElement_KeyDown);
                uiElement.KeyUp -= new KeyEventHandler(UIElement_KeyUp);
            }
        }

        #endregion

    }
}