using System;
using System.Diagnostics;
using System.Threading;
#if !UNITY_5_5_OR_NEWER
using System.Threading.Tasks;
#endif
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis.Samples
{
    public class ShellViewModel : Screen
    {
        private static ILog Log = LogManager.GetLog(typeof(ShellViewModel));

        private readonly IDialogService _dialogService;
        private readonly DialogScreen _dialog = new QueryDialog("ARE YOU SURE?", "You are about to overwrite your save. All your progress will be lost!", DialogResults.Yes | DialogResults.No, DialogResult.No);

        private string _helloMessage;
        private string _saveMessage;

        public ShellViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public string HelloMessage
        {
            get => _helloMessage;
            set => Set(ref _helloMessage, value);
        }

        public string SaveMessage
        {
            get => _saveMessage;
            set => Set(ref _saveMessage, value);
        }

        public void SayHello(string firstName, string lastName)
        {
            HelloMessage = $"Hello, {firstName} {lastName}";
        }

        public async UniTask OpenDialogAsync()
        {
            var result = await _dialogService.ShowDialogAsync(_dialog);

            if (result == DialogResult.Yes)
            {
                SaveMessage = "Saving...";
                await TaskHelper.Delay(2000);
                SaveMessage = string.Empty;
            }
        }
    }
}
