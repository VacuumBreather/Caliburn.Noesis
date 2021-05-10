namespace Caliburn.Noesis.Samples.Conductors.Model.Characters
{
    /// <summary>The database containing information about all NPCs.</summary>
    /// <seealso cref="PropertyChangedBase" />
    public class CharacterDatabase : PropertyChangedBase
    {
        #region Public Properties

        /// <summary>Gets a list of all NPCs.</summary>
        /// <value>A list of all NPCs.</value>
        public IBindableCollection<Character> Characters { get; } =
            new BindableCollection<Character>();

        #endregion
    }
}