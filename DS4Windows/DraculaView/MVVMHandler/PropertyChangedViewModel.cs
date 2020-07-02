﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace DS4WinWPF.DraculaView.MVVMHandler
{
    public class PropertyChangedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
