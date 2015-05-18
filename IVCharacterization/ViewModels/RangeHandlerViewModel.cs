﻿using Helper.Ranges.RangeHandlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVCharacterization
{
    public class RangeHandlerViewModel:INotifyPropertyChanged
    {
        public RangeHandlerViewModel()
        {
            m_repeatCounts = 0;
            m_rangeHandler = null;
        }
        private int m_repeatCounts;
        public int RepeatCounts {
            get { return m_repeatCounts; }
            set
            {
                SetField(ref m_repeatCounts, value, "RepeatCounts");
            }
        }

        private AbstractDoubleRangeHandler m_rangeHandler;
        public AbstractDoubleRangeHandler RangeHandler
        {
            get { return m_rangeHandler; }
            set
            {
                SetField(ref m_rangeHandler, value, "RangeHandler");
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        

        protected bool SetField<ST>(ref ST field, ST value, string propertyName)
        {
            if (EqualityComparer<ST>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}