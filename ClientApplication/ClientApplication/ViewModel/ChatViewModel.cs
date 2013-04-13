using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using ClientApplication.Common;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ClientApplication.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private const int MESSAGE_LIMIT = 50;

        public ObservableCollection<string> MessageCollection { get; private set; }
        public ICollectionView ChatView { get; private set; }

        private string _pendingInput = string.Empty;
        public string PendingInput { get { return _pendingInput; } set { this.RaiseAndSetIfChanged("PendingInput", ref _pendingInput, value, PropertyChanged); } }

        public ICommand SendMessage { get; private set; }

        public ChatViewModel(Action<string> sendMessageAction)
        {
            MessageCollection = new ObservableCollection<string>();

            var cappedMessageCollection = new ObservableCollection<string>();
            MessageCollection.CollectionChanged += (sender, e) =>
            {
                //Collection will only ever be changed when something is added
                cappedMessageCollection.Add((string)e.NewItems[0]);

                //If capped collection now has more items than allowed by limit, 
                if (cappedMessageCollection.Count > MESSAGE_LIMIT) cappedMessageCollection.RemoveAt(0);
            };

            ChatView = CollectionViewSource.GetDefaultView(cappedMessageCollection);

            //Create send message action
            var sendMessageAsync = Command.Create(() => PendingInput.Length > 0, () =>
            {
                sendMessageAction(PendingInput);
                PendingInput = string.Empty;
            });

            //Requery the button CanExecute when PendingInput changes
            this.PropertyChanged += (sender, e) => sendMessageAsync.RequeryCanExecute();

            //Allow for binding to command
            SendMessage = sendMessageAsync;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
