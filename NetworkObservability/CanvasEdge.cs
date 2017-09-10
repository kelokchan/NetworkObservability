using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using NetworkObservabilityCore;

namespace NetworkObservability
{
    public sealed class CanvasEdge : Shape
    {
        internal IEdge Impl
		{
			get;
			set;
		}

        public CanvasEdge(bool fake = false) : base()
        {
            if (!fake)
            {
                Impl = new Edge();
            }
            else
            {
                Impl = new ResultEdge();
            }
        }

        #region Dependency Properties

        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(CanvasEdge), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(CanvasEdge), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(CanvasEdge), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(CanvasEdge), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(CanvasEdge), new FrameworkPropertyMetadata(7.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(CanvasEdge), new FrameworkPropertyMetadata(7.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty IsDirectedProperty = DependencyProperty.Register("IsDirected", typeof(Boolean), typeof(CanvasEdge), new PropertyMetadata(true));
       
        #endregion

        private bool isSelected;

        #region CLR Properties

        public Boolean IsDirected
        {
            get { return (Boolean)this.GetValue(IsDirectedProperty); }
            set { this.SetValue(IsDirectedProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get { return (double)base.GetValue(X1Property); }
            set { base.SetValue(X1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get { return (double)base.GetValue(Y1Property); }
            set { base.SetValue(Y1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get { return (double)base.GetValue(X2Property); }
            set { base.SetValue(X2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get { return (double)base.GetValue(Y2Property); }
            set { base.SetValue(Y2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadWidth
        {
            get { return (double)base.GetValue(HeadWidthProperty); }
            set { base.SetValue(HeadWidthProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadHeight
        {
            get { return (double)base.GetValue(HeadHeightProperty); }
            set { base.SetValue(HeadHeightProperty, value); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                this.Stroke = isSelected ? Brushes.Green : Brushes.DarkGray;
            }
        }

        #endregion

        #region Overrides

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    InternalDrawArrowGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        #endregion

        #region Privates

        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt1 = new Point(X1, this.Y1);
            Point pt2 = new Point(X2, this.Y2);

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);

            if (IsDirected)
            {
                double mid_X = (X2 + X1) / 2;
                double mid_Y = (Y2 + Y1) / 2;

                Point mid_point = new Point(mid_X, mid_Y);

                Point pt3 = new Point(
                                mid_X + (HeadWidth * cost - HeadHeight * sint),
                                mid_Y + (HeadWidth * sint + HeadHeight * cost));

                Point pt4 = new Point(
                                mid_X + (HeadWidth * cost + HeadHeight * sint),
                                mid_Y - (HeadHeight * cost - HeadWidth * sint));

                context.LineTo(mid_point, true, true);
                context.LineTo(pt3, true, true);
                context.LineTo(mid_point, true, true);
                context.LineTo(pt4, true, true);               
            }

        }
        #endregion
    }
}
