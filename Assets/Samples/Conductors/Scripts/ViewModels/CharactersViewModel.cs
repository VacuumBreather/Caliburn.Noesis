namespace Caliburn.Noesis.Samples.Conductors.ViewModels
{
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Model.Characters;

    /// <summary>The screen listing all the NPCs.</summary>
    public class CharactersViewModel : Screen, ISubScreen
    {
        #region Constants and Fields

        private readonly CharacterDatabase npcDatabase = new CharacterDatabase();
        private Character selectedCharacter;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="CharactersViewModel" /> class.</summary>
        public CharactersViewModel()
        {
            DisplayName = "Characters";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a list of all NPCs.</summary>
        /// <value>A list of all NPCs.</value>
        public IBindableCollection<Character> Characters => this.npcDatabase.Characters;

        /// <summary>Gets or sets the selected character.</summary>
        /// <value>The selected character.</value>
        public Character SelectedCharacter
        {
            get => this.selectedCharacter;
            set => Set(ref this.selectedCharacter, value);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override UniTask OnActivateAsync(CancellationToken cancellationToken)
        {
            SelectedCharacter = Characters.FirstOrDefault();

            return UniTask.CompletedTask;
        }

        #endregion
    }
}