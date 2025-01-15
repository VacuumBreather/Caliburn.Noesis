using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis.Samples.EarlySample
{
    public class ShellViewModel : Screen
    {
        private readonly IDialogService _dialogService;
        private string _oldName;

        public ShellViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            DisplayName = "Hey here I am";
            OldName = "Nothing yet";
        }

        private bool hasHadError;
        private int i = 1;

        public async UniTask OpenDialog()
        {
            var result = await _dialogService.ShowDialogAsync(new QueryDialog("Hello", "Hello World", DialogResults.Yes | DialogResults.No | DialogResults.Cancel, DialogResult.Yes));

            DisplayName = result.ToString();
        }

        public async UniTask ChangeName(string displayName)
        {
            DisplayName = "Changed Name" + i++;

#if UNITY_5_5_OR_NEWER
            await UniTask.Delay(2000);
#else
            await Task.Delay(2000).AsUniTask();
#endif

            if (!hasHadError)
            {
                hasHadError = true;
                throw new Exception("Hello");
            }
            
            OldName = displayName;
        }

        public string OldName
        {
            get => _oldName;
            set => Set(ref _oldName, value);
        }
    }
}
