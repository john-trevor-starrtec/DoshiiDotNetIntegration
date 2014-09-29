using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Models;
using WebSocketSharp;


namespace DoshiiTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //DoshiiWebSocketsCommunication wsLogic;


        private DoshiiHelper helper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            helper = new DoshiiHelper(textBox1.Text, textBox3.Text, OrderModes.BistroMode, SeatingModes.DoshiiAllocation, textBox2.Text, true);
        }

        void wsLogic_webSocketMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.Type == Opcode.Text) {
              // Do something with e.Data
                textBox1.Text = e.Data.ToString();

              return;
            }

            if (e.Type == Opcode.Binary) {
              // Do something with e.RawData
                textBox1.Text = e.RawData.ToString();

              return;
            }
            
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            helper.DeleteAllProducts();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            helper.AddNewProducts(helper.MyCompleteProductList);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            helper.DoshiiProductList = helper.GetAllProducts();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "ws://meerkat.staging.dev.impos.com.au/pos/api/v1/socket";
            textBox3.Text = "NUb3jlKiphiRmfSCbqHOZ_zV6_Q";
            textBox2.Text = "http://meerkat.staging.dev.impos.com.au/pos/api/v1/";
        }

    }
}
