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

namespace WpfConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<String> CommandMemory = new List<string>();
        public int MemoryScrollIndex = 0;
        private RMUD.SinglePlayer.Driver Driver = new RMUD.SinglePlayer.Driver();
        private Action AfterNavigating = null;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                TextBox_TextChanged(null, null);
                InputBox.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            AfterNavigating = () => Driver.Start(typeof(SinglePlayer.Database.Player).Assembly, "SinglePlayer.Database", "Player", 
                s => 
                    Dispatcher.Invoke(new Action<String>(Output), System.Windows.Threading.DispatcherPriority.Normal, s.Replace("\n", "<br>")));

            Clear();

            Driver.BlockOnInput = false;
        }

        public void Output(String s)
        {
            var doc = OutputBox.Document as mshtml.HTMLDocument;
            doc.body.innerHTML += s;
        }

        public void Clear()
        {
            OutputBox.NavigateToString(
@"<script>
    function scroll()
    {
        window.scrollTo(0,document.body.scrollHeight);
    } 
</script><body></body>");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var realFontSize = this.InputBox.FontSize * this.InputBox.FontFamily.LineSpacing;
            var adjustedLineCount = System.Math.Max(this.InputBox.LineCount, 1) + 1;
            var newHeight = realFontSize * adjustedLineCount;
            if (newHeight > this.ActualHeight * 0.75f) newHeight = this.ActualHeight * 0.75f;
            if (newHeight < realFontSize * 2) newHeight = realFontSize * 2;
            this.BottomRow.Height = new GridLength(newHeight);
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Up)
            {
                MemoryScrollIndex -= 1;
                if (MemoryScrollIndex < 0) MemoryScrollIndex = 0;
                if (MemoryScrollIndex < CommandMemory.Count)
                {
                    InputBox.Text = CommandMemory[MemoryScrollIndex];
                    InputBox.Focus();
                    InputBox.SelectAll();
                }

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Down)
            {
                MemoryScrollIndex += 1;
                if (MemoryScrollIndex > CommandMemory.Count) MemoryScrollIndex = CommandMemory.Count;
                if (MemoryScrollIndex < CommandMemory.Count)
                {
                    InputBox.Text = CommandMemory[MemoryScrollIndex];
                    InputBox.Focus();
                    InputBox.SelectAll();
                }

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Return)
            {
                var saveInput = InputBox.Text.Trim();

                try
                {
                    InputBox.Clear();
                    Driver.Input(saveInput);
                    CommandMemory.Add(saveInput);
                    MemoryScrollIndex = CommandMemory.Count;
                }
                catch (Exception x)
                {
                    (OutputBox.Document as dynamic).body.innerHTML += x.Message;
                    InputBox.Text = saveInput;
                }

                OutputBox.InvokeScript("scroll");
                e.Handled = true;
            }
        }

        private void OutputBox_Navigating(object sender, NavigatingCancelEventArgs e)
        {

        }

        private void OutputBox_Navigated(object sender, NavigationEventArgs e)
        {
            if (AfterNavigating != null)
                AfterNavigating();
            AfterNavigating = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RMUD.Core.Shutdown();
        }
    }
}
