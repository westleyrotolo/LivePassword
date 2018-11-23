using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LivePassword.Common
{
    // Classe per gestire messagebox e inputbox, wrappa XNA
    public static class DialogService
    {
        public static Task<DialogResult> ShowMessageAsync(string title, string message, string primary)
        {
            return ShowMessageAsync(title, message, primary, null);
        }

        public static Task<DialogResult> ShowMessageAsync(string title, string message, string primary, string secondary)
        {
            var tsc = new TaskCompletionSource<DialogResult>();
            var buttons = secondary == null ? new string[] { primary } : new string[] { primary, secondary };
            Guide.BeginShowMessageBox(title, message, buttons, 0, MessageBoxIcon.None, (result) =>
            {
                var choice = Guide.EndShowMessageBox(result);
                if (choice.HasValue)
                {
                    tsc.SetResult(choice.Value == 0 ? DialogResult.Primary : DialogResult.Secondary);
                }
                else
                {
                    tsc.SetResult(DialogResult.None);
                }

            }, null);

            return tsc.Task;
        }

        public static void ShowMessage(string title, string message, string primary)
        {
            ShowMessage(title, message, primary, null);
        }

        public static void ShowMessage(string title, string message, string primary, string secondary)
        {
            var buttons = secondary == null ? new string[] { primary } : new string[] { primary, secondary };
            Guide.BeginShowMessageBox(title, message, buttons, 0, MessageBoxIcon.None, (result) =>
            {
            }, null);
        }

        public static void ShowMessageAndExit(string title, string message, string primary)
        {
            ShowMessageAndExit(title, message, primary, null);
        }

        public static void ShowMessageAndExit(string title, string message, string primary, string secondary)
        {
            var buttons = secondary == null ? new string[] { primary } : new string[] { primary, secondary };
            Guide.BeginShowMessageBox(title, message, buttons, 0, MessageBoxIcon.None, (result) =>
            {
                Application.Current.Terminate();
            }, null);
        }

        public static Task<string> ShowInputAsync(string title, string message, string defaultText)
        {
            var tsc = new TaskCompletionSource<string>();
            Guide.BeginShowKeyboardInput(PlayerIndex.One, title, message, defaultText, (result) =>
            {
                var choice = Guide.EndShowKeyboardInput(result);

                tsc.SetResult(choice);

            }, null);

            return tsc.Task;
        }
    }

    public enum DialogResult
    {
        Primary,
        Secondary,
        None
    }
}
