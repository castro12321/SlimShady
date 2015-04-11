using SlimShady.DataFeed;
using System.Windows.Controls;

namespace SlimShady
{
    public partial class ConfigurationControl : UserControl
    {
        public ConfigurationControl()
        {
            InitializeComponent();
            
            ThingSpeakAddressTextBox.TextChanged += (x, y) => ThingSpeakDataFeed.Instance.BaseUrl = ((TextBox)x).Text;
            ThingSpeakChannelTextBox.TextChanged += (x, y) => ThingSpeakDataFeed.Instance.Channel = ((TextBox)x).Text;
        }
        
        private void PlotFunction_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainWindow.Trace("txtchanged plot Function: " + ((TextBox)sender).Text);
            ThingSpeakLightConverter.Function = ((TextBox)sender).Text;
            OxyModel.RefreshModel(Plot);
        }
    }
}
