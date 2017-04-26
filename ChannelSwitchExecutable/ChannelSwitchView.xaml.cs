﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChannelSwitchExecutable
{
    public class ControlFinder
    {
        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }

    /// <summary>
    /// Interaction logic for ChannelSwitchView.xaml
    /// </summary>
    public partial class ChannelSwitchView : UserControl
    {
        public ChannelSwitchView()
        {
            InitializeComponent();


            _defaultBrush = (SolidColorBrush)Resources["DefaultBrush"];
            _OnBrush = Brushes.Green;
            DataContextChanged += ChannelSwitchView_DataContextChanged;
        }
        private Button _currentButton;
        private SolidColorBrush _defaultBrush;
        private SolidColorBrush _OnBrush;
        private const int MAX_CHANNELS = 32;
        private object syncRoot = new object();

        void ChannelSwitchView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var newDataContext = e.NewValue as ChannelSwitchExecutableViewModel;
            if(newDataContext!= null)
            {
                newDataContext.ChannelSwitched += ChannelSwitched;
            }
        }

        private void ChannelSwitched(object sender, ChannelSwitchEventArgs e)
        {
            var button = e.PressedButton as Button;
            if (button == null)
            {
                button = ControlFinder.FindChild<Button>(this, String.Format("Button_{0}", e.SelectedChannel));
                if (button == null)
                    throw new Exception("Refferenced object is not button");
            }
            
            lock (syncRoot)
            {
                if (Object.ReferenceEquals(button, _currentButton))
                {
                    SwitchOffBackground(button);
                    _currentButton = null;
                }
                else
                {
                    if (_currentButton != null)
                    {
                        SwitchOffBackground(_currentButton);
                    }
                    _currentButton = button;
                    SwitchOnBackground(_currentButton);
                }
            }
            
        }

        private void SwitchOnBackground(Button sender)
        {
            sender.Background = _OnBrush;
        }
        private void SwitchOffBackground(Button sender)
        {
            sender.Background = _defaultBrush;
        }
      
  

    }
}