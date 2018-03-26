using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Resources.Behaviors
{
    public class SelectedItemFillBehavior : Behavior<ListBox>
    {
        // Using a DependencyProperty as the backing store for DefaultHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultHeightProperty =
            DependencyProperty.Register("DefaultHeight", typeof(double), typeof(SelectedItemFillBehavior), new UIPropertyMetadata(30.0));

        // Using a DependencyProperty as the backing store for AnimationDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(int), typeof(SelectedItemFillBehavior), new UIPropertyMetadata(300));

        public double DefaultHeight
        {
            get { return (double)GetValue(DefaultHeightProperty); }
            set { SetValue(DefaultHeightProperty, value); }
        }
        
        public int AnimationDuration
        {
            get { return (int)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(OnAssociatedObjectSelectionChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(OnAssociatedObjectSelectionChanged);
        }

        private void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double selectedItemFinalHeight = AssociatedObject.ActualHeight;

            Storyboard storyBoard = new Storyboard();

            for (int i = 0; i < AssociatedObject.Items.Count; i++)
            {
                ListBoxItem item = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (!item.IsSelected)
                {
                    selectedItemFinalHeight -= DefaultHeight;

                    DoubleAnimation heightAnimation = new DoubleAnimation()
                    {
                        To = DefaultHeight,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, AnimationDuration))
                    };
                    Storyboard.SetTarget(heightAnimation, item);
                    Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(FrameworkElement.HeightProperty));
                    storyBoard.Children.Add(heightAnimation);
                }
            }

            selectedItemFinalHeight -= 4;

            if (AssociatedObject.SelectedIndex >= 0)
            {
                ListBoxItem selectedItem = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(AssociatedObject.SelectedIndex) as ListBoxItem;

                DoubleAnimation fillheightAnimation = new DoubleAnimation()
                {
                    To = selectedItemFinalHeight,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, AnimationDuration))
                };

                Storyboard.SetTarget(fillheightAnimation, selectedItem);
                Storyboard.SetTargetProperty(fillheightAnimation, new PropertyPath(FrameworkElement.HeightProperty));
                storyBoard.Children.Add(fillheightAnimation);
            }

            storyBoard.Begin(AssociatedObject);
        }
    }
}
