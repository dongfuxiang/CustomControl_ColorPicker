using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
namespace ColorLibrary
{
    public class ColorPicker : Control
    {

        #region 01依赖项属性
        //1.声明依赖项属性
        public static readonly DependencyProperty ColorProperty;
        public static readonly DependencyProperty RedProperty;
        public static readonly DependencyProperty GreenProperty;
        public static readonly DependencyProperty BlueProperty;
        public static readonly DependencyProperty AlphaProperty;
        //2.包装依赖项属性，依赖项属性也是一种属性，所以要包装一下，暴露出一般属性的特性方便使用
        public Color MyColor
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }

        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        public byte Alpha
        {
            get { return (byte)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }

        #endregion

        #region 02路由事件
        //1.定义路由事件
        public static readonly RoutedEvent ColorChangedEvent;

        //2.包装路由事件
        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }
        #endregion

        #region 03注册依赖项属性与路由事件
        //3.注册依赖项属性与路由事件
        //注意：必须再使用依赖项属性之前注册依赖项属性，所以依赖项属性总是再所在类静态构造函数或则静态字段中注册
        static ColorPicker()
        {
            //从样式中加载模板,固定写法
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

            //注册依赖项属性
            ColorProperty = DependencyProperty.Register(
                name: "MyColor",
                propertyType: typeof(Color),
                ownerType: typeof(ColorPicker),
                typeMetadata: new PropertyMetadata(propertyChangedCallback: OnColorChanged) { DefaultValue = Colors.Black },//属性改变时触发的回调
                validateValueCallback: ValidateValue//验证
                );

            RedProperty = DependencyProperty.Register(
                name: "Red",
                propertyType: typeof(byte),
                ownerType: typeof(ColorPicker),
                typeMetadata: new PropertyMetadata(propertyChangedCallback: OnRGBChanged),//属性改变时触发的回调
                validateValueCallback: ValidateValue//验证
                );

            GreenProperty = DependencyProperty.Register(
                name: "Green",
                propertyType: typeof(byte),
                ownerType: typeof(ColorPicker),
                typeMetadata: new PropertyMetadata(propertyChangedCallback: OnRGBChanged),
                validateValueCallback: ValidateValue
                );

            BlueProperty = DependencyProperty.Register(
                name: "Blue",
                propertyType: typeof(byte),
                ownerType: typeof(ColorPicker),
                typeMetadata: new PropertyMetadata(propertyChangedCallback: OnRGBChanged),
                validateValueCallback: ValidateValue
                );

            AlphaProperty = DependencyProperty.Register(
                name: "Alpha",
                propertyType: typeof(byte),
                ownerType: typeof(ColorPicker),
                typeMetadata: new PropertyMetadata(propertyChangedCallback: OnRGBChanged),
                validateValueCallback: ValidateValue
                );

            //注册路由事件
            ColorChangedEvent = EventManager.RegisterRoutedEvent(
                name: "ColorChanged",//事件名
                routingStrategy: RoutingStrategy.Bubble,//路由类型，冒泡
                handlerType: typeof(RoutedPropertyChangedEventHandler<Color>),//事件类型
                ownerType: typeof(ColorPicker)//属于哪个对象
                );
        }
        #endregion

        #region 04回调函数
        public static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;
            //MyColor属性改变后会触发OnColorChanged()回调函数，获得新值
            Color newcolor = (Color)e.NewValue;
            Color oldcolor = (Color)e.OldValue;


            //解析Color解析到RGB
            picker.Red = newcolor.R;
            picker.Green = newcolor.G;
            picker.Blue = newcolor.B;
            picker.Alpha = newcolor.A;

            //颜色改变后调用事件
            RoutedPropertyChangedEventArgs<Color> args =
                new RoutedPropertyChangedEventArgs<Color>(oldcolor, newcolor, ColorChangedEvent);

            picker?.RaiseEvent(args);
        }

        public static void OnRGBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = d as ColorPicker;
            Color color = picker.MyColor;


            if (e.Property == RedProperty)
            {
                color.R = (byte)e.NewValue;
            }

            if (e.Property == GreenProperty)
            {
                color.G = (byte)e.NewValue;
            }

            if (e.Property == BlueProperty)
            {
                color.B = (byte)e.NewValue;
            }

            if (e.Property == AlphaProperty)
            {
                color.A = (byte)e.NewValue;
            }

            picker.MyColor = color;
        }
        #endregion

        #region 05验证
        public static bool ValidateValue(object ob)
        {
            return true;
        }
        #endregion

        /// <summary>
        /// 当  DefaultStyleKeyProperty.OverrideMetadata初始化完成Style时回调
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //在后台代码手动建立数据绑定，必须给元素命名，方式为：PART_开头，后跟元素名称
            RangeBase redSlider = GetTemplateChild("PART_RedSlider") as RangeBase;

            Binding binding = new Binding("Red") //Path = Red
            {
                Source = this, //源
                Mode = BindingMode.TwoWay
            };
            redSlider?.SetBinding(RangeBase.ValueProperty, binding);


            //建立Brush的绑定,由于Brush没有SetBinding对象，只能this当作目标，Brush当作源，OneWayToSource，目标改变时改变源
            Brush brush = GetTemplateChild("PART_Brush") as Brush;

            Binding binding1 = new Binding("Color")
            {
                Source = brush,
                Mode = BindingMode.TwoWay
            };
            this.SetBinding(ColorProperty, binding1);
        }
    }
}
