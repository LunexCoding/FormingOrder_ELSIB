using System.Collections.Generic;
using System.Linq;
//
using Fox;
using FormationZakaz.Data;
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
            //
           
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
        #endregion
    }
}

