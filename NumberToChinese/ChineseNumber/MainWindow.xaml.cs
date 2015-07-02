using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ChineseNumber {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            MoneyInput.Focus();
            MoneyInput.SelectAll();
        }
                
        private void MoneyInput_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            TextBox Target = (TextBox)sender;
            Regex regex = new Regex("[^0-9]+");//過濾非數字
            e.Handled = regex.IsMatch(e.Text);

            //Fix IME 中文輸入
            if(e.Handled) Target.Text = Target.Text.Replace(e.Text, "");
        }

        private void MoneyInput_KeyUp(object sender ,KeyEventArgs e) {
            if(MoneyInput.Text.Length == 0)
                MoneyInput.Text = "0";
            string FormatValue = MoneyInput.Text.Replace("," ,"");
            if(FormatValue.Length > 15)
                FormatValue = FormatValue.Substring(0 ,15);
            FormatValue = long.Parse(FormatValue).ToString("#,0");
            
            MoneyInput.Text = FormatValue;
            MoneyInput.Select(MoneyInput.Text.Length ,0);
            CalMoney();
        }

        private void TaxInput_KeyUp(object sender ,KeyEventArgs e) {
            CalMoney();
        }

        private void CalMoney() {
            double Value = double.Parse("0" + MoneyInput.Text.Replace("," ,""));
            double TaxValue   = double.Parse("0" + TaxInput.Text.Replace(",",""));
            if(HasTax.IsChecked.HasValue && HasTax.IsChecked.Value) {
                long NoTax = (long)Math.Floor(Value / (TaxValue + 100)*100 + 0.5);
                NoTaxOutput.Text = NoTax.ToChineseNumber();
                TaxOutput.Text = ((long)Value - NoTax).ToChineseNumber();
                ChineseOutput.Text = ((long)Value).ToChineseNumber();
            } else {
                long Tax = (long)Math.Floor(Value * TaxValue/100 + 0.5);
                NoTaxOutput.Text = ((long)Value).ToChineseNumber();
                TaxOutput.Text = Tax.ToChineseNumber();
                ChineseOutput.Text =(Tax + (long)Value).ToChineseNumber();
            }
        }


        private void HasTax_Click(object sender ,RoutedEventArgs e) {
            CalMoney();
        }

        #region L1
        private void L1ListBox_SelectionChanged(object sender ,SelectionChangedEventArgs e) {
            L1Value.Text = ((ListBoxItem)L1ListBox.SelectedItem).Content.ToString();
        }

        private void L1Update_Click(object sender ,RoutedEventArgs e) {
            ListBoxItem Sed = (ListBoxItem)L1ListBox.SelectedItem;
            Sed.Content = L1Value.Text;
            UpdateDir();
            CalMoney();
        }
        #endregion

        #region L2
        private void L2ListBox_SelectionChanged(object sender ,SelectionChangedEventArgs e) {
            if(L2ListBox.SelectedItem == null)return;
            L2Value.Text = ((ListBoxItem)L2ListBox.SelectedItem).Content.ToString();
        }

        private void L2Update_Click(object sender ,RoutedEventArgs e) {
            if(L2ListBox.SelectedItem == null)return;
            ListBoxItem Sed = (ListBoxItem)L2ListBox.SelectedItem;
            Sed.Content = L2Value.Text;
            UpdateDir();
            CalMoney();
        }
        #endregion

        private void L2Del_Click(object sender ,RoutedEventArgs e) {
            if(L2ListBox.SelectedItem == null)return;
            if(L2ListBox.Items.Count == 1)return;
            L2ListBox.Items.RemoveAt(L2ListBox.SelectedIndex);
            UpdateDir();
            CalMoney();
        }

        private void L2Add_Click(object sender ,RoutedEventArgs e) {
            ListBoxItem NEW = new ListBoxItem();
            NEW.Content = L2Value.Text;
            L2ListBox.Items.Add(NEW);
            UpdateDir();
            CalMoney();
        }

        private void UpdateDir() {//更新對照字典
            List<string> L1 = new List<string>();L1.Add("");
            foreach(ListBoxItem t in L1ListBox.Items)L1.Add(t.Content.ToString());

            List<string> L2 = new List<string>();L2.Add("");
            foreach(ListBoxItem t in L2ListBox.Items)L2.Add(t.Content.ToString());

            NumberExtension.ChineseUnitLevel0 = L1.ToArray();
            NumberExtension.ChineseUnitLevel1 = L2.ToArray();
            NumberExtension.ChineseUnit = new string[][] { NumberExtension.ChineseUnitLevel0 ,NumberExtension.ChineseUnitLevel1 };
        }

        private void Button_Click(object sender ,RoutedEventArgs e) {
            MoneyInput.Clear();
            MoneyInput.Focus();
        }
    }
}
