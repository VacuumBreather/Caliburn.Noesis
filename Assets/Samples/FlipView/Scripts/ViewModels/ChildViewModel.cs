namespace Caliburn.Noesis.Samples.FlipView.ViewModels
{
    public class ChildViewModel : Screen
    {
        private static int index = 0;

        /// <inheritdoc />
        public ChildViewModel()
        {
            DisplayName = $"{GetType().Name} #{index++}";
        }
    }
}