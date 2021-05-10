namespace Caliburn.Noesis.Samples.Conductors.Model.Characters
{
    /// <summary>
    /// Represents information about an NPC.
    /// </summary>
    /// <seealso cref="PropertyChangedBase" />
    /// <seealso cref="IHaveDisplayName" />
    public class Character : PropertyChangedBase, IHaveDisplayName
    {
        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the description entries.
        /// </summary>
        /// <value>
        /// The description entries.
        /// </value>
        public IBindableCollection<string> DescriptionParagraphs { get; } =
            new BindableCollection<string>();
    }
}