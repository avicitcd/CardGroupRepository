using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CardWorkbench.Utils
{

    public class DynamicBindingList : ObservableCollection<DynamicDictionary>
    {
    }
    public class DynamicDictionary : DynamicObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region inner classes
        class SetMemberValueBinder : SetMemberBinder
        {
            public SetMemberValueBinder(string propertyName)
                : base(propertyName, false)
            {
            }
            public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
            {
                return errorSuggestion;
            }
        }
        class GetMemberValueBinder : GetMemberBinder
        {
            public GetMemberValueBinder(string propertyName)
                : base(propertyName, false)
            {
            }
            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                return errorSuggestion;
            }
        }
        #endregion

        public DynamicDictionary()
        {
            //SetValue("Id", 0);
            //SetValue("FirstName", "");
            //SetValue("LastName", "");
        }
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        public int Count { get { return dictionary.Count; } }
        public void SetValue(string propertyName, object value)
        {
            TrySetMember(new SetMemberValueBinder(propertyName), value);
        }
        public object GetValue(string propertyName)
        {
            object value = null;
            TryGetMember(new GetMemberValueBinder(propertyName), out value);
            return value;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name.ToLower()] = value;
            NotifyPropertyChanged(binder.Name);
            return true;
        }
        void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
    public class StringStateToBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !string.IsNullOrEmpty((string)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NewColumnModel : BindableBase
    {
        string columnNameCore;
        int typeIndexCore = 1;

        public string ColumnName
        {
            get { return columnNameCore; }
            set { SetProperty(ref columnNameCore, value, "ColumnName"); }
        }
        public int TypeIndex
        {
            get { return typeIndexCore; }
            set { SetProperty(ref typeIndexCore, value, "TypeIndex"); }
        }
    }
}
