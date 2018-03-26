using System.Windows;
using System.Windows.Controls;

namespace Resources.Controls
{
    /// <summary>
    /// The logic is very simple. Make the largest child fill.
    /// </summary>
    public class GroupPanel : Panel
    {
        private UIElement largestChild;
        private double totalHeight;

        protected override Size MeasureOverride(Size availableSize)
        {
            totalHeight = 0;
            double width = 0;

            largestChild = null;
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);

                if (largestChild == null || child.DesiredSize.Height >= largestChild.DesiredSize.Height)
                {
                    largestChild = child;
                }

                totalHeight += child.DesiredSize.Height;
                if (child.DesiredSize.Width > width)
                {
                    width = child.DesiredSize.Width;
                }
            }

            return new Size(width, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double yOffset = 0;
            foreach (UIElement child in Children)
            {
                if (child == largestChild)
                {
                    double finalHeight = child.DesiredSize.Height + finalSize.Height - totalHeight;
                    child.Arrange(new Rect(0, yOffset, finalSize.Width, finalHeight));
                    yOffset += finalHeight;
                }
                else
                {
                    child.Arrange(new Rect(0, yOffset, finalSize.Width, child.DesiredSize.Height));
                    yOffset += child.DesiredSize.Height;
                }
            }

            return finalSize;
        }
    }
}
