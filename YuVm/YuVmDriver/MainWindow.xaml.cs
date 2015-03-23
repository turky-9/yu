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
using YuVm;

namespace YuVmDriver
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            List<VMOp> lst = new List<VMOp>();

            lst.Add(new VMOp(EOpCode.Push, 10));
            lst.Add(new VMOp(EOpCode.Push, 11));
            lst.Add(new VMOp(EOpCode.Add, 0));

            VM vm = new VM(lst);

            this.dgMem.ItemsSource = vm.Mem;
            this.dgStack.ItemsSource = vm.Stack.Stack;

            vm.Run();



            int a = 0;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YuLexer lex = new YuLexer(this.txtSrc.Text);
                YuParser parser = new YuParser(lex);
                YuEval eval = new YuEval();

                YuTreeNode tree = parser.NextTree();
                while (tree.Token.Type != ETokenType.Eof)
                {
                    eval.TranslateTree(tree);
                    tree = parser.NextTree();
                }
                eval.TranslateTree(tree);
                eval.Eval();
                this.dgMem.ItemsSource = eval.Vm.Mem;
                this.dgStack.ItemsSource = eval.Vm.Stack.Stack;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
