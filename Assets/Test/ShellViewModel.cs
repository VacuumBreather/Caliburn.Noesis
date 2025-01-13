using Cysharp.Threading.Tasks;
using UnityEngine;
using Screen = Caliburn.Noesis.Screen;

namespace Testing
{
    public class ShellViewModel : Screen
    {
        private string _oldName;

        public ShellViewModel()
        {
            DisplayName = "Hey here I am";
            OldName = "Nothing yet";
        }

        public async UniTask ChangeName(string displayName)
        {
            DisplayName = "Changed Name";

            await UniTask.Delay(2000);
            
            OldName = displayName;
        }

        public string OldName
        {
            get => _oldName;
            set => Set(ref _oldName, value);
        }
    }
}
