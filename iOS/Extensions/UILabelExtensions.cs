﻿using System;
using UIKit;
using System.Threading.Tasks;
using System.Linq;

namespace Slink.iOS
{
    public static class UILabelExtensions
    {
        async public static void AnimateText(this UILabel label, string text, double delay, Action completed)
        {
            label.Text = String.Empty;

            if (!String.IsNullOrEmpty(text))
            {
                foreach (char character in text.ToCharArray())
                {
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                    label.Text = label.Text + character;
                }

                completed?.Invoke();
            }
        }
    }
}
