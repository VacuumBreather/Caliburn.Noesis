namespace Caliburn.Noesis
{
    /// <summary>A dialog displaying information to the user. The user can only confirm it.</summary>
    public class InformationDialog : QueryDialog
    {
        /// <summary>Initializes a new instance of the <see cref="InformationDialog"/> class.</summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content text of the dialog.</param>
        public InformationDialog(string title, string content)
            : base(title, content, DialogResults.Ok, DialogResult.Ok)
        {
        }
    }
}
