using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FormationZakaz.Data
{
    public class Order : INotifyPropertyChanged
    {
        public decimal OrderID { get; set; }
        public decimal Number { get; set; }
        public string Name { get; set; }
        public decimal? Draft { get; set; }
        public decimal? MainOrderID { get; set; }
        public decimal? MainOrderNumber { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? ProductType { get; set; }
        public decimal? FactoryServices { get; set; }
        public string Author { get; set; }
        public string Application { get; set; }
        public decimal CountResult { get; set; }
        public string Log { get; set; }
        private OrderStatus _status = OrderStatus.DEFAULT;
        public OrderStatus pStatus
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(pStatus));
                }
            }
        }
        private bool _isSelected;
        public List<outpro> Result { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public enum OrderStatus
    {
        DEFAULT, // Дефолтное значение
        COMPLECTED, // Выполнен
        NOT_COMPLECTED // Не выполнен
    }

}
