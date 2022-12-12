using AutoDbService.DbPrism.Attributes;
using AutoDbService.Entity.Entities;
using System;
using System.Collections.Generic;
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

namespace AutoDbService.TestAModule.Views
{
    /// <summary>
    /// AddLabwareView.xaml 的交互逻辑
    /// </summary> 
    [DbTableAddView(typeof(User))]
    public partial class AddUserView : UserControl
    {
        public AddUserView()
        {
            InitializeComponent();
        }
    }
}
