using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace ToolClass.StoryBoard
{
    /// <summary>
    /// adapted from
    /// http://chris.59north.com/post/mvvm-and-animations.aspx
    /// </summary>
    public static class StoryboardManager
    {
        public static DependencyProperty IDProperty =
            DependencyProperty.RegisterAttached(
                "ID",
                typeof(string),
                typeof(StoryboardManager),
                new PropertyMetadata(null, OnIDChanged));

        class StoryboardInfo
        {
            public Storyboard Storyboard { get; set; }
            public Action Callback { get; set; }
        }

        static Dictionary<string, StoryboardInfo> _storyboards = new Dictionary<string, StoryboardInfo>();

        static void OnIDChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Storyboard sb = obj as Storyboard;
            if (sb == null)
                return;

            string key = e.NewValue as string;
            if (_storyboards.ContainsKey(key))
            {
                // very strange, WPF currently making another instance of the storyboard here
                // but we've already started a different instance, so ignore
                return;
            }
            sb.Completed += delegate(object sender, EventArgs args) { _storyboards[key].Callback(); };
            _storyboards[key] = new StoryboardInfo() { Storyboard = sb, Callback = null };
        }

        public static void PlayStoryboard(string id, Callback callback, object state)
        {
            if (!_storyboards.ContainsKey(id))
            {
                callback(state);
                return;
            }
            var sb = _storyboards[id];
            sb.Callback = () => callback(state);
            sb.Storyboard.Begin();
        }

        public static void StopStoryboard(string id)
        {
            if (!_storyboards.ContainsKey(id))
            {
                return;
            }
            var sb = _storyboards[id];
            sb.Callback =null;
            sb.Storyboard.Stop();
        }

        public static void SetID(DependencyObject target, string id)
        {
            target.SetValue(IDProperty, id);
        }

        public static string GetID(DependencyObject target)
        {
            return target.GetValue(IDProperty) as string;
        }
    }
    public delegate void Callback(object state);


}
