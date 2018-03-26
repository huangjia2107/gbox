using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Resources.Controls
{
    public class ExpandPanel : Panel
    {
        private double totalDesiredHeight;
        private bool isAnimating;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!isAnimating)
            {
                double yOffset = 0;
                isAnimating = true;
                foreach (ListBoxItem child in Children)
                {
                    double finalHeight = child.IsSelected ? child.DesiredSize.Height + finalSize.Height - totalDesiredHeight :
                        child.DesiredSize.Height;

                    child.Arrange(new Rect(0, yOffset, finalSize.Width, finalHeight));
                    yOffset += finalHeight;
                }
                isAnimating = false;
            }
            else
            {
                double yOffset = 0;
                foreach (ListBoxItem child in Children)
                {
                    child.Arrange(new Rect(0, yOffset, finalSize.Width, child.DesiredSize.Height));
                    yOffset += child.DesiredSize.Height;
                }
            }


            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {

            totalDesiredHeight = 0;
            foreach (ListBoxItem child in Children)
            {
                child.Measure(availableSize);

                totalDesiredHeight += child.DesiredSize.Height;
            }

            return base.MeasureOverride(availableSize);
        }
    }
}
