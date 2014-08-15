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
using WebSocketSharp;


namespace DoshiiTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //DoshiiWebSocketsCommunication wsLogic;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //wsLogic = new DoshiiWebSocketsCommunication(textBox1.Text);
            //wsLogic.webSocketMessage += new EventHandler<WebSocketSharp.MessageEventArgs>(wsLogic_webSocketMessage);
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
            //wsLogic.SendMessage(textBox1.Text);
            textBox1.Text = "";
        }
    }
}
