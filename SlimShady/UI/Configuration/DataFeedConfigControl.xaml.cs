using SlimShady.DataFeed;
using SlimShadyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SlimShady.UI.Configuration
{
    /// <summary>
    /// Interaction logic for DataFeedConfigControl.xaml
    /// </summary>
    public partial class DataFeedConfigControl : UserControl
    {
        public DataFeedConfigControl()
        {
            InitializeComponent();

            ThingSpeakAddressTextBox.TextChanged += (x, y) => ThingSpeakDataFeed.Instance.BaseUrl = ((TextBox)x).Text;
            ThingSpeakChannelTextBox.TextChanged += (x, y) => ThingSpeakDataFeed.Instance.Channel = ((TextBox)x).Text;
        }

        private void PlotFunction_TextChanged(object sender, TextChangedEventArgs e)
        {
            NkLogger.info("txtchanged plot Function: " + ((TextBox)sender).Text);
            ThingSpeakLightConverter.Function = ((TextBox)sender).Text;
            OxyModel.RefreshModel(Plot);
        }
    }
}
