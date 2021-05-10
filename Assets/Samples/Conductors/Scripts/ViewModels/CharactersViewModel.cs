namespace Caliburn.Noesis.Samples.Conductors.ViewModels
{
    using Model.Characters;

    /// <summary>The screen listing all the NPCs.</summary>
    public class CharactersViewModel : Screen, ISubScreen
    {
        /// <summary>Initializes a new instance of the <see cref="CharactersViewModel" /> class.</summary>
        public CharactersViewModel()
        {
            DisplayName = "Characters";
        }

        private readonly CharacterDatabase npcDatabase = new CharacterDatabase();

        /// <summary>Gets a list of all NPCs.</summary>
        /// <value>A list of all NPCs.</value>
        public IBindableCollection<Character> Characters => this.npcDatabase.Characters;
    }
}