﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Ranges
{
    [Obsolete("use DoublePropertyValueRange instead",true)]
    public class DoubleValuesRangeBase:IEnumerable<double>,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public bool SetField<T> (ref T VarToSet, T Value, string PropertyName)
        {
            if (EqualityComparer<T>.Default.Equals(VarToSet, Value))
                return false;
            VarToSet = Value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        public DoubleValuesRangeBase()
        {
            m_RangeStartValue = 0;
            m_RangeEndValue = 0;
            m_StepValue = 0;
            m_RangeWidth = 0;
            m_RangePointsCount = 0;

            m_CountDirection = 1;
            m_CountingMode = CountingModeEnum.Repetitive;
            m_CyclesNumber = 1;
            m_EnumerationInProgress = false;
        }


        //create value checker which will firstly obtain value check its valid and if it is - send to property
        // if not - make textbox red and write error...

        public void AbortEnumeration()
        {
            m_EnumerationInProgress = false;
        }

        private bool m_EnumerationInProgress;

        private double m_RangeStartValue;

        public double RangeStartValue
        {
            get { return m_RangeStartValue; }
            set {
               if(SetField(ref m_RangeStartValue,value,"RangeStart"))
               {
                   CountDirection = Math.Sign(RangeEndValue - RangeStartValue);
                   RangeWidth = Math.Abs(RangeEndValue - RangeStartValue);
                   if (StepValue != 0)
                       RangePointsCount = (int)(RangeWidth / StepValue) + 1;
                   else
                       RangePointsCount = 1;
               }
            }
        }

        private double m_RangeEndValue;

        public double RangeEndValue
        {
            get { return m_RangeEndValue; }
            set {
                if (SetField(ref m_RangeEndValue, value, "RangeEndValue"))
                {
                    CountDirection = Math.Sign(RangeEndValue - RangeStartValue);
                    RangeWidth = Math.Abs(RangeEndValue - RangeStartValue);
                    if (StepValue != 0)
                        RangePointsCount = (int)(RangeWidth / StepValue) + 1;
                    else
                        RangePointsCount = 1;
                }
            }
        }

        private double m_StepValue;

        public double StepValue
        {
            get { return m_StepValue; }
            set {
                if(SetField(ref m_StepValue,value,"StepValue"))
                {
                    if (StepValue != 0)
                        RangePointsCount = (int)(RangeWidth / StepValue) + 1;
                    else
                        RangePointsCount = 1;
                }
            }
        }

        private double m_RangeWidth;

        public double RangeWidth
        {
            get { return m_RangeWidth; }
            private set {
                SetField(ref m_RangeWidth, value, "RangeWidth");
            }
        }

        private int m_RangePointsCount;

        public int RangePointsCount
        {
            get { return m_RangePointsCount; }
            set {
                if (SetField(ref m_RangePointsCount, value, "RangePointsCount"))
                {
                    if (RangePointsCount > 1)
                        StepValue = RangeWidth / (RangePointsCount - 1);
                    else
                        StepValue = 0;
                    TotalPointsCount = RangePointsCount * CyclesNumber;
                }
            }
        }

        private int m_TotalPointsCount;

        public int TotalPointsCount
        {
            get { return m_TotalPointsCount; }
            private set {
                SetField(ref m_TotalPointsCount, value, "TotalPointsCount");
            }
        }


        private int m_CountDirection;

        public int CountDirection
        {
            get { return m_CountDirection; }
            private set {
                SetField(ref m_CountDirection, value, "CountDirection");
            }
        }

        private int m_CyclesNumber;

        public int CyclesNumber
        {
            get { return m_CyclesNumber; }
            set {
                if(SetField(ref m_CyclesNumber, value, "CyclesNumber"))
                {
                    TotalPointsCount = RangePointsCount * CyclesNumber;
                }
                
            }
        }

        private CountingModeEnum m_CountingMode;

        public CountingModeEnum CountingMode
        {
            get { return m_CountingMode; }
            set {
                SetField(ref m_CountingMode, value, "CountingMode");
            }
        }

        

        public IEnumerator<double> GetEnumerator()
        {
            m_EnumerationInProgress = true;
            int counter =0;
            double value = RangeStartValue;
            int CurrentCountDirection = CountDirection;
            int mode = (int)CountingMode*3; // to make possible distinguish between modes + count direction

            var IncrementFuncArray = new Action[6]{
                new Action(()=>{    // Repeat and -1   -- variant 0
                    if (value + CurrentCountDirection * StepValue < RangeEndValue)
                    {
                        value = RangeStartValue;
                    }
                    else
                        value+=CurrentCountDirection*StepValue;
                }), 
                new Action(()=>{     // Repeat and 0   -- variant 1
                    
                }),
                new Action(()=>{     // Repeat and 1   -- variant 2
                    if (value+CurrentCountDirection * StepValue > RangeEndValue)
                    {
                        value = RangeStartValue;
                    }
                    else
                        value += CurrentCountDirection * StepValue;
                }),
                new Action(()=>{     // Cont and -1   -- variant 3
                    if (value+CurrentCountDirection * StepValue < RangeEndValue)
                    {
                        CurrentCountDirection = -CurrentCountDirection;
                    }
                    value += CurrentCountDirection * StepValue;
                }),
                new Action(()=>{     // Cont and 0   -- variant 4

                }),
                new Action(()=>{      // Cont and 1   -- variant 5
                    if (value+CurrentCountDirection * StepValue > RangeEndValue)
                    {
                        CurrentCountDirection = -CurrentCountDirection;
                    }
                    value += CurrentCountDirection * StepValue;
                })
            };

            for (; counter<TotalPointsCount&&m_EnumerationInProgress; IncrementFuncArray[CurrentCountDirection+mode+1]())
            {
                yield return value;
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
