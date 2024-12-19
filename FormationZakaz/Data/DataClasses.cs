using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FormationZakaz.Data
{
    class DataClasses
    {
        static public bool globaltmptabl { get; set; }
    }
    class Out
    {
        public string format { get; set; }
        public decimal posit { get; set; }
        public decimal what { get; set; }
        public decimal kuda { get; set; }
        public decimal quant { get; set; }
        public decimal? ed { get; set; }
        public decimal group { get; set; }
        public decimal? spec { get; set; }
        public decimal? ksi { get; set; }
        public string path { get; set; }
        public decimal root { get; set; }
        public int level { get; set; }
        public int id { get; set; }
        public int pid { get; set; }
        public decimal knk { get; set; }
        public decimal summ { get; set; }
        public string what_dse { get; set; }
        public string kuda_dse { get; set; }
    }
    public class rpathdim
    {
        public string mpath { get; set; }
        public decimal? mzax { get; set; }
        public int? msbor { get; set; }
        public int idx { get; set; }
    }
    /// <summary>
    /// Класс Itog
    /// </summary>
    class Itog
    {
        public string pr { get; set; }
        public string km { get; set; }
        public string fio { get; set; }
        public string hm { get; set; }
        public string gst { get; set; }
        public string prt { get; set; }
        public string gsts { get; set; }
        public decimal hzp { get; set; }
        public DateTime data { get; set; }
        public decimal norm { get; set; }
        public decimal potrebnost { get; set; }
        public string ei { get; set; }
        public string cpn { get; set; }
        public decimal cost { get; set; }
        public decimal normn { get; set; }
        public decimal costn { get; set; }
        public int id { get; set; }
    }
    class outproM
    {
        public decimal zakaz { get; set; }
        public decimal nom { get; set; }
        public decimal posit { get; set; }
        public decimal draft { get; set; }
        public decimal quant { get; set; }
        public decimal across { get; set; }
        public decimal knk { get; set; }
        public decimal? ksi { get; set; }
        public decimal? spec { get; set; }
        public decimal rung { get; set; }
        public decimal summ { get; set; }
        public string path { get; set; }
        public decimal km { get; set; }
        public decimal norm { get; set; }
        public decimal kz { get; set; }
        public DateTime? p_nm { get; set; }
        public DateTime? p_obm { get; set; }
        public DateTime? p_tr { get; set; }
        public decimal mg_pl { get; set; }
        public DateTime? p_pec { get; set; }
        public DateTime? mg_vd { get; set; }
        public DateTime? mg_sp { get; set; }
        public decimal imcom { get; set; }
        public decimal nom_nar { get; set; }
        public DateTime? p_ved { get; set; }
        public DateTime? p_neo { get; set; }
        public decimal g_nar { get; set; }
        public DateTime? p_cex { get; set; }
        public decimal ro { get; set; }
        public DateTime? d_opl { get; set; }
        public DateTime? d_dok { get; set; }
        public string blok { get; set; }
        public decimal cop { get; set; }
        public decimal normold { get; set; }
        public decimal norm_ob { get; set; }
        public decimal vari { get; set; }
        public int id { get; set; }
        public int pid { get; set; }
        public int? cid { get; set; }
        //
        public decimal cost { get; set; }
        public decimal nv { get; set; }
        public decimal zp { get; set; }
        public decimal mt { get; set; }
    }
    class Route
    {
        public string shop { get; set; }
        public bool check { get; set; }
        public int pass { get; set; }
    }
    class complectM
    {
        public string format { get; set; }
        public decimal posit { get; set; }
        public decimal what { get; set; }
        public decimal kuda { get; set; }
        public decimal quant { get; set; }
        public decimal ed { get; set; }
        public decimal group { get; set; }
        public decimal spec { get; set; }
        public decimal ksi { get; set; }
        public string path { get; set; }
        public string izv { get; set; }
        public string dti { get; set; }
        public string tfl { get; set; }
        public string what_DSE { get; set; }
    }
    class mg
    {
        public decimal okv { get; set; }
        public decimal @is { get; set; }
        public decimal k { get; set; }
        public string path { get; set; }
    }
    class pril_zM
    {
        public decimal zak { get; set; }
        public decimal nom { get; set; }
        public decimal ko { get; set; }
        public decimal poz { get; set; }
        public decimal what { get; set; }
        public decimal kol { get; set; }
        public decimal kuda { get; set; }
        public decimal spec { get; set; }
        public string path { get; set; }
        public decimal km { get; set; }
        public decimal norm { get; set; }
        public decimal dd { get; set; }
        public decimal ksi { get; set; }
        public DateTime dat { get; set; }
        public string norm_p { get; set; }
        public string r_zag { get; set; }
        public decimal k_det { get; set; }
    }
    class OutM
    {
        public string format { get; set; }
        public decimal posit { get; set; }
        public decimal what { get; set; }
        public decimal kuda { get; set; }
        public decimal quant { get; set; }
        public decimal? ed { get; set; }
        public decimal group { get; set; }
        public decimal? spec { get; set; }
        public decimal? ksi { get; set; }
        public string path { get; set; }
        public decimal root { get; set; }
        public int level { get; set; }
        public int id { get; set; }
        public int pid { get; set; }
        public decimal knk { get; set; }
        public decimal summ { get; set; }
        public string what_dse { get; set; }
        public string kuda_dse { get; set; }
    }
    public class ClMCSmall
    {
        //   public bool act { get; set; }
        public string km { get; set; }
        public string ei { get; set; }
        public string hm { get; set; }
        public string prt { get; set; }
        public string gst { get; set; }
        public string gsts { get; set; }
        public decimal? hzp { get; set; }
        public decimal? hz { get; set; }
        public string ocen { get; set; }
        public string pr { get; set; }
        public string fio { get; set; }
        public DateTime data { get; set; }
    }
    public static class FocusExtension
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool?), typeof(FocusExtension), new FrameworkPropertyMetadata(IsFocusedChanged) { BindsTwoWayByDefault = true });

        public static bool? GetIsFocused(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool?)element.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject element, bool? value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(IsFocusedProperty, value);
        }

        private static void IsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;

            if (e.OldValue == null)
            {
                fe.GotFocus += FrameworkElement_GotFocus;
                fe.LostFocus += FrameworkElement_LostFocus;
            }

            if (!fe.IsVisible)
            {
                fe.IsVisibleChanged += new DependencyPropertyChangedEventHandler(fe_IsVisibleChanged);
            }

            if (e.NewValue != null && (bool)e.NewValue)
            {
                fe.Focus();
            }
        }

        private static void fe_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            if (fe.IsVisible && (bool)fe.GetValue(IsFocusedProperty))
            {
                fe.IsVisibleChanged -= fe_IsVisibleChanged;
                fe.Focus();
            }
        }

        private static void FrameworkElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, true);
        }

        private static void FrameworkElement_LostFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, false);
        }
    }

    public enum CommandStatus
    {
        EXECUTED,
        FAILED
    }

}
