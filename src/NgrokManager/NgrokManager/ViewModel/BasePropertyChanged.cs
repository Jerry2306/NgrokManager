using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NgrokManager.ViewModel
{
    public class BasePropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Put this function in the 'set' of your property to raise PropertyChangedEvent manually
        /// </summary>
        /// <param name="source"></param>
        public void OnPropertyChanged([CallerMemberName]string source = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(source));

        /// <summary>
        /// Put this function in the 'set' of your property to set value and raise PropertyChangedEvent manually
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="property">Your property to set</param>
        /// <param name="value">The value your property is being set to</param>
        /// <param name="source">The 'CallerMemberName'</param>
        public void SetProperty<T>(ref T property, T value, [CallerMemberName]string source = "") => SetProperty(ref property, value, true, source);

        /// <summary>
        /// Put this function in the 'set' of your property and decide wether you wan't to invoke PropertyChangedEvent or not
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="property">Your property to set</param>
        /// <param name="value">The value your property is being set to</param>
        /// <param name="invokePropertyChanged">Determines whether PropertyChangedEvent should raise or not</param>
        /// <param name="source">The 'CallerMemberName'</param>
        public void SetProperty<T>(ref T property, T value, bool invokePropertyChanged, [CallerMemberName]string source = "")
        {
            property = value;

            if (invokePropertyChanged) OnPropertyChanged(source);
        }
    }
}
