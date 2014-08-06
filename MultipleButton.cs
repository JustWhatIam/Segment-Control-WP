using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Markup;

namespace OpGuard {

    [TemplatePart(Name = ItemsPanelHostProperty, Type = typeof(Panel))]
    [TemplatePart(Name = ItemsPresenterProperty, Type = typeof(ItemsPresenter))]
    public class MultipleButton : ItemsControl {

        private const string ItemsPanelHostProperty = "ItemsPanelHost";
        private const string ItemsPresenterProperty = "ItemsPresenter";

        private const string _radioGroupName = "ItemGroup";

        private Panel _itemsPanel;
        private ItemsPresenter _itemsPresenter;

        public CornerRadius LRCornerRadius { get; set; }

        public SolidColorBrush SelecetedBackground { get; set; }

        private object _deferredSelectedItem;

        /// <summary>
        /// Event that is raised when the selection changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(MultipleButton), new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((MultipleButton)o).OnSelectedItemChanged(e.OldValue, e.NewValue);
        }

        private void OnSelectedItemChanged(object oldValue, object newValue) {
            if (Template == null) {
                _deferredSelectedItem = newValue;
                return;
            }

            // Fire SelectionChanged event
            var handler = SelectionChanged;
            if (null != handler) {
                IList removedItems = (null == oldValue) ? new object[0] : new object[] { oldValue };
                IList addedItems = (null == newValue) ? new object[0] : new object[] { newValue };
                handler(this, new SelectionChangedEventArgs(removedItems, addedItems));
            }
        }

        /// <summary>
        /// Gets or sets the header of the control.
        /// </summary>
        public object Header {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the Header DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(MultipleButton), null);


        public ControlTemplate MultiButtonItemTemplate {
            get { return (ControlTemplate)GetValue(MultiButtonItemTemplateProperty); }
            set { SetValue(MultiButtonItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty MultiButtonItemTemplateProperty = DependencyProperty.Register("MultipleButtonItem", typeof(ControlTemplate), typeof(MultipleButton), null);
            
        public MultipleButton() {
            DefaultStyleKey = typeof(MultipleButton);
            this.IsEnabledChanged += MultipleButton_IsEnabledChanged;

        }

        void MultipleButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
            UpdateVisualStates(true);
        }

        private void UpdateVisualStates(bool useTransitions) {
            if (IsEnabled)
                VisualStateManager.GoToState(this, "Normal", true);
            else
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _itemsPanel = GetTemplateChild(ItemsPanelHostProperty) as Panel;
            _itemsPresenter = GetTemplateChild(ItemsPresenterProperty) as ItemsPresenter;
            _itemsPanel.Background = new SolidColorBrush(Colors.Brown);
            UpdateItemTemplate();
            UpdateVisualStates(true);
            if (null != _deferredSelectedItem) {
                ((_deferredSelectedItem as MultipleButtonItem).Content as RadioButton).IsChecked = true;
                _deferredSelectedItem = null;

                //UpdateMenuItem(SelectedItem as MultipleButtonItem, true);
            }
        }


        /// <summary>
        /// The data source that the this control is the view for.
        /// </summary>
        public IList<MultipleButtonItem> DataSource {
            get { return (IList<MultipleButtonItem>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        /// <summary>
        /// The DataSource DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IList<MultipleButtonItem>), typeof(MultipleButton), new PropertyMetadata(null, OnDataSourceChanged));

        private static void OnDataSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            MultipleButton picker = (MultipleButton)obj;

            //if (e.OldValue != null) {
            //    ((ILoopingSelectorDataSource)e.OldValue).SelectionChanged -= picker.OnDataSourceSelectionChanged;
            //}

            //if (e.NewValue != null) {
            //    ((ILoopingSelectorDataSource)e.NewValue).SelectionChanged += picker.OnDataSourceSelectionChanged;
            //}
            picker.UpdateData();
        }

        private void UpdateData() {
            if (DataSource != null) {
                Items.Clear();
                for (int i = 0; i < DataSource.Count; i++) {
                    
                    DataSource[i].NormalStateColor = Background as SolidColorBrush;
                    DataSource[i].SelectedStateColor = SelecetedBackground;

                    RadioButton radioButton = new RadioButton() { Content = DataSource[i].DisplayText };
                    radioButton.GroupName = _radioGroupName + this.GetHashCode();
                    AddHandler(radioButton);

                    radioButton.Background = SelecetedBackground;
                    radioButton.Foreground = Background as SolidColorBrush;

                    DataSource[i].Content = radioButton;
                    DataSource[i].Background = this.Background;
                    DataSource[i].BorderBrush = this.BorderBrush;
                    
                    if (DataSource.Count == 1) {
                        DataSource[i].ItemCornerRadius = LRCornerRadius;
                        DataSource[i].BorderThickness = BorderThickness;
                    }
                    else {
                        if (i == 0) {
                            DataSource[i].ItemCornerRadius = new CornerRadius(this.LRCornerRadius.TopLeft, 0, 0, this.LRCornerRadius.BottomLeft);
                            DataSource[i].BorderThickness = this.BorderThickness;
                        }
                        else {
                            DataSource[i].BorderThickness = new Thickness(0, this.BorderThickness.Top, this.BorderThickness.Right, this.BorderThickness.Bottom);
                            if (i + 1 == DataSource.Count)
                                DataSource[i].ItemCornerRadius = new CornerRadius(0, this.LRCornerRadius.TopRight, this.LRCornerRadius.BottomRight, 0);
                            
                        }
                    }
                    Items.Add(DataSource[i]);
                }
            }
        }

        private void UpdateItemTemplate() {

            foreach (object obj in Items) {
                RadioButton radioButton = (obj as MultipleButtonItem).Content as RadioButton;
                if (MultiButtonItemTemplate != null) {
                    radioButton.Template = MultiButtonItemTemplate;
                }
            }
        }

        private void AddHandler(RadioButton radioButton) {
            radioButton.Checked += radioButton_Checked;
            radioButton.Unchecked += radioButton_Unchecked;
        }

        void radioButton_Unchecked(object sender, RoutedEventArgs e) {
            UpdateMenuItem((e.OriginalSource as RadioButton).Parent as MultipleButtonItem, false);
        }

        void radioButton_Checked(object sender, RoutedEventArgs e) {
            UpdateMenuItem((e.OriginalSource as RadioButton).Parent as MultipleButtonItem, true);
            SelectedItem = (e.OriginalSource as RadioButton).Parent;
        }

        private void UpdateMenuItem(MultipleButtonItem item, bool isSelected) {
            if (item != null) {
                int index = DataSource.IndexOf(item);
                MultipleButtonItem.State state = isSelected ? MultipleButtonItem.State.Selected : MultipleButtonItem.State.Normal;
                item.SetState(state, true);
                
            }
        }
    }
}
