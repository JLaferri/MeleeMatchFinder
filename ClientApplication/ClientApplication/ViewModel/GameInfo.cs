using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ContractObjects;
using ClientApplication.Common;

namespace ClientApplication.ViewModel
{
    public class GameInfo : INotifyPropertyChanged
    {
        private SynchronizedGame _game;
        public SynchronizedGame Game { get { return _game; } set { this.RaiseAndSetIfChanged("Game", ref _game, value, PropertyChanged); } }

        public GameInfo(SynchronizedGame game)
        {
            Game = game;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
