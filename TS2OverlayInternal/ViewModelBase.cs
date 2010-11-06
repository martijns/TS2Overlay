using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TS2OverlayCore
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var eventCopy = PropertyChanged;
            if (eventCopy == null)
                return;

            eventCopy(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
