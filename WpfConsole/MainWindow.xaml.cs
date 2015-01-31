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
        private bool ShuttingDown = false;

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

            AfterNavigating = () => Driver.Start(typeof(Space.Player).Assembly, "Space",
                s =>
                    Dispatcher.Invoke(new Action<String>(Output), System.Windows.Threading.DispatcherPriority.Normal, PrepareString(s)),
                new RMUD.StartUpAssembly(typeof(StandardActionsModule.MiscRules).Assembly, new RMUD.ModuleInfo { BaseNameSpace = "StandardActionsModule" }));

            Clear();

            Driver.BlockOnInput = false;
            RMUD.Core.OnShutDown += () =>
                {
                    if (ShuttingDown) return;
                    Dispatcher.Invoke(new Action(() => Close()));
                };
        }

        public String PrepareString(String s)
        {
            s = s.Replace("\n", "<br>");
            s = s.Replace("  ", "&nbsp;&nbsp;");
            return s;
        }

        public void Output(String s)
        {
            var doc = OutputBox.Document as mshtml.HTMLDocument;
            
            while (doc.body == null) ;

            doc.body.innerHTML += s;
            OutputBox.InvokeScript("scroll");
        }

        public void Clear()
        {
            OutputBox.NavigateToString(
@"<head>
<style>
body
{
    margin-right: 50px;
    margin-left: 50px;

    background-color: #BAE4F7;
}
</style>
<script>
    function scroll()
    {
        window.scrollTo(0,document.body.scrollHeight + 1000);
    } 
</script></head><body></body>");
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
            else if (e.Key == Key.Return)
            {
                var saveInput = InputBox.Text.Trim();

                try
                {
                    InputBox.Clear();
                    (OutputBox.Document as dynamic).body.innerHTML += "<font color=red>" + saveInput + "</font><br>";
                    Driver.Input(saveInput);
                    CommandMemory.Add(saveInput);
                    MemoryScrollIndex = CommandMemory.Count;
                }
                catch (Exception x)
                {
                    (OutputBox.Document as dynamic).body.innerHTML += "<br><font color=red>" + x.Message + "</font><br>";
                    InputBox.Text = saveInput;
                }

                e.Handled = true;
            }
        }

        private void OutputBox_Navigating(object sender, NavigatingCancelEventArgs e)
        {

        }

        private void OutputBox_Navigated(object sender, NavigationEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ShuttingDown = true;
            RMUD.Core.Shutdown();
        }

        private void OutputBox_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (AfterNavigating != null)
                AfterNavigating();
            AfterNavigating = null;
        }
    }
}
