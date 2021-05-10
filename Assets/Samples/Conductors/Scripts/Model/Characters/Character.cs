namespace Caliburn.Noesis.Samples.Conductors.Model.Characters
{
    /// <summary>Represents information about an NPC.</summary>
    /// <seealso cref="PropertyChangedBase" />
    /// <seealso cref="IHaveDisplayName" />
    public class Character : PropertyChangedBase, IHaveDisplayName
    {
        #region Public Properties

        /// <summary>Gets the description entries.</summary>
        /// <value>The description entries.</value>
        public IBindableCollection<DescriptionParagraph> DescriptionParagraphs { get; } =
            new BindableCollection<DescriptionParagraph>();

        #endregion

        #region IHaveDisplayName Implementation

        /// <inheritdoc />
        public string DisplayName { get; set; }

        #endregion
    }
}