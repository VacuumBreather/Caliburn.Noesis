namespace Caliburn.Noesis.Samples.Conductors.ViewModels
{
    using Model.Characters;

    /// <summary>The screen listing all the NPCs.</summary>
    public class CharactersViewModel : Screen, ISubScreen
    {
        #region Constants and Fields

        private readonly CharacterDatabase npcDatabase = new CharacterDatabase();

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

        #endregion
    }
}