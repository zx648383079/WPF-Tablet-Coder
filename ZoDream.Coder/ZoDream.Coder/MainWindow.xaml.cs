using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Coder.ViewModel;

namespace ZoDream.Coder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Send(new NotificationMessageAction<string>(null, _addText));
        }

        private void _addText(string text)
        {
            TextEditor.Focus();
            TextEditor.SelectedText = text;
        }


        private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.Focus();
            TextEditor.SelectAll();
        }

        private void TabForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            var line = TextEditor.GetLineIndexFromCharacterIndex(TextEditor.SelectionStart);
            var line1 = TextEditor.GetLineIndexFromCharacterIndex(TextEditor.SelectionStart + TextEditor.SelectionLength);
            for (var i = line; i <= line1; i++)
            {
                TextEditor.SelectionStart = TextEditor.GetCharacterIndexFromLineIndex(i);
                TextEditor.SelectionLength = 0;
                TextEditor.SelectedText = "\t";
            }
            TextEditor.Focus();
            var start = TextEditor.GetCharacterIndexFromLineIndex(line);
            TextEditor.Select(start, (line1 + 1 >= TextEditor.LineCount) ? TextEditor.Text.Length : TextEditor.GetCharacterIndexFromLineIndex(line1 + 1) - start);
        }

        private void ZoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextEditor.FontSize < 10)
            {
                ZoomOutBtn.IsEnabled = false;
                return;
            }
            if (!ZoomInBtn.IsEnabled)
            {
                ZoomInBtn.IsEnabled = true;
            }
            TextEditor.FontSize -= 5;
        }

        private void ZoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextEditor.FontSize > 150)
            {
                ZoomInBtn.IsEnabled = false;
                return;
            }
            if (!ZoomOutBtn.IsEnabled)
            {
                ZoomOutBtn.IsEnabled = true;
            }
            TextEditor.FontSize += 5;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextEditor.SelectionLength > 0)
            {
                TextEditor.SelectedText = "";
            }
            else
            {
                TextEditor.Clear();
            }
        }

        private void TabBackwardBtn_Click(object sender, RoutedEventArgs e)
        {
            var line = TextEditor.GetLineIndexFromCharacterIndex(TextEditor.SelectionStart);
            var line1 = TextEditor.GetLineIndexFromCharacterIndex(TextEditor.SelectionStart + TextEditor.SelectionLength);
            for (var i = line; i <= line1; i++)
            {
                TextEditor.Select(TextEditor.GetCharacterIndexFromLineIndex(i), 1);
                if (TextEditor.SelectedText == "\t")
                {
                    TextEditor.SelectedText = "";
                }
            }
            TextEditor.Focus();
            var start = TextEditor.GetCharacterIndexFromLineIndex(line);
            TextEditor.Select(start, (line1 + 1 >= TextEditor.LineCount) ? TextEditor.Text.Length : TextEditor.GetCharacterIndexFromLineIndex(line1 + 1) - start);
        }

        private void EnterLineBreakBtn_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.SelectedText = "\n";
        }

        private void TextEditor_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private TouchPoint _startPoint;

        private void TextEditor_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            _startPoint = e.GetTouchPoint(TextEditor);
        }

        private void TextEditor_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            var endPoint = e.GetTouchPoint(TextEditor);
            int start, length;
            if (endPoint.Position.Y > _startPoint.Position.Y)
            {
                start = TextEditor.GetCharacterIndexFromPoint(_startPoint.Position, true);
                length = TextEditor.GetCharacterIndexFromPoint(endPoint.Position, true) - start;
            }
            else
            {
                start = TextEditor.GetCharacterIndexFromPoint(endPoint.Position, true);
                length = TextEditor.GetCharacterIndexFromPoint(_startPoint.Position, true) - start + 1;
                
            }
            TextEditor.Select(start, length);
        }
    }
}