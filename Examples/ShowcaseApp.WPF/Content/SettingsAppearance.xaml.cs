﻿using System.Windows.Controls;

namespace SimulationV1.WPF.Content
{
    /// <summary>
    /// Interaction logic for SettingsAppearance.xaml
    /// </summary>
    public partial class SettingsAppearance : UserControl
    {
        public SettingsAppearance()
        {
            InitializeComponent();

            // a simple view model for appearance configuration
            this.DataContext = new SettingsAppearanceViewModel();
        }
    }
}
