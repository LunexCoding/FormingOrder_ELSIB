using System.Collections.Generic;
using System.Linq;
//
using Fox;
using FormationZakaz.Data;
using System.Collections.ObjectModel;
using System;
//using System.Windows.Media;
//
namespace FormationZakaz.Models
{
    /// <summary>
    /// Класс mMain (Model)
    /// </summary>
    class mMain : MBase
    {
        #region Переменные

        /// <summary>
        /// Переменная для работы с базой данных
        /// </summary>
        public FOXEntities db;
        //
        // Переменная для свойства
        //
        public Dictionary<string, string> cbList;
       // public string gbResult;
        public int replaces;

        public string order, number, tip;
        public decimal mt, nv, zp;
        public List<outpro> listOutPro;
        public List<complect> listParts;
        public List<rr> rr_;
        public List<tarift> tf_;

        private const string ResultMsg = "Результат :";

        #endregion

        #region Свойства

        // 
        // Свойство
        //
        string gbResult;
        public string pgbResult
        {
            get { return gbResult; }
            set
            {
                if (gbResult != value)
                {
                    gbResult = value;
                    OnPropertyChanged("pgbResult");
                }
            }
        }
        decimal? TbOrder;
        public decimal? pTbOrder
        {
            get { return TbOrder; }
            set
            {
                if (TbOrder != value)
                {
                    TbOrder = value;
                    OnPropertyChanged("pTbOrder");
                }
            }
        }
        decimal? TbNumber;
        public decimal? pTbNumber
        {
            get { return TbNumber; }
            set
            {
                if (TbNumber != value)
                {
                    TbNumber = value;
                    OnPropertyChanged("pTbNumber");
                }
            }
        }
        List<outpro> ListOutPro;
        public List<outpro> pListOutPro
        {
            get { return ListOutPro; }
            set
            {
                if (ListOutPro != value)
                {
                    ListOutPro = value;
                    OnPropertyChanged("pListOutPro");
                }
            }
        }
        string TextBlock;
        public string pTextBlock
        {
            get { return TextBlock; }
            set
            {
                if (TextBlock != value)
                {
                    TextBlock = value;
                    OnPropertyChanged("pTextBlock");
                }
            }
        }
        bool EnCalc;
        public bool pEnCalc
        {
            get { return EnCalc; }
            set
            {
                if (EnCalc != value)
                {
                    EnCalc = value;
                    OnPropertyChanged("pEnCalc");
                }
            }
        }
        bool EnWrite;
        public bool pEnWrite
        {
            get { return EnWrite; }
            set
            {
                if (EnWrite != value)
                {
                    EnWrite = value;
                    OnPropertyChanged("pEnWrite");
                }
            }
        }
        bool EnNew;
        public bool pEnNew
        {
            get { return EnNew; }
            set
            {
                if (EnNew != value)
                {
                    EnNew = value;
                    OnPropertyChanged("pEnNew");
                }
            }
        }
        bool EnPrint;
        public bool pEnPrint
        {
            get { return EnPrint; }
            set
            {
                if (EnPrint != value)
                {
                    EnPrint = value;
                    OnPropertyChanged("pEnPrint");
                }
            }
        }
        bool IsFocusedWrite;
        public bool pIsFocusedWrite
        {
            get { return IsFocusedWrite; }
            set
            {
                if (IsFocusedWrite != value)
                {
                    IsFocusedWrite = value;
                    OnPropertyChanged("pIsFocusedWrite");
                }
            }
        }
        bool  IsFocusedNew;
        public bool  pIsFocusedNew
        {
            get { return IsFocusedNew; }
            set
            {
                if (IsFocusedNew != value)
                {
                    IsFocusedNew = value;
                    OnPropertyChanged("pIsFocusedNew");
                }
            }
        }
        bool IsFocusedOrder;
        public bool pIsFocusedOrder
        {
            get { return IsFocusedOrder; }
            set
            {
                if (IsFocusedOrder != value)
                {
                    IsFocusedOrder = value;
                    OnPropertyChanged("pIsFocusedOrder");
                }
            }
        }
        bool IsFocusedNum;
        public bool pIsFocusedNum
        {
            get { return IsFocusedNum; }
            set
            {
                if (IsFocusedNum != value)
                {
                    IsFocusedNum = value;
                    OnPropertyChanged("pIsFocusedNum");
                }
            }
        }
        bool IsFocusedCalc;
        public bool pIsFocusedCalc
        {
            get { return IsFocusedCalc; }
            set
            {
                if (IsFocusedCalc != value)
                {
                    IsFocusedCalc = value;
                    OnPropertyChanged("pIsFocusedCalc");
                }
            }
        }
        string WrBG;
        public string pWrBG
        {
            get { return WrBG; }
            set
            {
                if (WrBG != value)
                {
                    WrBG = value;
                    OnPropertyChanged("pWrBG");
                }
            }
        }
      
        string ContWrite;
        public string pContWrite
        {
            get { return ContWrite; }
            set
            {
                if (ContWrite != value)
                {
                    ContWrite = value;
                    OnPropertyChanged("pContWrite");
                }
            }
        }
        decimal pmt;
        public decimal ppmt
        {
            get { return pmt; }
            set
            {
                if (pmt != value)
                {
                    pmt = value;
                    OnPropertyChanged("ppmt");
                }
            }
        }
        decimal pnv;
        public decimal ppnv
        {
            get { return pnv; }
            set
            {
                if (pnv != value)
                {
                    pnv = value;
                    OnPropertyChanged("ppnv");
                }
            }
        }
        decimal pzp;
        public decimal ppzp
        {
            get { return pzp; }
            set
            {
                if (pzp != value)
                {
                    pzp = value;
                    OnPropertyChanged("ppzp");
                }
            }
        }


        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> pOrders
        {
            get { return _orders; }
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged(nameof(pOrders));
                }
            }
        }

        private Order _selectedOrder;
        public Order pSelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged(nameof(pSelectedOrder));
                    UpdateOrderData();
                    UpdateLog();
                    UpdateResult();
                }
            }
        }

        private bool _isProgressVisible;
        public bool pIsProgressVisible
        {
            get { return _isProgressVisible; }
            set
            {
                if (_isProgressVisible != value)
                {
                    _isProgressVisible = value;
                    OnPropertyChanged(nameof(pIsProgressVisible));
                }
            }
        }

        private double _progress;
        public double pProgress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged(nameof(pProgress));
                }
            }
        }


        private bool _accessCommandElementsForOneOrder;
        public bool pAccessCommandElementsForOneOrder
        {
            get { return _accessCommandElementsForOneOrder; }
            set
            {
                if (_accessCommandElementsForOneOrder != value)
                {
                    _accessCommandElementsForOneOrder = value;
                    OnPropertyChanged(nameof(pAccessCommandElementsForOneOrder));
                }
            }
        }

        private bool _accessCommandElementsForOrders;
        public bool pAccessCommandElementsForOrders
        {
            get { return _accessCommandElementsForOrders; }
            set
            {
                if (_accessCommandElementsForOrders != value)
                {
                    _accessCommandElementsForOrders = value;
                    OnPropertyChanged(nameof(pAccessCommandElementsForOrders));
                }
            }
        }
        
        private bool _isAllSelectedOrders;
        public bool pIsAllSelected
        {
            get { return _isAllSelectedOrders; }
            set
            {
                if (_isAllSelectedOrders != value)
                {
                    _isAllSelectedOrders = value;
                    OnPropertyChanged(nameof(pIsAllSelected));
                }
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Конструктор
        /// </summary>
        public mMain()
        {
            db = new FOXEntities();
            db.Connection.Open();
            db.CommandTimeout = 6000;
            //
            // Инициализация свойств
            // pName = значение;
            //pTitle = VMLocator.programName + " (" + Environment.UserDomainName + @"\" + Environment.UserName + ")";
            //
            replaces = 0;
            order = "0";
            number = "0";
            tip = "0";
            gbResult = "Результат:";
            WrBG = "Gray";
            pContWrite = "Запись в OUTPRO";

            pOrders = new ObservableCollection<Order>();
           
            //
            GetList();
        }
        public void ReSetVars()
        {
            replaces = 0;
            order = ""; number = ""; tip = "";
            mt = 0; nv = 0; zp = 0;
            if (listOutPro != null) listOutPro.Clear();
            if (listParts != null) listParts.Clear();
           
        }
        /// <summary>
        /// Получить cif_bukf лист
        /// </summary>
        void GetList()
        {
            cbList = new Dictionary<string, string>();
            var _cbL = (from p in db.CIF_BUKF select p).ToList();
                foreach(var v in _cbL)
            {
                cbList.Add(v.buk.Trim(), v.cif.ToString().Trim());
            }
                 rr_ = (from p in db.rr select p).ToList();
                  tf_ = (from p in db.tarift select p).ToList();
        }

        private void UpdateLog()
        {
            if (pSelectedOrder != null)
            {
                pTextBlock = pSelectedOrder.Log;
            }
        }

        private void UpdateResult()
        {
            if (pSelectedOrder != null)
            {
                pListOutPro = pSelectedOrder.Result;
                pgbResult = ResultMsg + " " + pSelectedOrder.CountResult;
            }
        }

        private void UpdateOrderData()
        {
            if (pSelectedOrder != null)
            {
                pTbOrder = pSelectedOrder.OrderID;
                pTbNumber = pSelectedOrder.Number;
            }
        }

        public List<Order> mGetOrders(DateTime firstDate, DateTime secondDate)
        {
            return db.pl_god
            .Where(res => res.data >= firstDate && res.data <= secondDate)
            .Select(res => new Order
                {
                    OrderID = res.zakaz,
                    Number = res.nom,
                    Name = res.name,
                    Draft = res.draft,
                    MainOrderID = res.zak_os,
                    MainOrderNumber = res.nom_os,
                    ReleaseDate = res.dvippl,
                    ProductType = res.tip,
                    FactoryServices = res.typ,
                    Author = res.avtor,
                    Application = res.npril,
                    Log = String.Empty,
                    pStatus = OrderStatus.DEFAULT,
                    IsSelected = false
                 }
            )
            .ToList();
        }

        public List<Order> mGetSelectedOrders()
        {
            return pOrders
                .Where(order => order.IsSelected == true)
                .ToList();
        }

        public void mUpdateOrders(List<Order> orders) 
        { 
            // Проверка на null
            if (orders == null) 
            { 
                return; 
            } 
 
            foreach (var order in orders) 
            { 
                // Проверяем наличие записи в outpro
                outpro row = mFindOrderInOutpro(order); 
 
                // Если запись найдена, пропускаем заказ
                if (row != null) 
                { 
                    continue; 
                } 
 
                // Ищем заказ в pOrders
                var existingOrder = pOrders.FirstOrDefault(o => o.OrderID == order.OrderID && o.Number == order.Number); 
 
                if (existingOrder == null) 
                { 
                    // Если заказа нет в pOrders, добавляем его
                    pOrders.Add(order); 
                    OnPropertyChanged(nameof(pOrders)); 
                } 
                else if (existingOrder.pStatus == OrderStatus.COMPLECTED) 
                { 
                    // Если заказ найден и статус Completed, удаляем его из pOrders
                    pOrders.Remove(existingOrder); 
                    OnPropertyChanged(nameof(pOrders)); 
                } 
            } 
        }

        private outpro mFindOrderInOutpro(Order order)
        {
            return db.outpro
                .Where(outpro =>
                    outpro.nom == order.Number &&
                    outpro.zakaz == order.OrderID
                )
                .FirstOrDefault();
        }


        #endregion
    }
}

