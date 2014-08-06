using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpGuard {

    [TemplatePart(Name = BorderNameProperty, Type = typeof(Border))]
    [TemplateVisualState(GroupName = CommonGroupName, Name = NormalStateName)]
    [TemplateVisualState(GroupName = CommonGroupName, Name = SelectedStateName)]
    public class MultipleButtonItem : ContentControl {

        private const string BorderNameProperty = "border";

        private const string CommonGroupName = "Common";
        private const string NormalStateName = "Normal";
        private const string SelectedStateName = "Selected";

        private Border _border;

        public bool IsSelected { get; set; }

        public string DisplayText { get; set; }

        private Action _deferredAction;

        public CornerRadius ItemCornerRadius {
            get { return (CornerRadius)GetValue(ItemCornerRadiusProperty); }
            set { SetValue(ItemCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ItemCornerRadiusProperty = 
            DependencyProperty.Register("ItemCornerRadius", typeof(CornerRadius), typeof(MultipleButtonItem), null);

        private static void OnItemCornerRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { 
          
        }

        public SolidColorBrush NormalStateColor {
            get { return (SolidColorBrush)GetValue(NormalStateColorProperty); }
            set { SetValue(NormalStateColorProperty, value); }
        }

        public static readonly DependencyProperty NormalStateColorProperty = DependencyProperty.Register("NormalStateColor", typeof(SolidColorBrush), typeof(MultipleButtonItem), null);

        public SolidColorBrush SelectedStateColor {
            get { return (SolidColorBrush)GetValue(SelectedStateColorProperty); }
            set { SetValue(SelectedStateColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedStateColorProperty = DependencyProperty.Register("SelectedStateColor", typeof(SolidColorBrush), typeof(MultipleButtonItem), null);

        /// <summary>
        /// The states that this can be in.
        /// </summary>
        internal enum State {
            /// <summary>
            /// Unselected
            /// </summary>
            Normal,
            /// <summary>
            /// Selected
            /// </summary>
            Selected
        };
        private State _state;

        public MultipleButtonItem() {
            DefaultStyleKey = typeof(MultipleButtonItem);
        }

        /// <summary>
        /// Put this item into a new state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="useTransitions">Flag indicating that transitions should be used when going to the new state.</param>
        internal void SetState(State newState, bool useTransitions) {

            _deferredAction = new Action(() => {
                if (_state != newState) {
                    _state = newState;
                    switch (_state) {
                        case State.Normal:
                            VisualStateManager.GoToState(this, NormalStateName, useTransitions);
                            break;
                        case State.Selected:
                            VisualStateManager.GoToState(this, SelectedStateName, useTransitions);
                            break;
                    }
                }
            });
            if (Template != null)
                _deferredAction();

        }

        /// <summary>
        /// Returns the current state.
        /// </summary>
        /// <returns>The current state.</returns>
        internal State GetState() { return _state; }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _border = GetTemplateChild(BorderNameProperty) as Border;
            _border.Background = this.Background;

            if (_deferredAction != null) {
                _deferredAction();
                _deferredAction = null;
            }

        }

    }
}
