using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace WPF_LoggerTray.ViewModels
{
    public class DialogInputViewModel : Conductor<object>
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value;
                NotifyOfPropertyChange();
            }
        }
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; NotifyOfPropertyChange(); }
        }
        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; NotifyOfPropertyChange(); }
        }
        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; NotifyOfPropertyChange(); }
        }

        public bool acceptValue { get; set; } = false;

        public DialogInputViewModel(string title, string text, string value,int width=200)
        {
            Title = title;
            Text = text;
            Value = value;
            Width = width;
        }

        public void ButtonOk()
        {
            acceptValue = true;
            this.TryCloseAsync();
        }
    }
}
