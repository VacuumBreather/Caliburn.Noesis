namespace Caliburn.Noesis.Samples.Conductors.Model.Characters
{
    /// <summary>
    /// Represents a paragraph of information in a character description.
    /// </summary>
    /// <seealso cref="PropertyChangedBase" />
    public class DescriptionParagraph : PropertyChangedBase
    {
        private string text;
        private bool hasBeenRead;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get => this.text;
            set => Set(ref this.text, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this paragraph has been read by the player.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this paragraph has been read by the player; otherwise, <c>false</c>.
        /// </value>
        public bool HasBeenRead
        {
            get => this.hasBeenRead;
            set => Set(ref this.hasBeenRead, value);
        }
    }
}