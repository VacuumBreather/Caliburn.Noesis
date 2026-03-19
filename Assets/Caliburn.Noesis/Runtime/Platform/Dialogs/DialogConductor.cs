using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>A conductor handling dialogs.</summary>
    public sealed class DialogConductor : Conductor<DialogScreen>.Collection.OneActive, IDialogService
    {
        private sealed class IdentityComparer<T> : IEqualityComparer<T> where T : class
        {
            public bool Equals(T x, T y) => ReferenceEquals(x, y);
            public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
        
        private readonly QueryDialog _queryDialog = new("", "", DialogResults.Ok, DialogResult.Ok);
        private readonly Dictionary<DialogScreen, UniTaskCompletionSource<DialogResult>> _activeTrackers = 
            new(new IdentityComparer<DialogScreen>());

        /// <summary>Initializes a new instance of the <see cref="DialogConductor" /> class.</summary>
        public DialogConductor()
        {
            DisplayName = GetType().Name;
        }

        /// <inheritdoc />
        object IHaveReadOnlyActiveItem.ActiveItem => ActiveItem;

        /// <inheritdoc />
        public async UniTask<DialogResult> ShowDialogAsync(DialogScreen dialog,
            CancellationToken cancellationToken = default)
        {
            if (Items.Contains(dialog))
                throw new ArgumentException(
                    $"Attempting to open a {dialog.GetType().Name} dialog with the same instance multiple times simultaneously.",
                    nameof(dialog));
            
            var tcs = new UniTaskCompletionSource<DialogResult>();
            _activeTrackers[dialog] = tcs;

            try
            {
                await ActivateItemAsync(dialog, cancellationToken);

                return await tcs.Task;
            }
            catch
            {
                _activeTrackers.Remove(dialog);
                throw;
            }
        }

        public override async UniTask DeactivateItemAsync(DialogScreen item, bool close, CancellationToken cancellationToken = default)
        {
            await base.DeactivateItemAsync(item, close, cancellationToken);

            if (close)
            {
                _activeTrackers[item].TrySetResult(item.DialogResult);
                _activeTrackers.Remove(item);
            }
        }

        /// <inheritdoc />
        public UniTask<DialogResult> ShowQueryDialogAsync(string title, string content, DialogResults dialogResults,
            DialogResult defaultResult = DialogResult.None, CancellationToken cancellationToken = default)
        {
            _queryDialog.Title = title;
            _queryDialog.Content = content;
            _queryDialog.DialogResults = dialogResults;
            _queryDialog.DefaultResult = defaultResult;

            return ShowDialogAsync(_queryDialog, cancellationToken);
        }

        /// <inheritdoc />
        public UniTask<DialogResult> ShowInformationDialogAsync(string title, string content,
            CancellationToken cancellationToken = default)
        {
            _queryDialog.Title = title;
            _queryDialog.Content = content;
            _queryDialog.DialogResults = DialogResults.Ok;
            _queryDialog.DefaultResult = DialogResult.Ok;

            return ShowDialogAsync(_queryDialog, cancellationToken);
        }
    }
}
