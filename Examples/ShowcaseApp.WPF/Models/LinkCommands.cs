using System.Windows.Input;

namespace SimulationV1.WPF.Models
{
    public static class LinkCommands
    {
        private static readonly RoutedUICommand ShowMiniSpecialDialogInternal = new RoutedUICommand("Show help", "ShowMiniSpecialDialog", typeof(LinkCommands));

        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedUICommand ShowMiniSpecialDialog
        {
            get { return ShowMiniSpecialDialogInternal; }
        }
    }
}
