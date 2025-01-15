using System;
using Noesis;

namespace Caliburn.Noesis
{
    /// <summary>
    /// An <see cref="AdornerDecorator"/> which allows to overlay elements with a <see cref="DialogHost"/>.
    /// The additional element will be rendered on top of the regular <see cref="AdornerLayer"/>.
    /// </summary>
    public class DialogAdornerDecorator : AdornerDecorator
    {
        private readonly DialogHost _dialogHost = new();
        private readonly AdornerLayer _adornerLayer;

        private UIElement _child;

        public DialogAdornerDecorator()
        {
            _adornerLayer = GetAdornerLayer();
        }

        public DialogAdornerDecorator(bool logicalChild) : base(logicalChild)
        {
            _adornerLayer = GetAdornerLayer();
        }

        /// <summary>Gets or sets the child of the AdornerDecorator.</summary>
        public new UIElement Child
        {
            get => _child;
            set
            {
                if (ReferenceEquals(_child, value))
                {
                    return;
                }

                if (_child is not null && value is null)
                {
                    RemoveOverlays();
                }
                else if (_child is null && value is not null)
                {
                    AddOverlays();
                }

                _child = value;
            }
        }

        /// <summary>Gets the <see cref="Visual"/> children count.</summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return Child is null ? 0 : 3;
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var finalSizeRect = new Rect(finalSize);
            Child.Arrange(finalSizeRect);

            if (VisualTreeHelper.GetParent(_adornerLayer) != null)
            {
                _adornerLayer.Arrange(finalSizeRect);
            }

            _dialogHost.Arrange(finalSizeRect);

            return finalSize;
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            if (Child == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            switch (index)
            {
                case 0:
                    return Child;

                case 1:
                    return _adornerLayer;

                case 2:
                    return _dialogHost;

                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            var desiredSize = new Size();

            if (Child != null)
            {
                Child.Measure(constraint);
                desiredSize = Child.DesiredSize;
            }

            if (VisualTreeHelper.GetParent(_adornerLayer) is not null)
            {
                _adornerLayer.Measure(constraint);
            }

            _dialogHost.Measure(constraint);

            return desiredSize;
        }

        private void AddOverlays()
        {
            AddVisualChild(_dialogHost);
        }

        private void RemoveOverlays()
        {
            RemoveVisualChild(_dialogHost);
        }
    }
}
