using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Coder.Helper;
using ZoDream.Coder.Helper.Local;
using ZoDream.Coder.Model;

namespace ZoDream.Coder.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string _file;

        private NotificationMessageAction<string> _addText;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction<string>>(this, m => _addText = m);
            _load();
        }

        private void _load()
        {
            Kinds = LocalHelper.GetAllFileName(AppDomain.CurrentDomain.BaseDirectory + "\\Codes").ToArray();
            _loadCode(Kinds.GetValue(0).ToString());
        }

        private void _loadCode(string name)
        {
            CodeLists.Clear();
            Task.Factory.StartNew(() =>
            {
                var list = XmlHelper.GetXmlNodeListByXpath(AppDomain.CurrentDomain.BaseDirectory + $"\\Codes\\{name}.xml",
                "/codes/code");
                foreach (XmlNode item in list)
                {
                    var code = new CodeItem();
                    foreach (XmlNode node in item.ChildNodes)
                    {
                        if (node.Name == "name")
                        {
                            code.Name = node.InnerText;
                        }
                        else if (node.Name == "description")
                        {
                            code.Descroption = node.InnerText;
                        }
                        else if (node.Name == "content")
                        {
                            code.Content = node.InnerText;
                        }
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CodeLists.Add(code);
                    });
                }
            });
        }

        /// <summary>
        /// The <see cref="CodeLists" /> property's name.
        /// </summary>
        public const string CodeListsPropertyName = "CodeLists";

        private ObservableCollection<CodeItem> _codeLists = new ObservableCollection<CodeItem>();

        /// <summary>
        /// Sets and gets the CodeLists property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<CodeItem> CodeLists
        {
            get
            {
                return _codeLists;
            }
            set
            {
                Set(CodeListsPropertyName, ref _codeLists, value);
            }
        }



        /// <summary>
        /// The <see cref="Content" /> property's name.
        /// </summary>
        public const string ContentPropertyName = "Content";

        private string _content = string.Empty;

        /// <summary>
        /// Sets and gets the Content property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                Set(ContentPropertyName, ref _content, value);
            }
        }

        /// <summary>
        /// The <see cref="Kinds" /> property's name.
        /// </summary>
        public const string KindsPropertyName = "Kinds";

        private Array _kinds;

        /// <summary>
        /// Sets and gets the Kinds property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Array Kinds
        {
            get
            {
                return _kinds;
            }
            set
            {
                Set(KindsPropertyName, ref _kinds, value);
            }
        }



        private RelayCommand _openCommand;

        /// <summary>
        /// Gets the OpenCommand.
        /// </summary>
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand
                    ?? (_openCommand = new RelayCommand(ExecuteOpenCommand));
            }
        }

        private void ExecuteOpenCommand()
        {
            _file = LocalHelper.ChooseFile();
            Content = LocalHelper.GetText(_file);
        }

        private RelayCommand _openFolderCommand;

        /// <summary>
        /// Gets the OpenFolderCommand.
        /// </summary>
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand
                    ?? (_openFolderCommand = new RelayCommand(ExecuteOpenFolderCommand));
            }
        }

        private void ExecuteOpenFolderCommand()
        {
            
        }

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            if (string.IsNullOrEmpty(_file))
            {
                _file = LocalHelper.ChooseSaveFile("新建文本");
            }
            if (string.IsNullOrEmpty(_file)) return;
            LocalHelper.SaveFile(Content, _file);
        }

        private RelayCommand _refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(ExecuteRefreshCommand));
            }
        }

        private void ExecuteRefreshCommand()
        {
            if (string.IsNullOrEmpty(_file)) return;
            Content = LocalHelper.GetText(_file);
        }

        private RelayCommand _keyboardCommand;

        /// <summary>
        /// Gets the KeyboardCommand.
        /// </summary>
        public RelayCommand KeyboardCommand
        {
            get
            {
                return _keyboardCommand
                    ?? (_keyboardCommand = new RelayCommand(ExecuteKeyboardCommand));
            }
        }

        private void ExecuteKeyboardCommand()
        {
            const string file = "C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
            if (!File.Exists(file)) return;
            Process.Start(file);
        }

        private RelayCommand _editCommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand
                    ?? (_editCommand = new RelayCommand(ExecuteEditCommand));
            }
        }

        private void ExecuteEditCommand()
        {
            var file = "C:\\Program Files\\Notepad++\\notepad++.exe";
            if (!File.Exists(file)) return;
            Process.Start(file, "c:\\1.txt");
        }

        private RelayCommand<DragEventArgs> _dropCommand;

        /// <summary>
        /// Gets the DropCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> DropCommand
        {
            get
            {
                return _dropCommand
                    ?? (_dropCommand = new RelayCommand<DragEventArgs>(ExecuteDropCommand));
            }
        }

        private void ExecuteDropCommand(DragEventArgs parameter)
        {
            var item = (CodeItem)parameter.Data.GetData(typeof (CodeItem));
            _addText.Execute(item.Content);
        }

        private RelayCommand<object> _drapCommand;

        /// <summary>
        /// Gets the DrapCommand.
        /// </summary>
        public RelayCommand<object> DrapCommand
        {
            get
            {
                return _drapCommand
                    ?? (_drapCommand = new RelayCommand<object>(ExecuteDrapCommand));
            }
        }

        private void ExecuteDrapCommand(object sender)
        {
            var list = (ListBox) sender;
            if (list.SelectedIndex < 0 || list.SelectedIndex >= CodeLists.Count) return;
            DragDrop.DoDragDrop(list, new DataObject(typeof(CodeItem), list.SelectedItem), DragDropEffects.Copy);
        }

        private RelayCommand<string> _changedCommand;

        /// <summary>
        /// Gets the ChangedCommand.
        /// </summary>
        public RelayCommand<string> ChangedCommand
        {
            get
            {
                return _changedCommand
                    ?? (_changedCommand = new RelayCommand<string>(ExecuteChangedCommand));
            }
        }

        private void ExecuteChangedCommand(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _loadCode(name);
        }

        private RelayCommand<string> _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand<string> AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand<string>(ExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            new AddView().Show();
            Messenger.Default.Send(new NotificationMessageAction(name, ()=> {}), "add");
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}