namespace Caliburn.Noesis
{
    /// <summary>A dialog displaying a user query. The user can confirm it based on the defined dialog result options.</summary>
    public class QueryDialog : DialogScreen
    {
        /// <summary>Initializes a new instance of the <see cref="QueryDialog"/> class.</summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content text of the dialog.</param>
        /// <param name="dialogResults">The possible results the dialog can return.</param>
        /// <param name="defaultResult">(Optional) The default result.</param>
        public QueryDialog(string title,
            string content,
            DialogResults dialogResults,
            DialogResult defaultResult = DialogResult.None)
        {
            Title = title;
            Content = content;
            DialogResults = dialogResults;
            DefaultResult = defaultResult;
        }

        /// <summary>Gets the content of the dialog.</summary>
        public string Content { get; }

        /// <summary>Gets the default result.</summary>
        public DialogResult DefaultResult { get; }

        /// <summary>Gets the possible results the dialog can return.</summary>
        public DialogResults DialogResults { get; }

        /// <summary>Gets the title of the dialog.</summary>
        public string Title { get; }
    }
}
