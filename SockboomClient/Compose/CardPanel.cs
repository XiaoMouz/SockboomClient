using Microsoft.UI.Xaml;
using Windows.UI.Xaml.Markup;
using System;
using Control = Microsoft.UI.Xaml.Controls.Control;

namespace SockboomClient.Compose
{
    /// <summary>
    /// DO NOT USE THIS COMPOSE, BUG
    /// todo: fix
    /// </summary>
    [ContentProperty(Name = nameof(Content))]
    public sealed class CardPanel : Control
    {
        public CardPanel()
        {
            this.DefaultStyleKey = typeof(CardPanel);
        }

        #region 标题
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(CardPanel),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnTitlechanged))
            );

        public bool HasTitleValue { get; set; }
        private static void OnTitlechanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardPanel titleControl = d as CardPanel; //null checks omitted
            String s = e.NewValue as String; //null checks omitted
            if (s == String.Empty)
            {
                titleControl.HasTitleValue = false;
            }
            else
            {
                titleControl.HasTitleValue = true;
            }
        }
        #endregion




        #region 子元素
        [System.ComponentModel.Bindable(true)]
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content), 
            typeof(object), 
            typeof(CardPanel), 
            null
            );
        #endregion
    }
}
