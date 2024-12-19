using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
//
using Fox;
using FormationZakaz.Data;
using FormationZakaz.Models;
using FormationZakaz.Views;
using System.Windows.Documents;
using System.Net.Mail;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.Windows.Controls;
//
namespace FormationZakaz.ViewModels
{
    /// <summary>
    /// Класс Main (ViewModel)
    /// </summary>
    class vmMain : VMBase
    {
        #region Переменные

        /// <summary>
        /// Переменная View
        /// </summary>
        public Main View;
        /// <summary>
        /// Переменная Model
        /// </summary>
        public mMain Model;
        private BackgroundWorker _worker;

        private const string ResultMsg = "Результат :";
        //
        // Переменная для свойства команды
        //
        //CommandBase commandName;

        #endregion

        #region Свойства


        CommandBase pDDisx;

        public CommandBase ppDDisx
        {
            get { return pDDisx ?? (pDDisx = new CommandBase(mpDDisx)); }
        }
      
        CommandBase BtnCalc;

        public CommandBase pBtnCalc
        {
            get { return BtnCalc ?? (BtnCalc = new CommandBase(_mBtnCalcWithFlag)); }
        }

        
        CommandBase BtnWrite;

        public CommandBase pBtnWrite
        {
            get { return BtnWrite ?? (BtnWrite = new CommandBase(mBtnWrite)); }
        }
        
        CommandBase BtnNew;

        public CommandBase pBtnNew
        {
            get { return BtnNew ?? (BtnNew = new CommandBase(mBtnNew)); }
        }
        
        CommandBase BtnPrint;

        public CommandBase pBtnPrint
        {
            get { return BtnPrint ?? (BtnPrint = new CommandBase(mBtnPrint)); }
        }
        
        CommandBase FocusNum;

        public CommandBase pFocusNum
        {
            get { return FocusNum ?? (FocusNum = new CommandBase(mFocusNum)); }
        }
        
        CommandBase FocusCalc;

        public CommandBase pFocusCalc
        {
            get { return FocusCalc ?? (FocusCalc = new CommandBase(mFocusCalc)); }
        }
        
        CommandBase Excel;

        public CommandBase pExcel
        {
            get { return Excel ?? (Excel = new CommandBase(mExcel)); }
        }


        private CommandBase _LoadOrders;
        public CommandBase pLoadOrders
        {
            get { return _LoadOrders ?? (_LoadOrders = new CommandBase(_mLoadOrders)); }
        }


        private CommandBase _CalcAllOrders;
        public CommandBase pCalcAllOrders
        {
            get { return _CalcAllOrders ?? (_CalcAllOrders = new CommandBase(_mStartCalculation)); }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Конструктор
        /// </summary>
        public vmMain()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += _mWorker_DoWork;
            _worker.ProgressChanged += _mWorker_ProgressChanged;
            _worker.RunWorkerCompleted += _mWorker_RunWorkerCompleted;
        }
        /// <summary>
        /// Обработчик события загрузки View
        /// </summary>
        /// <param name="_sender"></param>
        /// <param name="_routedEventArgs"></param>
        public void viewLoaded(object _sender, RoutedEventArgs _routedEventArgs)
        {
            View = (Main)view;
            Model = (mMain)model;
            //
            Model.PropertyChanged += modelPropertyChangedHandler;
            Model.pEnNew = true;
            Model.pEnWrite = false;
            Model.pEnPrint = false;
            Model.pgbResult = ResultMsg;
            Model.pEnCalc = false;
            Model.pIsProgressVisible = false;
            Model.pAccessCommandElementsForOneOrder = true;
            Model.pAccessCommandElementsForOrders = true;

            System.Drawing.Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            if (resolution.Height * 2 / 3 > 600)
            {
                View.Width = resolution.Width * 2 / 3;
                View.Height = resolution.Width * 2 / 3 * resolution.Height / resolution.Width;
                View.Left = View.Left - (resolution.Width * 2 / 3 - 800) / 2;
                View.Top = View.Top - (View.Height - 600) / 2;
            }

            _mLoadOrders();

        }

        /// <summary>
        /// Обработчик изменения свойств модели
        /// </summary>
        /// <param name="_sender"></param>
        /// <param name="_eventArgs"></param>
        public void modelPropertyChangedHandler(object _sender, PropertyChangedEventArgs _eventArgs)
        {
            if (_eventArgs.PropertyName == "pTbNumber" && Model.pTbOrder > 0 && Model.pTbNumber > 0)
            {
               // Model.pCalcBG = "LightGreen";
                Model.pEnCalc = true;
            }
            if (_eventArgs.PropertyName == "pTbNumber" && (Model.pTbOrder == 0 || Model.pTbNumber == 0))
            {
                //Model.pCalcBG = "LightGray";
                Model.pEnCalc = false;
            }
            if ( _eventArgs.PropertyName == "pIsAllSelected")
            {
                _mSetAllOrdersSelection(Model.pIsAllSelected);
            }
        }


        public void mExcel()
        {
            if (Model.pListOutPro != null && Model.pListOutPro.Count > 0)
            {
                string tfile = Path.GetTempPath() + Path.GetRandomFileName().Replace('.', ' ') + ".xlsx";
                FileInfo newFile = new FileInfo(tfile);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Сменное задание");
                    worksheet.Cells[1, 1].Value = "Заказ";
                    worksheet.Cells[1, 2].Value = "№";
                    worksheet.Cells[1, 3].Value = "Поз.";
                    worksheet.Cells[1, 4].Value = "Чертеж";
                    worksheet.Cells[1, 5].Value = "Колич. непоср.";
                    worksheet.Cells[1, 6].Value = "Узел";
                    worksheet.Cells[1, 7].Value = "К-во в нар.";
                    worksheet.Cells[1, 8].Value = "РСП";
                    worksheet.Cells[1, 9].Value = "КСИ";
                    worksheet.Cells[1, 10].Value = "Ранг";
                    worksheet.Cells[1, 11].Value = "К-во- полн.";
                    worksheet.Cells[1, 12].Value = "Маршрут";
                    worksheet.Cells[1, 13].Value = "Код мат";
                    worksheet.Cells[1, 14].Value = "К-во мат. в нар.";
                    worksheet.Cells[1, 15].Value = "Дата форм";
                    worksheet.Cells[1, 16].Value = "№ нар.";



                    var recm = (from p in Model.pListOutPro

                                select new
                                {
                                    p,

                                }).ToList();

                    int k = 1;
                    foreach (var v in recm)
                    {
                        k++;

                        worksheet.Cells[k, 1].Value = v.p.zakaz;
                        worksheet.Cells[k, 2].Value = v.p.nom;
                        worksheet.Cells[k, 3].Value = v.p.posit;
                        worksheet.Cells[k, 4].Value = v.p.draft;
                        worksheet.Cells[k, 5].Value = v.p.quant;
                        worksheet.Cells[k, 6].Value = v.p.across;
                        worksheet.Cells[k, 7].Value = v.p.knk;
                        worksheet.Cells[k, 8].Value = v.p.spec;
                        worksheet.Cells[k, 9].Value = v.p.ksi;
                        worksheet.Cells[k, 10].Value = v.p.rung;
                        worksheet.Cells[k, 11].Value = v.p.summ;
                        worksheet.Cells[k, 12].Value = v.p.path;
                        worksheet.Cells[k, 13].Value = v.p.km;
                        worksheet.Cells[k, 14].Value = v.p.norm;
                        worksheet.Cells[k, 15].Value = v.p.p_nm;
                        worksheet.Cells[k, 16].Value = v.p.nom_nar;

                    }
                    worksheet.Cells[2, 4, k, 4].Style.Numberformat.Format = "0.00";
                    worksheet.Cells[2, 6, k, 6].Style.Numberformat.Format = "0.00";
                    worksheet.Cells[2, 5, k, 5].Style.Numberformat.Format = "0.00000";
                    worksheet.Cells[2, 11, k, 11].Style.Numberformat.Format = "0.00000";
                    worksheet.Cells[2, 7, k, 7].Style.Numberformat.Format = "0.00000";
                    worksheet.Cells[2, 15, k, 15].Style.Numberformat.Format = "dd.mm.yyyy";
                    worksheet.Cells[1, 1, k, 16].AutoFitColumns(0);
                    worksheet.Cells[1, 1, 1, 16].Style.Font.Bold = true;

                    package.Save();

                    Process.Start(tfile);
                }
            }
        }

        /// <summary>
        /// Кнопка Расчет
        /// </summary>
        // +
        public CommandStatus mBtnCalc(bool showMessageFlag)
        {
            CommandStatus status;
            Model.pTextBlock = "";// View.rtbInfo.Document.Blocks.Clear();
               Model.pListOutPro = new List<outpro>(); // View.dgResult.Items.Clear();
               Model.ReSetVars();
                        //
                        if (Model.pTbOrder!=null && Model.pTbNumber!=null && Model.pTbOrder>0 && Model.pTbNumber>0)
                        {
                           
                            Model.pWrBG = "Black";
                           
                            status = CalcOutPro(showMessageFlag);
                            //
                            if (Model.pEnWrite == true)
                                Model.pEnWrite = true;// View.btnWrite.IsEnabled = true;
                            else
                                Model.pEnWrite = false;

                            Model.pIsFocusedWrite = true;//  View.btnWrite.Focus();
                            Model.pEnNew = true;// View.btnNew.IsEnabled = true;
                            Model.pEnPrint = true;// View.btnPrint.IsEnabled = true;
                            
                        }
                
            else
            {
                if (showMessageFlag)
                {
                    MessageBox.Show("Не заполнено поле Заказ и/или Номер!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                status = CommandStatus.FAILED;
            }
            return status;
                
        }
       
        /// <summary>
        /// Кнопка Запись
        /// </summary>
        // +
        public void mBtnWrite()
        {
            Console.WriteLine("Lock");

        //    if (Model.pListOutPro.Count > 0)
        //    {
        //        //********************* проверим маршрут
        //        bool flyes = true;
        //        List<rpathdim> pth = new List<rpathdim>();
        //        int mcoun = 0;
        //        string cxx = "";
        //        var rrt = (from p in Model.db.rr
        //                       //where p.cexzp == "ro"
        //                   select new
        //                   {
        //                       cex = p.cex
        //                   }).ToList();
        //        var Zcompo = (from p in Model.pListOutPro
        //                      select new
        //                      {
        //                          zakaz = p.zakaz,
        //                          nom = p.nom,
        //                          what = p.draft,
        //                          kuda = p.across,
        //                          path = p.path,
        //                          nom_nar = p.nom_nar
        //                      }).ToList();
        //        // pZnoincomp = new List<noOutpro>();
        //        foreach (var v in Zcompo)
        //        {
        //            try
        //            {
        //                pth = rpath(v.path, out mcoun); //разобранный список маршрутп
        //                for (int i = 0; i < mcoun; i++)
        //                {

        //                    cxx = pth[i].mpath;
        //                    var yy = rrt.FirstOrDefault(p => p.cex == cxx);
        //                    if (yy == null)
        //                    {
        //                        AddTextToRtbInfo("Ошибка: Чертеж Что = " + v.what.ToString().Replace(',', '.') + " в узле =" + v.kuda.ToString().Replace(',', '.') + " \n    в маршруте " + v.path + " ошибка \n см. цех " + cxx, true);
        //                        flyes = false;
        //                    }

        //                }
        //            }
        //            catch
        //            {
        //                AddTextToRtbInfo("Ошибка: Чертеж Что =" + v.what.ToString().Replace(',', '.') + " в узле =" + v.kuda.ToString().Replace(',', '.') + " \n    в маршруте " + v.path + " ошибка \n см. цех " + cxx, true);
        //                flyes = false;
        //            }
        //        }

        //        if (flyes)
        //        {
        //            if (MessageBox.Show("Действительно выполнить запись в базу?", "Внимание!", MessageBoxButton.YesNo).ToString() == "Yes")
        //            {
        //                WriteFromPrilzTmpToPrilz();
        //                //
        //                if (Model.replaces > 0) WriteZ_td7AndOutBnsi();
        //                else WriteIntoOutproAndPlgod();
        //                //
        //                Model.pWrBG = "Green";//View.btnWrite.Background = new SolidColorBrush(Colors.Green);
        //                //
        //                Model.pEnNew = true;// View.btnNew.IsEnabled = true;
        //                Model.pIsFocusedNew = true;//View.btnNew.Focus();
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Запись не произведена - в маршрутах ошибки!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        }
        //        //**************************
        //    }
        //    else
        //        MessageBox.Show("Расчёт не выполнен!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public List<rpathdim> rpath(string lpath, out int mcount)
        {
            List<rpathdim> mpathl = new List<rpathdim>();
            int m, i, j, n, nn, nl, nfirst, rdim, mm, l;
            m = i = j = n = nn = nl = nfirst = rdim = mm = l = 0;

            string a = "";
            if (lpath != null)
            {
                if (lpath.Trim().Length > 0)
                {
                    nl = CountWords(lpath, "-");
                    rdim = nl - CountWords(lpath, "-()") - CountWords(lpath, "-[]"); ;
                    for (int kk = 0; kk < rdim; kk++)
                    {
                        rpathdim tdim = new rpathdim();
                        tdim.mpath = "   ";
                        tdim.mzax = tdim.msbor = 0;
                        mpathl.Add(tdim);
                    }
                    nfirst = lpath.IndexOf("-") + 1;
                    for (i = 0; i < nl; i++)
                    {
                        m = lpath.IndexOf("-", m) + 1;

                        nn = lpath.Length - m < 3 ? lpath.Length - m : 3;
                        a = lpath.Substring(m, nn).PadRight(3);
                        if (a != "   " || m == nfirst)
                        {
                            if (a.IndexOf("()") > -1)
                                mpathl[i - 1 - mm].msbor = 1;
                            if (a.IndexOf("()") > -1 || a.IndexOf("[]") > -1 || a.IndexOf("   ") > -1 && m == nfirst)
                            {
                                mm++;
                                continue;
                            }
                            n = 1;
                            for (j = 0; j < rdim; j++)
                            {
                                if (mpathl[j].mpath == a && mpathl[j].msbor == 0)//*****************
                                    n++;
                                else
                                {
                                    if (mpathl[j].mpath == "   ")
                                    {
                                        mpathl[j].mpath = a;
                                        mpathl[j].mzax = n;
                                        mpathl[j].idx = i;
                                        l = j;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                            break;
                    }
                    mcount = l + 1;
                    return mpathl;
                }
                else
                {
                    mcount = l;
                    return mpathl;
                }
            }
            else
            {
                mcount = 0;
                return mpathl;
            }
        }
        public int CountWords(string s, string s0)
        {
            int count = (s.Length - s.Replace(s0, "").Length) / s0.Length;
            return count;
        }

        
        /// <summary>
        /// Кнопка Новый
        /// </summary>
        // +
        public void mBtnNew()
        {
            Model.pTbOrder = 0;// View.tbOrder.Text = "";
            Model.pTbNumber = 0;// View.tbNumber.Text = "";

            Model.pIsFocusedOrder = true;// View.tbOrder.Focus();
            Model.pTextBlock = ""; //View.rtbInfo.Document.Blocks.Clear();
            //
            Model.ReSetVars();
            //
            Model.pListOutPro = new List<outpro>(); //View.dgResult.ItemsSource = null;
            //
            Model.pgbResult = ResultMsg;
            Model.pEnCalc = false;// View.btnCalc.IsEnabled = true;
            Model.pEnWrite = false;// View.btnWrite.IsEnabled = false;
            Model.pWrBG = "Black"; //View.btnWrite.Background = null;
            Model.pContWrite = "Запись в outpro";// View.btnWrite.Content = "Запись в outpro";
            Model.pEnPrint = false; //View.btnPrint.IsEnabled = false;
            //View.gbResult.Header = Model.pgbResult;
        }
       
        /// <summary>
        /// Кнопка Печать
        /// </summary>
        //+
        public void mBtnPrint()
        {
          
            string FileName = String.Format(@"{0}.txt", System.Guid.NewGuid());
            File.WriteAllText(FileName, Model.pTextBlock);
            Process.Start(VMLocator.programLocation +"\\" +FileName);
           
        }
        //+
        /// <summary>
        /// Фокус на номер
        /// </summary>
        public void mFocusNum()
        {
            Model.pIsFocusedNum = true;
        }
        //+
        /// <summary>
        /// Фокус на кнопку расчет
        /// </summary>
        public void mFocusCalc()
        {
            Model.pIsFocusedCalc = true;
        }
        public void mpDDisx()
        {
            string npril = "";
            var _plg = Model.db.pl_god.FirstOrDefault(p => p.zakaz == Model.pTbOrder && p.nom == Model.pTbNumber);
                //
            if (_plg != null)
            {
                npril = _plg.npril.Trim();//чертеж приложения
                var prilzList = new List<pril_zM>();
                if (!string.IsNullOrEmpty(npril))
                {
                    var _izvv = (from p in Model.db.izvv where p.nom == npril select p).ToList();

                    if (_izvv != null && _izvv.Count > 0)
                    {
                        string tfile = Path.GetTempPath() + Path.GetRandomFileName().Replace('.', ' ') + ".xlsx";
                        FileInfo newFile = new FileInfo(tfile);
                        using (ExcelPackage package = new ExcelPackage(newFile))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Приложение");
                            worksheet.Cells[1, 1].Value = "nom";
                            worksheet.Cells[1, 2].Value = "data";
                            worksheet.Cells[1, 3].Value = "№ стр";
                            worksheet.Cells[1, 4].Value = "n_lisk";
                            worksheet.Cells[1, 5].Value = "Формат";
                            worksheet.Cells[1, 6].Value = "Код обн.";
                            worksheet.Cells[1, 7].Value = "Поз.";
                            worksheet.Cells[1, 8].Value = "Чертеж что";
                            worksheet.Cells[1, 9].Value = "Исп. от";
                            worksheet.Cells[1, 10].Value = "Исп. до";
                            worksheet.Cells[1, 11].Value = "Наименование";
                            worksheet.Cells[1, 12].Value = "razm";
                            worksheet.Cells[1, 13].Value = "К-во ";
                            worksheet.Cells[1, 14].Value = "Четеж куда";
                            worksheet.Cells[1, 15].Value = "isk_ot";
                            worksheet.Cells[1, 16].Value = "РСП";
                            worksheet.Cells[1, 17].Value = "КСИ";
                            worksheet.Cells[1, 18].Value = "Масса";
                            worksheet.Cells[1, 19].Value = "mat_k";
                            worksheet.Cells[1, 20].Value = "n_izd";
                            worksheet.Cells[1, 21].Value = "iz_t";
                            worksheet.Cells[1, 22].Value = "mat_t";
                            worksheet.Cells[1, 23].Value = "Код мат.";
                            worksheet.Cells[1, 24].Value = "Разм.заг.";
                            worksheet.Cells[1, 25].Value = "к-во дет";
                            worksheet.Cells[1, 26].Value = "Норма мат";
                            worksheet.Cells[1, 27].Value = "Ед изм";
                            worksheet.Cells[1, 28].Value = "Маршрут";
                            worksheet.Cells[1, 29].Value = "Заказ";
                            worksheet.Cells[1, 30].Value = "№";
                            worksheet.Cells[1, 31].Value = "ФИО_К";
                            worksheet.Cells[1, 32].Value = "t_z";
                            worksheet.Cells[1, 33].Value = "tr";
                            worksheet.Cells[1, 34].Value = "id";
                            worksheet.Cells[1, 35].Value = "ФИО_Т";


                            var recm = (from p in _izvv

                                        select new
                                        {
                                            p,

                                        }).ToList();

                            int k = 1;
                            foreach (var v in recm)
                            {
                                k++;

                                worksheet.Cells[k, 1].Value = v.p.nom;
                                worksheet.Cells[k, 2].Value = v.p.data;
                                worksheet.Cells[k, 3].Value = v.p.nstr;
                                worksheet.Cells[k, 4].Value = v.p.n_lisk;
                                worksheet.Cells[k, 5].Value = v.p.format;
                                worksheet.Cells[k, 6].Value = v.p.k_ob;
                                worksheet.Cells[k, 7].Value = v.p.posit;
                                worksheet.Cells[k, 8].Value = v.p.what;
                                worksheet.Cells[k, 9].Value = v.p.is_ot;
                                worksheet.Cells[k, 10].Value = v.p.is_do;
                                worksheet.Cells[k, 11].Value = v.p.name;
                                worksheet.Cells[k, 12].Value = v.p.razm;
                                worksheet.Cells[k, 13].Value = v.p.quant;
                                worksheet.Cells[k, 14].Value = v.p.kuda;
                                worksheet.Cells[k, 15].Value = v.p.isk_ot;
                                worksheet.Cells[k, 16].Value = v.p.spec;

                                worksheet.Cells[k, 17].Value = v.p.ksi;
                                worksheet.Cells[k, 18].Value = v.p.mm;
                                worksheet.Cells[k, 19].Value = v.p.mat_k;
                                worksheet.Cells[k, 20].Value = v.p.n_izd;
                                worksheet.Cells[k, 21].Value = v.p.iz_t;
                                worksheet.Cells[k, 22].Value = v.p.mat_t;
                                worksheet.Cells[k, 23].Value = v.p.km;
                                worksheet.Cells[k, 24].Value = v.p.r_zag;
                                worksheet.Cells[k, 25].Value = v.p.k_det;
                                worksheet.Cells[k, 26].Value = v.p.norm;
                                worksheet.Cells[k, 27].Value = v.p.ei;
                                worksheet.Cells[k, 28].Value = v.p.path;
                                worksheet.Cells[k, 29].Value = v.p.zak;
                                worksheet.Cells[k, 30].Value = v.p.nk;
                                worksheet.Cells[k, 31].Value = v.p.fio;
                                worksheet.Cells[k, 32].Value = v.p.t_z;
                                worksheet.Cells[k, 33].Value = v.p.tr;
                                worksheet.Cells[k, 34].Value = v.p.id;
                                worksheet.Cells[k, 35].Value = v.p.fio_t;

                            }
                            //worksheet.Cells[2, 8, k, 8].Style.Numberformat.Format = "0.00";
                            //worksheet.Cells[2, 14, k, 14].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[2, 26, k, 26].Style.Numberformat.Format = "0.00000";
                            worksheet.Cells[2, 13, k, 13].Style.Numberformat.Format = "0.00000";
                            worksheet.Cells[2, 18, k, 18].Style.Numberformat.Format = "0.00000";
                            worksheet.Cells[2, 2, k, 2].Style.Numberformat.Format = "dd.mm.yyyy";
                            worksheet.Cells[1, 1, k, 35].AutoFitColumns(0);
                            worksheet.Cells[1, 1, 1, 35].Style.Font.Bold = true;

                            package.Save();

                            Process.Start(tfile);
                        }
                    }

                }
            }
        }
        //+
        /// <summary>
        /// Расчет
        /// </summary>
        public CommandStatus CalcOutPro(bool showMessageFlag)
        {
            CommandStatus status = CommandStatus.EXECUTED;
            CommandStatus subStatus = CommandStatus.EXECUTED;
            decimal draft = 0;
            var notErr = true;
            //
       
            var _Outp=Model.db.outpro.FirstOrDefault(p=>p.zakaz==Model.pTbOrder && p.nom==Model.pTbNumber);
           
            if ( _Outp!=null)// есть заказ в outpro?
            {
                    AddTextToRtbInfo("Заказ уже есть в OUTPRO.", true);
                    if (showMessageFlag)
                    {
                        var res = MessageBox.Show("Заказ уже есть в OutPro. \nПродолжить расчёт?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        //
                        notErr = (res.ToString() == "Yes");
                        if (!notErr)
                        {
                            status = CommandStatus.FAILED;
                        }
                        Model.pEnWrite = false;
                    }
                    else
                    {
                        notErr = false;
                        status = CommandStatus.FAILED;
                }
            }
            else
            {
                Model.pEnWrite = true;
                
            }
                
            
            //
            if (notErr)
            {
              
                var _plg = Model.db.pl_god.FirstOrDefault(p => p.zakaz == Model.pTbOrder && p.nom == Model.pTbNumber);
                //
                if (_plg != null)
                {
                    var prilzList = new List<pril_zM>();
                    var kudaList = new List<decimal>();
                    var compList = new List<complect>();
                    decimal tip = 0;
                    decimal m_otgr = 0;
                    decimal g_otgr = 0;
                    string npril = "";

                    draft = (decimal)_plg.draft;//чертеж общего вида из план года
                    tip = (decimal)_plg.tip;//тип заказа
                    npril = _plg.npril.Trim();//чертеж приложения
                    m_otgr = (decimal)_plg.m_otgr;// признаки товара
                    g_otgr = (decimal)_plg.g_otgr;//

                    Model.tip = tip.ToString();

                    if (m_otgr == 0 && g_otgr == 0)
                    {
                        // если не в товаре
                        if (tip == 1 || tip == 2 || tip == 3 || tip == 5)
                        {
                            //не оснастка и не з/ч
                            AddTextToRtbInfo("Основная продукция.", false);
                            if (!string.IsNullOrEmpty(npril))
                            {
                                // если есть приложение
                                GetPrilzAndKudaList(kudaList, npril, prilzList);//формируем список узлов, подлежащих изменению и приложение для данного заказа ( будущий Prilz)
                                //
                                if (CheckOut(prilzList, draft) != "") // проверка на наличие изменяемых и удаляемых позиций приложения в OUT
                                {
                                    //есть ошибки
                                    if (showMessageFlag)
                                    {
                                        Model.pWrBG = "Red";// View.btnWrite.Background = new SolidColorBrush(Colors.Red);
                                        var res = MessageBox.Show("Ошибки при сверке с Out. \nПродолжить?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                                        //
                                        notErr = (res.ToString() == "Yes");
                                        if (!notErr)
                                        {
                                            status = CommandStatus.FAILED;
                                        }
                                    }
                                    else
                                    {
                                        notErr = false;
                                        status = CommandStatus.FAILED;
                                    }
                                }
                                
                                //
                                if (notErr)
                                 {
                                    // если продолжаем...
                                    GetComplList(kudaList, compList);// записываем в comList все узлы из COMPLECT, которые затрагивает приложение, с учетом групповых признаков

                                    AddTextToRtbInfo(DateTime.Now + " CompList до модификации: " + compList.Count, false);
                                    //
                                    ModifyComplect(prilzList.OrderByDescending(z => z.ko).ToList(), compList, draft.ToString()); //вносим изменения по приложению в CompList с учетом группового признака
                                }
                            }
                            //
                            if (notErr)
                            { //если продолжаем...

                                CreateAndWriteComplTable(compList); // формируем временный частичный комплект #Compl на основе CompList
                                //

                                CreateAndWritePrilzTmpTable("#prilz", prilzList);// формируем временный частичный комплект #prilz на основе prilzlist
                                //

                                subStatus = FormOutPro(draft, 0, prilzList.Where(z => z.ko == 1 || z.ko == 3).ToList(), showMessageFlag);
                                //формируем Listoutpro - для записи в outpro или out_bnsi
                                if (subStatus == CommandStatus.FAILED)
                                    status = CommandStatus.FAILED;

                                //status = subStatus == CommandStatus.EXECUTED ? status = CommandStatus.EXECUTED : status = CommandStatus.FAILED;
                            }
                        }
                        else

                            if (tip == 4)
                            {
                                AddTextToRtbInfo("Запчасти.", false);
                                //
                                if (Convert.ToInt32(Model.pTbNumber) >= 900)
                                {
                                    draft = Convert.ToDecimal(Model.pTbOrder.ToString() + Model.pTbNumber.ToString() + ",00");
                                    //
                                    GetPartsComplList(compList, draft, showMessageFlag);
                                    //
                                    CreateAndWriteComplTable(compList);
                                    //
                                    subStatus = FormOutPro(draft, 0, null, showMessageFlag);//формируем Listoutpro - для записи в outpro 
                                    if (subStatus == CommandStatus.FAILED)
                                        status = CommandStatus.FAILED;
                                }
                                else
                                {
                                AddTextToRtbInfo("Ошибка: номер заказа меньше 900.", true);
                                    status = CommandStatus.FAILED;
                                }
                            }
                            else
                            {
                                if (tip == 6)
                                {
                                    AddTextToRtbInfo("Оснастка.", false);
                                    //
                                    var dr = GetProdList();
                                    AddTextToRtbInfo("Чертёж: " + draft.ToString().Replace(',', '.'), false);//dr
                                                                                                             //
                                    if (draft == dr)
                                    {

                                        subStatus = FormOutPro(draft, 1, null, showMessageFlag);//формируем Listoutpro - для записи в outpro 
                                        if (subStatus == CommandStatus.FAILED)
                                            status = CommandStatus.FAILED;
                                    //
                                    Model.pWrBG = "RoyalBlue";// View.btnWrite.Background = new SolidColorBrush(Colors.RoyalBlue);
                                        
                                    }
                                    else
                                    {
                                        AddTextToRtbInfo("Ошибка: Чертеж  в  PL_GOD =" + draft.ToString().Replace(',', '.') + " не совпадает с\n              чертежом из PROD =" + dr.ToString().Replace(',', '.') + " \n              Заказ не сформирован. ", true);
                                        status = CommandStatus.FAILED;
                                    }
                                }
                                else
                                {
                                    AddTextToRtbInfo("Ошибка tip в pl_god .", true);
                                    status = CommandStatus.FAILED;
                                }
                            }
                     }
                    else
                    {
                                AddTextToRtbInfo("Заказ в товаре", true);
                                Model.pEnWrite = false;
                                status = CommandStatus.FAILED;
                    }
                }
                else
                {
                    AddTextToRtbInfo("Заказа нет в pl_god.", true);
                    Model.pEnWrite = false;
                    status = CommandStatus.FAILED;
                }
            }
            else
            {
                status = CommandStatus.FAILED;
            }
            return status;
        }

        //+
        /// <summary>
        /// Добавить текст в окно ИНФО
        /// </summary>
        /// <param name="text"></param>
        /// <param name="err"></param>
        void AddTextToRtbInfo(string text, bool err)
        {
          
            Model.pTextBlock = Model.pTextBlock + text.Trim()+"\n";
            
            if (err)
                
                Model.pWrBG = "Red";
            DoEvents();
        }
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
               new DispatcherOperationCallback((f) =>
               {
                   ((DispatcherFrame)f).Continue = false;
                   return null;
               }), frame);
            Dispatcher.PushFrame(frame);
        }

        //+
        /// <summary>
        /// Получить pril_z и kuda
        /// </summary>
        /// <param name="kudaList"></param>
        /// <param name="npril"></param>
        /// <param name="prilzList"></param>
        void GetPrilzAndKudaList(List<decimal> kudaList, string npril, List<pril_zM> prilzList)
        {
            //читаем приложение из IZVV
            var _izvv = (from p in Model.db.izvv where p.nom == npril select p).ToList();
      
            foreach(var v in _izvv) // для каждой записи приложения...
     
            {
                var kudaStr = LetterToDigit(v.kuda.ToString().Trim());
                var kudaIsStr = v.isk_ot.ToString().Trim().PadLeft(2, '0');
                var kuda = Convert.ToDecimal(kudaStr + "," + kudaIsStr);//сформировали чертеж КУДА
                //
               // var spec = v.spec.ToString().Trim();
               // var kob = v.k_ob.ToString().Trim();
                //
                var whatStr = LetterToDigit(v.what.ToString().Trim());
                var whatIsStr = v.is_ot.ToString().Trim().PadLeft(2, '0');
                var what = Convert.ToDecimal(whatStr + "," + whatIsStr);//сформировали чертеж ЧТО
                //
                if (!kudaList.Contains(kuda)) kudaList.Add(kuda);//в список KUDALIST добавляем уникальное КУДА
            //в PRILZLIST добавляем записи из IZVV добавив заказ, №, сформированные чертежи Куда и Что
                prilzList.Add(new pril_zM
                {
                    zak =(decimal)Model.pTbOrder,
                    nom =(decimal) Model.pTbNumber,
                    ko = (decimal)v.k_ob,
                    poz = (decimal)v.posit,
                    what = what,
                    kol = (decimal)v.quant,
                    kuda = kuda,
                    spec = (decimal)v.spec,
                    path = v.path,
                    km = (decimal)v.km,
                    norm = (decimal)v.norm,
                    dd = 0,
                    ksi = (decimal)v.ksi,
                    dat = Convert.ToDateTime(v.data),
                    norm_p = npril,
                    r_zag = v.r_zag,
                    k_det = (decimal)v.k_det
                });
            }
            //

            var rt = (from p in prilzList join o in kudaList on p.what equals o where p.ko == 2 select p).ToList();//формируем список ДСЕ входящих в в удаляемые узлы - ошибка
            if (rt.Count > 0)
            {
                foreach (var l in rt)
                    AddTextToRtbInfo("\nОшибка в ДД - есть изменения в удаляемом узле  :" + "\nПозиция: " + l.poz + "\tЧертёж: " + (decimal)l.what + "\t Узел: " + (decimal)l.kuda + "\nПроверьте в ДД  записи, входящие в вышеуказанный узел \n", true);
            }

        }
        /// <summary>
        /// Преобразовывает буквенно-цифровой чертёж в цифровой
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        //+
        string LetterToDigit(string inputStr)
        {
            string charStr = "", digStr = "";
            //
            foreach (var c in inputStr)
            {
                if (!char.IsDigit(c)) charStr += c;
            }
            //
            if (charStr == "") return inputStr;
            else
            {
                Model.cbList.TryGetValue(charStr, out digStr);
                return inputStr.Replace(charStr, digStr);
            }
        }
        /// <summary>
        /// Проверка узлов по таблице Out
        /// </summary>
        /// <param name="list"></param>
        // +
        string CheckOut(List<pril_zM> list, decimal draft)
        {
            var str = "";
            str += DateTime.Now + "\n ";
            str += "\n*****************************************************************************************************";
            str += "\n* Сверка позиций приложения с OUT, если найдены позиции которых нет в общем виде                                            *";
            str += "\n* - проверьте не касаются ли позиции приложения с КО=2 или КО=3 ДСЕ вводимых в данном приложении                *";
            str += "\n*  -  если это так, то можно формировать заказ далее                                                                                               *";
            str += "\n*****************************************************************************************************";
            str += "   ";
            AddTextToRtbInfo(str, false);
            str = "";
            //
            foreach (var l in list.Where(z => z.ko != 1))
            {
                var _out = (from p in Model.db.@out where p.to == draft && p.across == l.kuda select p).ToList();
                if (_out.Count==0)
                    str += "\nПозиция: " + l.poz + "\tЧертёж: " + (decimal)l.what + "\t Узел: " + (decimal)l.kuda + " - нет в общем виде.";
            }
            //
            if (str != "")
            {
                AddTextToRtbInfo("____________________________________________________________________________", false);
                AddTextToRtbInfo(DateTime.Now + "\t Заказ/Номер: " + Model.pTbOrder.ToString() + "/" + Model.pTbNumber.ToString() + "\t Чертёж: " + (decimal)draft, false);
                AddTextToRtbInfo(str, false);
                AddTextToRtbInfo("____________________________________________________________________________", false);
            }
            else
            {
                //                    AddTextToRtbInfo("____________________________________________________________________________", false);
                AddTextToRtbInfo(DateTime.Now + " Сверка прошла - О Ш И Б О К   Н Е Т!", false);
                AddTextToRtbInfo("*****************************************************************************************************", false);
            }
            //
            return str;
        }
        /// <summary>
        /// Получить compl
        /// </summary>
        /// <param name="kudaList"></param>
        /// <param name="complList"></param>
        // +
        /// <summary>
        /// Получить compl
        /// </summary>
        /// <param name="kudaList"></param>
        /// <param name="complList"></param>
        void GetComplList(List<decimal> kudaList, List<complect> complList)
        {
            foreach (var kl in kudaList)
            {
                var _cmpl = (from p in Model.db.complect where p.kuda == kl select p).ToList();
                foreach(var v in _cmpl)
                {
                    complList.Add(new complect
                    {
                        format = v.format,
                        posit = v.posit,
                        what = v.what,
                        kuda = v.kuda,
                        quant = v.quant,
                        ed = v.ed,
                        group = v.group,
                        spec = v.spec,
                        ksi = v.ksi,
                        path = v.path,
                        izv = v.izv,
                        dti = v.dti,
                        tfl = v.tfl
                    });
                }
            }
            //***** 28.04 2016 если в comlist попали записи с переменной частью то и подгружаем постоянную часть
            var grkl = (from p in complList where p.@group == 2 select p.kuda).Distinct().ToList();
            if (grkl.Count > 0)
            {
                foreach (var kl in grkl)
                {
                    var intkl = decimal.Truncate(kl);
                    if (intkl != kl)
                    {
                        var _cml = (from p in Model.db.complect where p.kuda == intkl && p.@group == 1 select p).ToList();
                       
                        foreach(var v in _cml)
                        {
                            //****************************
                            var rrr = complList.Count(p => p.format == v.format && p.posit == v.posit && p.what == v.what && p.kuda == v.kuda && p.quant == v.quant && p.ed == v.ed && p.group == v.group && p.spec == v.spec && p.ksi == v.ksi && p.path == v.path && p.izv == v.izv && p.dti == v.dti && p.tfl == v.tfl);

                            if(rrr==0)
                                //**************************************
                            complList.Add(new complect
                            {
                                format = v.format,
                                posit = v.posit,
                                what = v.what,
                                kuda = v.kuda,
                                quant = v.quant,
                                ed = v.ed,
                                group = v.group,
                                spec = v.spec,
                                ksi = v.ksi,
                                path = v.path,
                                izv = v.izv,
                                dti = v.dti,
                                tfl = v.tfl
                            });
                        }

                    }
                }
            }
            //////******************
        }
        /// <summary>
        /// Модифицировать complect
        /// </summary>
        /// <param name="prilzList"></param>
        /// <param name="compList"></param>
        // +
        void ModifyComplect(List<pril_zM> prilzList, List<complect> compList, string draft)
        {
            

            int del = 0, add = 0, change = 0;
            //
            AddTextToRtbInfo("____________________________________________________________________________", false);
            AddTextToRtbInfo("" + DateTime.Now + "\t Заказ/Номер: " + Model.pTbOrder + "/" + Model.pTbNumber + "\t Чертёж: " + draft.Replace(',', '.'), false);
            //
            foreach (var pl in prilzList)
            {
                var compl = compList.Find(z => z.kuda == pl.kuda);
                var query = "";
                //
                if (compl != null)
                    pl.dd = compl.group > 0 ? 2 : 0;
                else
                    AddTextToRtbInfo("Узла: " + pl.kuda + " нет в compl.", true);
                var _cmp = Model.db.complect.FirstOrDefault(p => p.what == pl.what);
                if (pl.ko != 2)
                {
                    //если не удаляемая запись...
                    if (_cmp==null)
                    { 
                        // если узла не было в комплекте вставляем фиктивную запись в узел 9999(хотя надо бы в 99999)- еще не разу не вставили
                        query = "INSERT INTO complect values ('',{0},{1},9999,{2},0,0,{3},{4},{5},{6},{7},''";
                        Model.db.ExecuteStoreCommand(query, pl.poz, pl.what, pl.kol, pl.spec, pl.ksi, pl.path, "ДД" + pl.zak.ToString(), "/" + pl.nom.ToString());
                        AddTextToRtbInfo("Вставлена в complect запись: what = " + (decimal)pl.what + " kuda = " + (decimal)pl.kuda, false);
                        if (pl.spec == 2)
                        {
                            // усли узел - вставляем заголовок что=куда рсп=1
                            query = "INSERT INTO complect values ('',{0},{1},{1},1,0,0,1,{2},{3},{4},{5},''";
                            Model.db.ExecuteStoreCommand(query, pl.poz, pl.what,pl.what,  pl.ksi, pl.path, "ДД" + pl.zak.ToString(), "/" + pl.nom.ToString());
                            AddTextToRtbInfo("Вставлена в complect запись: what = " + (decimal)pl.what + " kuda = " + (decimal)pl.what, false);
                        }
                    }
                }
                var res = compList.OrderBy(z => z.spec).SingleOrDefault(z => z.what == pl.what && z.posit == pl.poz && z.kuda == pl.kuda && z.spec == pl.spec && z.ksi == pl.ksi);
                var kudaStr = pl.kuda.ToString();
                var ind = kudaStr.IndexOf(",");
                var kudaEnd = Convert.ToDecimal(kudaStr.Substring(ind + 1));
                var what = (decimal)pl.what;
                var kuda = (decimal)pl.kuda;
               
                if (res != null)
                {
                    //если запись есть в CompList
                    if (pl.ko == 2)
                    {
                        //подлежит удалению...
                        var resus = compList.Where(p => p.kuda == pl.what).ToList();
                  
                        compList.Remove(res);
                        del++;//удалили
                    }
                    else if (pl.ko == 3)
                    {
                        //подлежит изменению...
                        res.quant = pl.kol; res.spec = pl.spec; res.path = pl.path;
                        change++;//внесли изменения...
                    }
                    else  //ошибка - уже есть вставляемая запись
                        AddTextToRtbInfo("ДД\tПозиция:" + pl.poz + "\tЧертёж:" + what + "\tУзел: " + kuda + "\tРСП: " + pl.spec + "\tКСИ: " + pl.ksi + "\t ko = 1 уже есть в сomplect", true);
                }
                else
                {
                    //записи нет в Complist///
                    if (pl.ko == 1)
                    {
                        //надо вставить....
                        compList.Add(new complect
                        {
                            posit = pl.poz,
                            kuda = pl.kuda,
                            what = pl.what,
                            quant = pl.kol,
                            spec = pl.spec,
                            path = pl.path,
                            ksi = pl.ksi,
                            group = pl.dd == 0 ? 0 : (kudaEnd > 0 ? 2 : 1)
                        });
                        add++;// вставили из приложения
                    }
                    else
                    {
                        // надо  изменить или удалить в групповой спецификации - отслеживаем изменения в постоянной части
                        //************28/04/2016 проверяем не переменная ли часть если да ищем и удаляем в постоянной
                        var intkuda = decimal.Truncate(pl.kuda);
                        var b1 = compList.Where(p => p.kuda == pl.kuda && p.group == 2).ToList();
                        if (b1.Count > 0)
                        {
                            var res1 = compList.OrderBy(z => z.spec).SingleOrDefault(z => z.what == pl.what && z.posit == pl.poz && z.kuda == intkuda && z.spec == pl.spec && z.ksi == pl.ksi && z.group == 1);
                            if (res1 != null)
                            {
                                if (pl.ko == 2)
                                {
                                    compList.Remove(res1);//удалили в постоянной части
                                    del++;
                                }
                                else
                                {
                                    res1.quant = pl.kol; res1.spec = pl.spec; res1.path = pl.path;// изменили в постоянной части
                                    change++;
                                }
                            }
                            else //ошибка - нет в комплекте постоянной части
                                AddTextToRtbInfo("ДД\tПозиция:" + pl.poz + "\tЧертёж: " + what + "\tУзел: " + kuda + "\tРСП: " + pl.spec + "\tКСИ: " + pl.ksi + "\t ko = " + pl.ko + " нет в сomplect", true);
                        }
                        else
                            //не нашли записи ни в одиночной ни в групповой спецификациях
                            AddTextToRtbInfo("ДД\tПозиция:" + pl.poz + "\tЧертёж: " + what + "\tУзел: " + kuda + "\tРСП: " + pl.spec + "\tКСИ: " + pl.ksi + "\t ko = " + pl.ko + " нет в сomplect", true);
                    }
                }
            }
            AddTextToRtbInfo("____________________________________________________________________________", false);
            AddTextToRtbInfo("ko = 3 изменено: " + change + "\tko = 2 удалено: " + del + "\tko = 1 добавлено: " + add, false);
            AddTextToRtbInfo(DateTime.Now + " CompList после модификации: " + compList.Count, false);
        }
        //+
        /// <summary>
        /// Создать и заполнить таблицу compl
        /// </summary>
        /// <param name="complList"></param>
        void CreateAndWriteComplTable(List<complect> complList)
        {
            Model.db.ExecuteStoreCommand("IF OBJECT_ID(N'TempDB..#compl', N'U') IS NOT NULL   DROP TABLE  #compl");
            var query = "CREATE TABLE #compl (format char(2), posit numeric(3, 0), what numeric(13, 2), kuda numeric(13, 2)," +
                        " quant numeric(11, 5), ed numeric(3, 0),[group] numeric(2, 0), spec numeric(2, 0), ksi numeric(2, 0)," +
                        " path char(46), izv char(6), dti char(6), tfl nchar(5))";
            var resm = Model.db.ExecuteStoreCommand(query);
            foreach (var pl in complList)
            {
                query = "INSERT INTO #compl VALUES({0}, {1}, {2}, {3}, {4},{5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})";
                Model.db.ExecuteStoreCommand(query, pl.format,pl.posit, pl.what, pl.kuda,pl.quant,pl.ed,pl.group, pl.spec, pl.ksi, pl.path, pl.izv, pl.dti,pl.tfl);
            }
        }
        /// <summary>
        /// Создать врменную таблицу #prilz и записать в неё данные
        /// </summary>
        // +
  
        public void CreateAndWritePrilzTmpTable(string tNameP, List<pril_zM> prilzList)//#prilz
        {
            Model.db.ExecuteStoreCommand("IF OBJECT_ID(N'TempDB..#prilz', N'U') IS NOT NULL   DROP TABLE  #prilz");
            DataClasses.globaltmptabl = false;
            var query = "CREATE TABLE #prilz([zak] [numeric](4, 0), [nom] [numeric](3, 0), [ko] [numeric](1, 0), [poz] [numeric](3, 0), [what] [numeric](13, 2), " +
                        "[kol] [numeric](11, 5), [kuda] [numeric](13, 2), [spec] [numeric](2, 0), [path] [char](46), [km] [numeric](10, 0), [norm] [numeric](11, 5), " +
                        "[dd] [numeric](5, 0), [ksi] [numeric](1, 0), [dat] [date], [nom_p] [char](20), [r_zag] [char](20),[k_det] [numeric](5, 0))";
            var resm = Model.db.ExecuteStoreCommand(query);
            DataClasses.globaltmptabl = true;
            //
            foreach (var pl in prilzList)
            {
                query = "INSERT INTO #prilz VALUES({0}, {1},{2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13},{14}, {15}, {16})";
                Model.db.ExecuteStoreCommand(query, pl.zak, pl.nom, pl.ko, pl.poz, pl.what, pl.kol, pl.kuda, pl.spec, pl.path, pl.km, pl.norm, pl.dd,pl.ksi,pl.dat,pl.norm_p,pl.r_zag,pl.k_det);
            }
        }
        /// <summary>
        /// Сформировать OutPro
        /// </summary>
        /// <param name="draft"></param>
        // +
        CommandStatus FormOutPro(decimal draft, int osn, List<pril_zM> prilzList, bool showMessageFlag)
        {
            CommandStatus status = CommandStatus.EXECUTED;
            var normErr = 0;
            var tName = "#outpro";
            var query = "";
            //
            var prilzErrList = new List<pril_zM>();
            //
            CreateOutProTable();//создание временного #Outpro
           
            NormOfMaterialsDD(draft, Convert.ToBoolean(osn), Convert.ToInt32(Model.pTbOrder), Convert.ToInt32(Model.pTbNumber));//разузлование, расчет норм и запись в #Outpro
           
            if (Model.tip == "4") ModifyPartsCompl();
            //
           query = "SELECT * FROM #outpro";
           var resm=Model.db.ExecuteStoreQuery<outproM>(query).ToList();
          //  query = "SELECT * FROM " + tName;
            //var sqlData = SQLHandler.GetSQL(query);
            Model.listOutPro = new List<outpro>();
            //
            foreach(var v in resm)
          //  while (sqlData.Read())
            {
                var km = (decimal?)v.km;
                var norm = (decimal?)v.norm;
                var draftTmp = (decimal?)v.draft;
                var across = (decimal?)v.across;
                var spec = (decimal?)v.spec;
                Model.mt += v.cost;
                Model.nv += v.nv;
                Model.zp += v.zp;
                //
                Model.listOutPro.Add(new outpro
                {
                    zakaz = v.zakaz,
                    nom = v.nom,
                    posit = (decimal?)v.posit,
                    draft = draftTmp,
                    quant = (decimal?)v.quant,
                    across = across,
                    knk = (decimal?)v.knk,
                    ksi = v.ksi,
                    spec = spec,
                    rung = (decimal?)v.rung,
                    summ = (decimal?)v.summ,
                    path = v.path!=null?v.path.TrimEnd(new[] { '-', ' ' }):" ",
                    km = (decimal?)v.km,
                    norm = (decimal?)v.norm,
                    kz = (decimal?)v.kz,
                    p_nm = v.p_nm,
                    p_obm = v.p_obm,
                    p_tr = v.p_tr,
                    mg_pl = (decimal?)v.mg_pl,
                    p_pec = v.p_pec,
                    mg_vd = v.mg_vd,
                    mg_sp = v.mg_sp,
                    imcom = (decimal?)v.imcom,
                    nom_nar = (decimal?)v.nom_nar,
                    p_ved = v.p_ved,
                    p_neo = v.p_neo,
                    g_nar = (decimal?)v.g_nar,
                    p_cex = v.p_cex,
                    ro = (decimal?)v.ro,
                    d_opl = v.d_opl,
                    d_dok =v.d_dok,
                    blok = v.blok,
                    cop = (decimal?)v.cop,
                    normold = (decimal?)v.normold,
                    norm_ob = (decimal?)v.norm_ob,   //nv,//****************************
                    vari = (decimal?)v.vari,
                    cid = v.cid,
                    pid = (int?)v.pid
                });
                //
                if (spec != 1 && spec != 2 && spec != 7 && spec != 8 && spec != 22)
                {
                    if (km == null || km == 0 || norm == null || norm == 0)
                    {
                        //
                        if (normErr == 0) AddTextToRtbInfo("__________________________________________________________________________________________________", false);
                        //
                        AddTextToRtbInfo("\n Чертёж: " + (decimal)draftTmp + "\tУзел: " + (decimal)across + "\tКод материала: " + km + "\tНорма: " + norm, false);
                        normErr++;
                    }
                }
            }
            Model.ppmt = Model.mt; Model.ppnv = Model.nv; Model.ppzp = Model.zp;
            if (Model.ppnv == 0)
            {
                status = CommandStatus.FAILED;
                AddTextToRtbInfo("\n В заказе трудоемкость = 0! - Ошибка!!!", true);
                if (showMessageFlag)
                {
                    MessageBox.Show("В заказе трудоемкость = 0! - Ошибка!!!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //
           // sqlData.Close();
            //
            if (osn == 0 && Model.tip != "4")// если появится решение по з/ч со скл.67 -убрать проверку на тип=4
            {
                CheckReplaces();
                //
                if (Model.replaces > 0)
                {
                    AddTextToRtbInfo(DateTime.Now + " В sp_drzam найдено замен: " + Model.replaces, false);
                    Model.pContWrite = "Запись в z_dt7 и out_bnsi";//           View.btnWrite.Content = "Запись в z_dt7 и out_bnsi";
                    Model.pWrBG = "DarkOrange";// View.btnWrite.Background = new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    Model.pContWrite = "Запись в OUTPRO";
                    //Model.pWrBG = "Black";
                }
                //
                if (Model.tip != "4")
                {
                    CheckPrilzVsOutpro(prilzErrList, prilzList);
                    //
                    if (prilzErrList.Count > 0)
                    {
                        status = CommandStatus.FAILED;
                        AddTextToRtbInfo("__________________________________________________________________________________________________", true);
                        AddTextToRtbInfo("Ошибки в ДД: ", true);
                        //
                        foreach (var pz in prilzErrList)
                        {
                            var str = "В сформированном outpro нет узла для заданной позиции:\n";
                            //
                            if (pz.ko == 3) str = "В сформированном outpro не такой позициии:\n";
                            //
                            AddTextToRtbInfo(str + "ДД - чертёж: " + (decimal)pz.what +
                                                   "\tузел: " + (decimal)pz.kuda +
                                                   "\tspec: " + pz.spec + " ksi: " + pz.ksi +
                                                   "\tпозиция: " + pz.poz, true);
                        }
                    }
                }
            }
            //
            if (normErr > 0)
            {
                status = CommandStatus.FAILED;
                AddTextToRtbInfo("В заказе есть чертежи с нулевым кодом материала/нормой!", true);
                if (showMessageFlag)
                {
                    MessageBox.Show("В заказе есть чертежи с нулевым кодом материала/нормой!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
                //
                View.Hide();
                View.Show();
                Model.pWrBG = "Red"; //View.btnWrite.Background = new SolidColorBrush(Colors.Red);
            }
            //
            Model.pListOutPro = Model.listOutPro.OrderBy(z => z.pid).ThenBy(x => x.cid).ToList();
            var uz = Model.pListOutPro.Where(p => p.spec == 2 || p.spec == 7 || p.spec == 8).ToList();
            AddTextToRtbInfo("__________________________________________________________________________________________________", false);
                foreach(var w in uz)
                {
                   var cn= Model.pListOutPro.Count(p=>p.across==w.draft);
                    if(cn==0)
                    {
                        status = CommandStatus.FAILED;
                              AddTextToRtbInfo("В Н И М А Н И Е ! Проверьте состав  узла : "+(decimal) w.draft+ " в нем нет входящих позиций!", true);
                    }
                }
            //View.dgResult.ItemsSource = Model.listOutPro.OrderBy(z => z.pid).ThenBy(x => x.cid).ToList();
            Model.pgbResult = Model.pgbResult + "(" + Model.listOutPro.Count + ")";// View.gbResult.Header = Model.pgbResult + "(" + Model.listOutPro.Count + ")";
            return status;
        }
        /// <summary>
        /// Создать таблицу #outpro
        /// </summary>
        // +
        void CreateOutProTable()//#outpro
        {
            Model.db.ExecuteStoreCommand("IF OBJECT_ID(N'TempDB..#outpro', N'U') IS NOT NULL   DROP TABLE  #outpro");
            var query = "CREATE TABLE #outpro([zakaz] [numeric](4, 0), [nom] [numeric](3, 0), [posit] [numeric](4, 0), [draft] [numeric](13, 2), " +
                        "[quant] [numeric](11, 5), [across] [numeric](13, 2), [knk] [numeric](11, 5), [ksi] [numeric](2, 0), [spec] [numeric](2, 0), " +
                        "[rung] [numeric](2, 0), [summ] [numeric](11, 5), [path] [char](46), [km] [numeric](10, 0), [norm] [numeric](11, 5), " +
                        "[kz] [numeric](10, 0), [p_nm] [date], [p_obm] [date], [p_tr] [date], [mg_pl] [numeric](4, 0), [p_pec] [date], " +
                        "[mg_vd] [date], [mg_sp] [date], [imcom] [numeric](1, 0), [nom_nar] [numeric](4, 0), [p_ved] [date], [p_neo] [date], " +
                        "[g_nar] [numeric](4, 0), [p_cex] [date], [ro] [numeric](5, 0), [d_opl] [date], [d_dok] [date], [blok] [char](30), " +
                        "[cop] [numeric](1, 0), [normold] [numeric](11, 5), [norm_ob] [numeric](11, 5), [vari] [numeric](1, 0), " +
                        "[cost] [numeric](11, 2), [nv] [numeric](9, 4), [zp] [numeric](9, 2), [cid] [int], [pid] int )";//9.3 nv
            var resm = Model.db.ExecuteStoreCommand(query);
        }
        /// <summary>
        /// Получение норм материалов с ДД
        /// </summary>
        /// <param name="draft">Чертёж</param>
        /// <param name="osn">Оснастка: 0 - нет, 1 - да</param>
        /// <param name="order">Заказ</param>
        /// <param name="number">Номер</param>
        /// Пишет во временную таблицу #Outpro
        //+  
        public  void NormOfMaterialsDD(decimal draft, bool osn, int order, int number)
        {

            var dr = (decimal)draft;
            var os = (bool)osn;
            var acrossTmp = (decimal)(Model.pTbOrder * 1000 + Model.pTbNumber);
            if (number >= 900 || CheckDraft(dr, os))
            {
                decimal curNomnar = 1;
                var listOut = CallUnnodingDD(dr, os);
                //
                var listItog = GetMaterialsDD(listOut, Model.pTbOrder, Model.pTbNumber);
                //
                var listOutPro = (from lo in listOut
                                  join li in listItog
                                      on lo.id equals li.id
                                  select new outproM
                                  {
                                      zakaz = (decimal)Model.pTbOrder,
                                      nom = (decimal)Model.pTbNumber,
                                      posit = lo.posit,
                                      draft = lo.what,
                                      quant = lo.quant,
                                      knk = lo.knk,
                                      ksi = lo.ksi,
                                      across = lo.kuda,
                                      spec = lo.spec,
                                      rung = lo.level,
                                      summ = lo.summ,
                                      path = lo.path,
                                      km = Convert.ToDecimal(li.km),
                                      norm = li.norm,
                                      id = lo.id,
                                      pid = lo.pid,
                                      cost = li.cost
                                  }
                                 ).OrderBy(z => z.rung).ThenBy(x => x.posit).ThenBy(y => y.draft).ToList();
                //
                SetNomnar(listOutPro, dr, 0, 1, ref curNomnar);
                //
                foreach (var lop in listOutPro)
                {
                    if (!os)
                    {
                        if (number < 900) if (lop.draft == lop.across) lop.draft = 0;
                    }
                    else
                    {
                        if (lop.draft == lop.across)
                        {
                            lop.across = acrossTmp;
                            lop.spec = 2;
                        }
                        else
                        {
                            if (lop.draft != 0) lop.rung++;
                        }
                    }
                    //
                    //if (lop.draft == 24396000001.00m)
                    //    MessageBox.Show("y");
                    if (lop.p_tr == null)
                    {
                        if (!(lop.spec == 6 || (lop.spec == 3 && lop.ksi == 3) || (lop.spec == 5 && lop.ksi == 3) || (lop.spec == 4 && lop.ksi == 3) || (lop.spec == 4 && lop.ksi == 4)))
                        //if (lop.spec == 1 || lop.spec == 2 || lop.spec == 7 || lop.spec == 8 || lop.spec == 22 ||
                        //     (lop.spec == 3 || lop.spec == 4 || lop.spec == 5) && (lop.ksi == 0 || lop.ksi == 2 || lop.ksi == 4)
                        //   )
                        {
                            char[] _sqlch = lop.path.ToArray();
                                

                            var tuple = CheckRouteList(GetRouteList(_sqlch),  osn, (lop.draft == 0 ? lop.across : lop.draft), lop.knk);
                            //
                            if (tuple.Item3) lop.p_tr = DateTime.Now;
                            //
                            lop.nv = tuple.Item1;
                            lop.zp = tuple.Item2;
                        }
                        else lop.p_tr = DateTime.Now;
                    }
                }
                //
                if (os) listOutPro = new List<outproM>{new outproM{zakaz = (decimal)Model.pTbOrder, nom = (decimal)Model.pTbNumber, quant = 1, across = acrossTmp,
                                                                         knk = 1, spec = 1, rung = 1, summ = 1, p_nm = DateTime.Now,ksi=0,path="", 
                                                                         p_obm = DateTime.Now, p_tr = DateTime.Now, p_pec = DateTime.Now, 
                                                                         p_ved = DateTime.Now, cop = 1, id = -1, pid = -1}
                                                             }.Concat(listOutPro).ToList();
                foreach (var lop in listOutPro)
                {
                    if (osn)
                    {
                        if (!(lop.id == 0 && lop.pid == 0)) lop.pid++;
                        //
                        lop.id++;
                    }
                    if (lop.km == 0)
                        lop.p_obm = DateTime.Now;
                    var query = "INSERT INTO #outpro ([zakaz], [nom], [posit], [draft], [quant], [across], [knk], [ksi], [spec], [rung] " +
                         ",[summ], [path], [km], [norm], [kz], [p_nm], [p_obm], [p_tr], [mg_pl], [p_pec], "+
                         " [mg_vd], [mg_sp], [imcom], " +
                         "[nom_nar], [p_ved], [p_neo], [g_nar], [p_cex], [ro], [d_opl], [d_dok], [blok], "+
                         "[cop], [normold], [norm_ob], [vari], [cost], [nv], [zp], [cid], [pid] ) " +
                         "VALUES ({0},{1}, {2}, {3}, {4},{5}, {6}, {7},{8}, {9}"+
                         ",{10}, {11}, {12}, {13}, 0, {14},{15},{16}, 0, {17},"+
                         " NULL, NULL, 0,"
                         +"{18},{19},"+
                         "NULL,0,NULL,0,NULL,NULL,'',"
                         +"{20}, 0, 0, 0, {21},{22},{23},{24},{25})";
                    var resm = Model.db.ExecuteStoreCommand(query,lop.zakaz,lop.nom,lop.posit,lop.draft,lop.quant,lop.across,lop.knk,lop.ksi,lop.spec,lop.rung,//9
                        lop.summ,lop.path, lop.km,lop.norm,DateTime.Now, lop.p_obm,lop.p_tr,lop.p_pec,//17
                        lop.nom_nar, lop.p_ved,
                        lop.cop, lop.cost, lop.nv, lop.zp, lop.id, lop.pid).ToString();//25
                }
            }
        }
        /// <summary>
        /// Проверить чертёж
        /// </summary>
        //+
         bool CheckDraft(decimal draft, bool osn)
        {
            if (osn)
            {
                var _cmp = Model.db.ocomplect.FirstOrDefault(p => p.kuda == draft);
                if(_cmp==null) return false;
                else
                    return true;
            }
            else
            {
                 var _cmp = Model.db.complect.FirstOrDefault(p => p.kuda == draft);
                 if (_cmp == null) return false;
                 else
                     return true;
            }
            //var query = "SELECT TOP 1 kuda FROM FOX.dbo.";
            ////
            //if (osn) query += "o";
            //query += "complect WHERE kuda = " + (SqlDecimal)draft;
            ////
            //using (var reader = new SqlCommand(query, connection).ExecuteReader()) return reader.HasRows;
        }
        /// <summary>
        /// Вызов разузлования с ДД
        /// </summary>
        /// <param name="draft"></param>
        /// <param name="osn"></param>
        /// <returns></returns>
        //+
          List<Out> CallUnnodingDD(decimal draft, bool osn)
         {
             var list = new List<Out>();
             int level = 0, id = 0;
             //
             DoUnnodingDD(draft, draft, osn,  ref list, 1, ref level, ref id, 0);
             list = list.Where(z => (z.what == draft && z.spec == 1) || z.spec != 1).ToList();
             //
             var querySumm = (from l in list
                              group l by new { l.what } into pre
                              select new { pre.Key.what, summ = pre.Sum(z => z.knk) });
             var listOut = (from l in list
                            join q in querySumm on l.what equals q.what
                            select new Out
                            {
                                format = l.format,
                                posit = l.posit,
                                what = l.what,
                                kuda = l.kuda,
                                quant = l.quant,
                                ed = l.ed,
                                @group = l.@group,
                                spec = l.spec,
                                ksi = l.ksi,
                                path = l.path,
                                root = l.root,
                                level = l.level,
                                id = l.id,
                                pid = l.pid,
                                knk = l.spec == 1 ? l.quant : l.knk,
                                summ = l.spec == 1 ? l.quant : q.summ,
                                what_dse = l.what_dse,
                                kuda_dse = l.kuda_dse
                            }
                             ).ToList();
             return listOut;
         }
        /// <summary>
        /// Разузлование с ДД
        /// </summary>
        /// <param name="draft"></param>
        /// <param name="Root"></param>
        /// <param name="osn"></param>
        /// <param name="list"></param>
        /// <param name="quantity"></param>
        /// <param name="Level"></param>
        /// <param name="Id"></param>
        /// <param name="Pid"></param>
        //+
           void DoUnnodingDD(decimal draft, decimal Root, bool osn,  ref List<Out> list, decimal quantity, ref int Level, ref int Id, int Pid)
          {
             
              string query = "";
              if (osn)
              {
                   query = "SELECT C.*,(CASE WHEN L.DSE IS NOT NULL THEN L.DSE WHEN M.hm IS NOT NULL THEN M.hm WHEN LF.NM IS NOT NULL THEN LF.NM WHEN P.name IS NOT NULL THEN P.name ELSE '' END) AS what_DSE";
                  query += " FROM ocomplect AS C";
                  query += " LEFT JOIN olistdse AS L ON C.what  = L.DRAFT";
                  query += " LEFT JOIN FOX.dbo.m_cennik AS M ON CAST(C.what AS DECIMAL(11, 0)) = M.km_num";
                  query += " LEFT JOIN FOX.dbo.LIST_FR AS LF ON C.what = LF.WHAT";
                  query += " LEFT JOIN (SELECT TOP(1) draft, name FROM prodo WHERE draft>={0} AND draft<={1}  AND name IS NOT NULL) AS P ON C.what = P.draft";
                  query += " WHERE C.kuda >= {0} AND C.kuda <= {1} ORDER BY posit, spec";
              }
              else
              {
                   query = "SELECT C.*,(CASE WHEN L.DSE IS NOT NULL THEN L.DSE WHEN M.hm IS NOT NULL THEN M.hm WHEN LF.NM IS NOT NULL THEN LF.NM WHEN P.name IS NOT NULL THEN P.name ELSE '' END) AS what_DSE";
                  query += " FROM complect AS C";
                  query += " LEFT JOIN listdse AS L ON CAST (C.what / 1000 AS INT) = L.DRAFT";
                  query += " LEFT JOIN FOX.dbo.m_cennik AS M ON CAST(C.what AS DECIMAL(11, 0)) = M.km_num";
                  query += " LEFT JOIN FOX.dbo.LIST_FR AS LF ON C.what = LF.WHAT";
                  query += " LEFT JOIN (SELECT TOP(1) draft, name FROM prodact WHERE draft>={0} AND draft<={1}  AND name IS NOT NULL) AS P ON C.what = P.draft";
                  query += " WHERE C.kuda >= {0} AND C.kuda <= {1} ORDER BY posit, spec";
              }
              var queryCompl = "SELECT C.*,(CASE WHEN L.DSE IS NOT NULL THEN L.DSE WHEN M.hm IS NOT NULL THEN M.hm WHEN LF.NM IS NOT NULL THEN LF.NM WHEN P.name IS NOT NULL THEN P.name ELSE '' END) AS what_DSE";
              queryCompl += " FROM #compl AS C";
              queryCompl += " LEFT JOIN listdse AS L ON CAST (C.what / 1000 AS INT) = L.DRAFT";
              queryCompl += " LEFT JOIN FOX.dbo.m_cennik AS M ON CAST(C.what AS DECIMAL(11, 0)) = M.km_num";
              queryCompl += " LEFT JOIN FOX.dbo.LIST_FR AS LF ON C.what = LF.WHAT";
              queryCompl += " LEFT JOIN (SELECT TOP(1) draft, name FROM prodact WHERE draft>={0} AND draft<={1}  AND name IS NOT NULL) AS P ON C.what = P.draft";
              queryCompl += " WHERE C.kuda >= {0} AND C.kuda <= {1} ORDER BY posit, spec";
             
              var draftInt = decimal.Truncate(draft);
            
              var listNodes = new List<Out>();
              var listPre = new List<Out>();
              List<OutM> _cur = new List<OutM>();
              
              var IsCompl = true;
             
              if (!osn)
              {
                 
                  _cur = Model.db.ExecuteStoreQuery<OutM>(queryCompl, draftInt, draft).ToList();
                 
                  if (_cur.Count == 0)
                      IsCompl = false;
                  else
                  {
                      if (_cur.FirstOrDefault(p => p.kuda == draft) == null)
                          IsCompl = false;
                      else
                          IsCompl = true;
                  }
                
              }
              else IsCompl = false;
              

              if (!IsCompl)
              {
                 
                  _cur = Model.db.ExecuteStoreQuery<OutM>(query, draftInt, draft).ToList();
                  if (_cur.Count == 0)
                      MessageBox.Show("Нет узла " + draft.ToString());
              }
              
              Level++;
              var kuda_dse = "не найдено";
              
               foreach(var v in _cur)
              {
                  var spec = (decimal?)v.spec;
                  var what_dse = v.what_dse;
                  //
                  if (spec == 1) kuda_dse = what_dse;
                  //
                  listPre.Add(new Out
                  {
                      format = (string)v.format,
                      posit = (decimal)v.posit,
                      what = (decimal)v.what,
                      kuda = (decimal)v.kuda,
                      quant = (decimal)v.quant,
                      ed = (decimal?)v.ed,
                      group = (decimal)v.group,
                      spec = spec,
                      ksi = (decimal?)v.ksi,
                      path = v.path,
                      what_dse = what_dse,
                      kuda_dse = kuda_dse
                  });
              }
              //
          
              //
              var listGroup = listPre.Where(p => p.kuda == draft && p.group > 0).ToList();//==0  ************4.04.2016
              //
              if (listGroup.Count > 0)//<=0  //**************************4.04.2016
              {
                  listGroup = listPre.Where(p => p.kuda == draft && p.group == 2).ToList();
                  //
                  if (listGroup.Count > 0)
                  {
                      var listGropup1 = listPre.Where(p => decimal.Truncate(p.kuda) == draftInt && p.group == 1).ToList();
                      //
                      foreach (var l in listGropup1) l.kuda = draft;
                      //
                      listGroup = listGroup.Concat(listGropup1).ToList();
                  }
              }
              //моя вставка********************4.04.2016
              var llist0 = listPre.Where(p => p.kuda == draft && p.group == 0).ToList();
              listGroup = listGroup.Concat(llist0).OrderBy(p => p.spec).ToList();
              //**********************************
              //
              foreach (var l in listGroup)
              {
                  l.root = Root; l.level = Level; l.id = Id; l.pid = Pid; l.knk = (quantity * l.quant);
                  list.Add(l);
                  //
                  Id++;
                  //
                  if (l.spec == 2 || l.spec == 7 || l.spec == 8) listNodes.Add(l);
              }
              //
              foreach (var r in listNodes)
              {

                  DoUnnodingDD(r.what, Root, osn,  ref list, r.knk, ref Level, ref Id, r.id);
                  Level--;
              }
          }
        /// <summary>
        /// формирование ИТОГ
        /// </summary>
        /// <param name="list"></param>
        /// <param name="order"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        //+
            List<Itog> GetMaterialsDD( List<Out> list, decimal? order, decimal? number)
           {
               var listItog = new List<Itog>();
             
              // var lookInPrilz = Model.db.ExecuteStoreCommand("IF (OBJECT_ID(N'TempDB..#prilz', N'U') IS NOT NULL) select 0");
               var lookInPrilz = DataClasses.globaltmptabl == true ? 1 : 0;
           
               foreach (var l in list)
               {
                   var itog = new Itog();
                   var res = false;
                   // Литьё, поковка, штамп, покупное по чертежу (доработка)
                   if (l.spec == 3 && l.ksi == 2)
                   {
                       res = GetMCennikDataDr( ref itog, l.path, ((decimal)l.what).ToString().PadLeft(14, '0').Replace(',','.'));
                       //

                       if (!res)
                       {
                           AddTextToRtbInfo("Ошибка: в m_cennik нет кода ТМЦ для "+(((decimal)l.what).ToString().PadLeft(14, '0').Replace(',','.'))+"  с признаком менеджера", true);
                       }
                       itog.norm = l.knk; itog.potrebnost = l.knk; itog.cost = l.knk * itog.hzp;
                       itog.normn = l.knk; itog.costn = itog.cost;
                   }
                   // Материалы специфицированные и вспомогательные (без трудовых нормативов)
                   else if (l.spec == 6)
                   {
                       decimal norm = 0, normn = 0;
                       //
                       if (lookInPrilz>0)
                       {
                           var tup = GetNormFromPrilzDD( l.what, l.kuda, l.posit, order, number);
                           norm = tup.Item1;
                           normn = norm;
                       }
                       //
                       if (norm == 0)
                       {
                           var _t = (from p in Model.db.material where p.mater == l.what && p.whatt == l.kuda && p.posit == l.posit select new { norm = p.norm, normn = p.normn }).ToList();
                           if (_t.Count >0)
                           {
                               norm = (decimal)_t[0].norm;
                               normn = (decimal)_t[0].normn;
                           }
                       }
                       res = GetMCennikData( ref itog, l.path,  decimal.Truncate(l.what));
                       //
                       var norm_tmp = (l.knk / l.quant) * norm;
                       itog.norm = norm_tmp; itog.potrebnost = norm_tmp; itog.cost = norm_tmp * itog.hzp;
                       //
                       var normN_tmp = (l.knk / l.quant) * normn;
                       itog.normn = normN_tmp; itog.costn = normN_tmp * itog.hzp;

                   }
                   // 3 0 Деталь собственного изготовления, входящая в изделие
                   // 3 4 Покупной крепёж с доработкой
                   // 4 0 Деталь собственного изготовления по стандартам преприятия
                   // 4 4 Покупной крепёж (без доработки)
                   else if ((l.spec == 3 || l.spec == 4) && (l.ksi == 0 || l.ksi == 4))
                   {
                       decimal norm_det = 0, matrl = 0, norm_detn = 0;
                       //
                       if (lookInPrilz>0)
                       {
                           var tup = GetNormFromPrilzDD( l.what, l.kuda, l.posit, order, number);
                           norm_det = tup.Item1;
                           matrl = tup.Item2;
                           norm_detn = norm_det;
                       }
                       //
                       if (norm_det == 0)
                       {
                           var _tt = (from p in Model.db.technol
                                      where p.whattech == l.what && p.meth_prod == l.ksi
                                      select new
                                          {
                                              norm_det = p.norm_det,
                                              norm_detn = p.norm_detn,
                                              matrl = p.matrl
                                          }).ToList();
                           if (_tt.Count > 0)
                           {
                               norm_det = (decimal)_tt[0].norm_det;
                               matrl = (decimal)_tt[0].matrl;
                               norm_detn = (decimal)_tt[0].norm_detn;
                           }
                       }
                       //
                       res = GetMCennikData( ref itog, l.path,  matrl);
                       //
                       var norm_tmp = l.knk * norm_det;
                       itog.norm = norm_tmp; itog.potrebnost = norm_tmp; itog.cost = norm_tmp * itog.hzp;
                       //
                       var normN_tmp = l.knk * norm_detn;
                       itog.normn = normN_tmp; itog.costn = normN_tmp * itog.hzp;
                   }
                   // 3 3 Покупная деталь
                   // 4 3 Стандартное изделие
                   // 5 3 Покупное изделие без доработки (без трудовых нормативов)
                   // 5 4 Покупное изделие с доработкой
                   else if ((l.spec == 3 || l.spec == 4 || l.spec == 5) && (l.ksi == 3) || (l.spec == 5 && l.ksi == 4))
                   {
                       res = GetMCennikData( ref itog, l.path,  decimal.Truncate(l.what));
                       //
                       itog.norm = l.knk; itog.potrebnost = l.knk; itog.cost = l.knk * itog.hzp;
                       itog.normn = l.knk; itog.costn = itog.cost;
                   }
                   //
                   if (!res) { itog.prt = l.spec.ToString() + "_" + l.ksi.ToString(); itog.hm = l.what.ToString(); }
                   //
                   itog.id = l.id;
                   listItog.Add(itog);
               }
               //
               return listItog;
           }
        /// <summary>
        /// Формирование записи в иток по КМ
        /// </summary>
        /// <param name="itog"></param>
        /// <param name="lpath"></param>
        /// <param name="km_n"></param>
        /// <returns></returns>
        //+
             bool GetMCennikData( ref Itog itog, string lpath, decimal km_n)
            {
                var res = false;
                var query = "SELECT pr, km, fio, hm, gst, prt, gsts, hzp, data, ei FROM m_cennik LEFT JOIN polzov ON pr = kod WHERE km_num={0}";
             var _mc=   Model.db.ExecuteStoreQuery<ClMCSmall>(query, km_n);
               
                 foreach(var v in _mc )
              
                {
                    res = true;
                    var path = "";
                    var ind = lpath.IndexOf("-");
                    //
                    if (ind > 0) path = lpath.Substring(ind + 1, 3);
                    //
                    itog.pr = v.pr;
                    itog.km = v.km;
                    itog.fio = v.fio;
                    itog.hm = v.hm;
                    itog.gst = v.gst;
                    itog.prt = v.prt;
                    itog.gsts = v.gsts;
                     itog.hzp = (decimal)v.hzp;
                     itog.data = v.data; itog.ei = v.ei; itog.cpn = path;

                    
                }
                
                return res;
            }
        /// <summary>
        /// Формирование записи в итог для 3.2
        /// </summary>
        /// <param name="itog"></param>
        /// <param name="lpath"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        //+
             bool GetMCennikDataDr(ref Itog itog, string lpath,string dr)
             {
                 var res = false;
                     var query = "SELECT pr, km, fio, hm, gst, prt, gsts, hzp, data, ei FROM m_cennik LEFT JOIN polzov ON pr = kod WHERE ocen={0}";
                     var _mc = Model.db.ExecuteStoreQuery<ClMCSmall>(query,dr).OrderBy(p=>p.km).ToList();
                     while (_mc.Count > 0 && _mc.FirstOrDefault(p => p.pr.Trim() == "")!=null) 
                     {

                         var ttt = _mc.FirstOrDefault(p => p.pr.Trim() == "");
                             if (ttt != null)
                                 _mc.Remove(ttt);
                        
                     }
                     foreach (var v in _mc)
                 {
                     res = true;
                     var path = "";
                     var ind = lpath.IndexOf("-");
                     //
                     if (ind > 0) path = lpath.Substring(ind + 1, 3);
                     //
                     itog.pr = v.pr;
                     itog.km = v.km;
                     itog.fio = v.fio;
                     itog.hm = v.hm;
                     itog.gst = v.gst;
                     itog.prt = v.prt;
                     itog.gsts = v.gsts;
                     itog.hzp = (decimal)v.hzp;
                     itog.data = v.data; itog.ei = v.ei; itog.cpn = path;

                     //itog.pr = reader[0].ToString(); itog.km = reader[1].ToString(); itog.fio = reader[2].ToString(); itog.hm = reader[3].ToString();
                     //itog.gst = reader[4].ToString(); itog.prt = reader[5].ToString(); itog.gsts = reader[6].ToString(); itog.hzp = (decimal)reader[7];
                     //itog.data = (DateTime)reader[8]; itog.ei = reader[9].ToString(); itog.cpn = path;
                     ////
                     break;
                 }
                 //
              //   reader.Close();
                 //
                 return res;
             }
        /// <summary>
        /// Получить норму расхода материала из приложения
        /// </summary>
        /// <param name="what"></param>
        /// <param name="kuda"></param>
        /// <param name="posit"></param>
        /// <param name="order"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        //+
             Tuple<decimal, decimal> GetNormFromPrilzDD( decimal what, decimal kuda, decimal posit, decimal? order, decimal? number)
             {
                

                 decimal norm = 0, km = 0;
                 //
                 var query = "SELECT norm, km FROM #prilz WHERE zak = {0} AND nom ={1} AND what = {2} AND kuda ={3} AND poz = {4}";
                 var _mc = Model.db.ExecuteStoreQuery<pril_zM>(query, order, number, what, kuda, posit);

                 foreach(var v in _mc)
                 {
                     norm = v.norm;
                     km = v.km;
                 }
                 return new Tuple<decimal, decimal>(norm, km);
             }
        /// <summary>
        /// Установка номеров нарядов
        /// </summary>
        /// <param name="listOutPro"></param>
        /// <param name="across"></param>
        /// <param name="pid"></param>
        /// <param name="acrNomnar"></param>
        /// <param name="curNomnar"></param>
        //+
             void SetNomnar(List<outproM> listOutPro, decimal across, int pid, decimal acrNomnar, ref decimal curNomnar)
             {
                 var listTmp = listOutPro.Where(z => z.across == across && z.pid == pid).OrderBy(x => x.id);
                 var nodeList = new List<outproM>();
                 //
                 foreach (var lt in listTmp)
                 {
                     if (lt.draft == 0) lt.nom_nar = 1;
                     //
                     if (((lt.spec == 3 || lt.spec == 4 || lt.spec == 5) && lt.ksi == 3) || (lt.spec == 6 )) lt.nom_nar = acrNomnar;
                     else lt.nom_nar = curNomnar++;
                     //
                     if (lt.spec == 2 || lt.spec == 7 || lt.spec == 8) nodeList.Add(lt);
                 }
                 //
                 foreach (var nl in nodeList)
                 {
                     SetNomnar(listOutPro, nl.draft, nl.id, nl.nom_nar, ref curNomnar);
                 }
             }
        /// <summary>
        /// Проверка маршрута и подсчет трудовых и з/п
        /// </summary>
        /// <param name="routeList"></param>
        /// <param name="osn"></param>
        /// <param name="draft"></param>
        /// <param name="knk"></param>
        /// <returns></returns>
        //+
           Tuple<decimal, decimal, bool> CheckRouteList(List<Route> routeList,  bool osn, decimal draft, decimal knk)
             {
                 var qPart = "";
                 decimal nv = 0, zp = 0;
                 //
                 if (osn) qPart = "o";
                 //
                 //if (draft == 24396000001.00m)
                 //    MessageBox.Show("y");
                 //var rr_ = (from p in Model.db.rr select p).ToList();
                 foreach (var rl in routeList)
                 {
                     var _rr =Model.rr_.FirstOrDefault(p => p.cex.PadRight(3) == rl.shop.PadRight(3));
                     if (_rr != null && _rr.notr==1)
                         rl.check = true;
                   
                 }
                 //
                 var notTr = routeList.Count(z => !z.check);
                 var cntTr = 0;
                 string sh = "";
                 //
                 foreach (var rl in routeList)
                 {
                     sh = rl.shop.PadRight(3);
                     var list = new List<Tuple<decimal, decimal, decimal>>();
                     if (osn)
                     {
                         var _opn = (from p in Model.db.ooper_n where p.okv == draft && p.ucex == sh && p.zah == rl.pass select new { normv = p.normv, kei = p.kei == 0 ? 1 : p.kei, rpab = p.rpab, gr_opl = p.gr_opl }).ToList();
                         if (_opn.Count > 0)
                             
                         {
                             if (!rl.check) cntTr++;
                             foreach(var v in _opn)
                             list.Add(new Tuple<decimal, decimal, decimal>(knk * (decimal)v.normv / Convert.ToDecimal(Math.Pow(10.0, Convert.ToDouble(v.kei - 1))), (decimal)v.rpab, (decimal)v.gr_opl));
                         }
                        
                     }
                     else 
                     {
                         var _opn = (from p in Model.db.oper_n where p.okv == draft && p.ucex == sh && p.zah == rl.pass select new { normv = p.normv, kei = p.kei == 0 ? 1 : p.kei, rpab = p.rpab, gr_opl = p.gr_opl }).ToList();
                         if (_opn.Count > 0)
                         {
                             if (!rl.check) cntTr++;
                             foreach (var v in _opn)
                                 list.Add(new Tuple<decimal, decimal, decimal>(knk * (decimal)v.normv / Convert.ToDecimal(Math.Pow(10.0, Convert.ToDouble(v.kei - 1))), (decimal)v.rpab, (decimal)v.gr_opl));
                         }
                         
                     }
                     

                    
                     string sitem2 = "";
                   //  var tf_ = (from p in Model.db.tarift select p).ToList();
                     foreach (var l in list)
                     {
                         nv += l.Item1;
                         sitem2 = l.Item2.ToString();
                         var _tf = Model.tf_.Where(p => p.stavka == l.Item3 && p.raz ==sitem2).ToList();
                         foreach( var y in _tf)
                         {
                             zp=zp+l.Item1*(decimal)y.tarif;
                         }
                        
                     }
                 }
                 //
                 return new Tuple<decimal, decimal, bool>(nv, zp, notTr == cntTr);
             }
        /// <summary>
        /// Получить маршрут
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        //+
           List<Route> GetRouteList(char [] route)
           {
               var routeList = new List<Route>();
               var tmpStr = "";
               var shopCount = 0;
               var prevRoute = new Route();
               //
               for (int j = 0; j < route.Length; j++)
               {
                   if (route[j] != '-') tmpStr += route[j];
                   else shopCount++;
                   //
                   if (route[j] == '-' || j == route.Length - 1)
                   {
                       tmpStr = tmpStr.Trim();
                       //
                       if (shopCount >= 1 && tmpStr.Length > 0 && !(tmpStr.Contains("[") || tmpStr.Contains("]")))
                       {
                           var tmpRoute = routeList.FindLast(z => z.shop == tmpStr);
                           var pass = 1;
                           //
                           if (tmpRoute != null) pass = tmpRoute.pass + 1;
                           //
                           routeList.Add(new Route { shop = tmpStr, pass = pass });
                       }
                       //
                       tmpStr = "";
                   }
               }
               //
               foreach (var rl in routeList)
               {
                   if (rl.shop.Contains("()")) prevRoute.check = true;
                   //
                   prevRoute = rl;
               }
               for (int lll = 0; lll < routeList.Count; lll++)
               {
                   if (routeList[lll].check)
                   {
                       for (int llo = lll + 1; llo < routeList.Count; llo++)
                       {
                           if (routeList[llo].shop == routeList[lll].shop && routeList[llo].check == routeList[llo].check)
                               routeList[llo].pass = routeList[llo].pass - 1;
                       }
                   }

               }
               return routeList.Where(z => !z.check && z.shop != "()").ToList();
           }
           /// <summary>
           /// Обновить норму в Outpro
           /// </summary>
           // +
           void ModifyPartsCompl()
           {
               foreach (var lp in Model.listParts)
               {
                   var query = "UPDATE #outpro SET norm = {0} WHERE draft = {1} AND across = {2}";
                  
                   Model.db.ExecuteStoreCommand(query, lp.quant, lp.what, lp.kuda);
               }
           }
           /// <summary>
           /// Проверить на замены из sp_drzam
           /// </summary>
           /// <returns></returns>
           //+
           void CheckReplaces()
           {
               var dtMkart = (from p in Model.db.m_kart select p).ToList();
               var dtRashgbe = (from p in Model.db.rash_gbe select p).ToList();
               var dtSpdrzam = (from p in Model.db.sp_drzam select p).ToList();
               foreach (var op in Model.listOutPro)
               {
                   foreach( var r in dtSpdrzam)
                   {
                       if (r.draft == op.draft)
                       {
                           var km = r.km;
                           decimal q1 = 0, qp = 0, qr = 0, rSum = 0;
                           var find = 0;
                           //
                           foreach (var mk in dtMkart)
                           {
                               if (mk.km == km && mk.ns == "67")
                               {
                                   q1 += (decimal)mk.q1;
                                   qp += (decimal)mk.qp;
                                   qr += (decimal)mk.qr;
                                   //
                                   find++;
                               }
                           }
                           //
                           if (find > 0)
                           {
                               foreach (var rg in dtRashgbe)
                               {
                                   if ((decimal)rg.km == Convert.ToDecimal(km) && rg.datv.ToString() == "") rSum += (decimal)rg.kol;
                               }
                               //
                               if ((q1 + qp - qr - rSum) > 0)
                               {
                                   Model.replaces++;
                               }
                           }
                           //
                           break;
                       }
                   }
               }
           }
           /// <summary>
           /// Проверить соответствие prilz и сформированного outpro
           /// </summary>
           /// <returns></returns>
           //+
           void CheckPrilzVsOutpro(List<pril_zM> prilzErrList, List<pril_zM> prilzList)
           {
               foreach (var pz in prilzList)
               {
                   var opFind = Model.listOutPro.Find(op => pz.what == op.draft && pz.kuda == op.across &&
                   pz.spec == op.spec && pz.ksi == op.ksi && pz.poz == op.posit);

                   if (opFind == null)
                    {
                        prilzErrList.Add(pz);
                    }
                    
               }
           }
           /// <summary>
           /// Получить лист compl для запчастей
           /// </summary>
           /// <param name="complList"></param>
           /// <param name="draft"></param>
           // +
           void GetPartsComplList(List<complect> complList, decimal draft, bool showMessageFlag)
           {
               var _mg4 = (from p in Model.db.mg405 where p.kzt == Model.pTbOrder && p.kzz == Model.pTbNumber select new {ocv=p.ocv,@is=p.@is,kv=p.kv,ki=p.ki }).ToList();
               //var query = "SELECT ocv, [is], kv, ki FROM FOX.dbo.mg405 WHERE kzt = " + Model.order + " AND kzz = " + Model.number;
               //var sqlData = SQLHandler.GetSQL(query);
               var mgList = new List<mg>();
               var mgErr = 0;
               //
               complList.Add(new complect { posit = 0, what = draft, kuda = draft, quant = 1, group = 0, spec = 1, ksi = 0, path = "   -   -16" });
               //
               foreach(var v in _mg4)
              // while (sqlData.Read())
               {
                   var kv = (decimal)v.kv;
                   var ki = (decimal)v.ki;
                   var k = kv - ki;
                   var ocv = (decimal)v.ocv;
                   var @is = (decimal)v.@is;
                   var what = (decimal)(ocv + Convert.ToDecimal("0," + @is.ToString().PadLeft(2, '0')));
                   //
                   if (k > 0) mgList.Add(new mg { okv = ocv, @is = @is, k = k, path = "" });
                   else
                   {
                       AddTextToRtbInfo("\n Чертёж: " + what + " Количество включено: " + kv + " Количество на складе: " + ki, false);
                       mgErr++;
                   }
               }
               //
           //    sqlData.Close();
               //
               if (mgErr > 0)
               {
                    if (showMessageFlag)
                    {
                    MessageBox.Show("Предупреждение! \n В mg405 количество на складе и в заказе совпадают! \n В заказ для изготовления ДСЕ не включается", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
                   //
                   Model.pWrBG = "Red";// View.btnWrite.Background = new SolidColorBrush(Colors.Red);
               }
               //
               foreach (var m in mgList)
               {
                   var shopList = new List<string>();
                   var @is = Convert.ToDecimal("0," + m.@is.ToString().PadLeft(2, '0'));
                   var what = m.okv + @is;
                   var _n189 = (from p in Model.db.n189 where p.ocv == m.okv && p.@is == m.@is select p).ToList();
                   //query = "SELECT * FROM FOX.dbo.n189 WHERE ocv = " + (SqlDecimal)m.okv + " AND [is] = " + (SqlDecimal)m.@is;
                   //sqlData = SQLHandler.GetSQL(query);
                   //
                   foreach(var v in _n189)
                   //while (sqlData.Read())
                   {
                       var spec = (decimal)v.spec;
                       var ksi = (decimal)v.ksi;
                       //
                 //      for (int i = 1; i < 10; i++) shopList.Add(sqlData["c" + i].ToString());
                       shopList.Add(v.c1);
                       shopList.Add(v.c2);
                       shopList.Add(v.c3);
                       shopList.Add(v.c4);
                       shopList.Add(v.c5);
                       shopList.Add(v.c6);
                       shopList.Add(v.c7);
                       shopList.Add(v.c8);
                       shopList.Add(v.c9);
                     
                       //
                       m.path = SplitRoute(v.skl, v.cp, shopList);
                       //
                       if (!(spec == 1 || spec == 2 || spec == 7 || spec == 8))
                       {
                           complList.Add(new complect { what = what, kuda = draft, quant = m.k, spec = spec, ksi = ksi, path = m.path });
                       }
                       //
                       break;
                   }
                   //
                   //sqlData.Close();
                   //
                   var w = Model.db.complect.OrderBy(p=>p.group).FirstOrDefault(p => p.what == what);
                   //query = "SELECT TOP 1 [format],[posit],[what],[kuda],[quant],[ed],[group],[spec],[ksi],[path],[izv],[dti],[tfl] " +
                   //        " FROM FOX.dbo.complect WHERE what = " + (SqlDecimal)what;
                   //sqlData = SQLHandler.GetSQL(query);
                   //
                   //while (sqlData.Read())
                   if(w!=null)
                   {
                       var spec = (decimal)w.spec;
                       //
                       if (complList.Find(z => z.what == what) == null) complList.Add(new complect
                       {
                           format = w.format,
                           posit = w.posit,
                           what = w.what,
                           kuda = draft,
                           quant = m.k,
                           ed = w.ed,
                           group = w.group,
                           spec = spec == 1 ? 2 : spec,
                           ksi = w.ksi,
                           path = m.path != "" ? m.path : w.path,
                           izv = w.izv,
                           dti =w.dti,
                           tfl = w.tfl
                       });
                   }
                   //
                //   sqlData.Close();
               }
               //
               complList = complList.OrderBy(z => z.spec).ThenBy(x => x.what).ToList();
               Model.listParts = complList.Where(z => z.spec == 6 && z.ksi == 0).ToList();  // GetPartsComplList
        }
           /// <summary>
           /// Склеивает маршрут из 9-ти частей
           /// </summary>
           /// <param name="skl"></param>
           /// <param name="cp"></param>
           /// <param name="shopList"></param>
           /// <returns></returns>
           //+
           string SplitRoute(string skl, string cp, List<string> shopList)
           {
               var path = skl + "-" + cp;
               //
               foreach (var sl in shopList)
               {
                   var slTmp = sl.Trim();
                   //
                   if (slTmp.Length > 0) path += "-" + slTmp.PadRight(3);
               }
               //
               return path;
           }
           /// <summary>
           /// Получить prod для оснастки
           /// </summary>
           //+
           decimal GetProdList()
           {
               decimal dr = 0;
               var _pr = Model.db.prod.FirstOrDefault(p => p.zakaz == Model.pTbOrder && p.nom == Model.pTbNumber).dr;
               dr = (decimal)_pr;
               return dr;
           }
           /// <summary>
           /// Записать из #prilz в pril_z
           /// </summary>
           //+
           void WriteFromPrilzTmpToPrilz() //Запись в Prilz from temp
           {
               var query = "DELETE FROM FOX.dbo.pril_z WHERE zak = {0} AND nom = {1}";
               //
               if (Model.tip == "1" || Model.tip == "2" || Model.tip == "3" || Model.tip == "5")
               {
                   var listItog = new List<Itog>();
               

                   var query1 = "SELECT norm, km FROM #prilz WHERE zak = {0} AND nom ={1}";
                   var lookInPrilz = Model.db.ExecuteStoreQuery<pril_zM>(query1, Model.pTbOrder, Model.pTbNumber).ToList().Count();

                   if (lookInPrilz>0)
                   {
                       var resm = Model.db.ExecuteStoreCommand(query, Model.pTbOrder, Model.pTbNumber);
                       //
                       query = "INSERT INTO FOX.dbo.pril_z SELECT [zak], [nom], [ko], [poz], [what], [kol], [kuda], [spec], [path], [km], [norm], " +
                               "[dd], [ksi], [dat], [nom_p], [r_zag], [k_det] FROM #prilz";
                       
                       AddTextToRtbInfo("Вставлено в pril_z строк: " + Model.db.ExecuteStoreCommand(query).ToString(), false);
                       //
                       query = "DROP TABLE #prilz";
                       Model.db.ExecuteStoreCommand(query);
                   }
               }
           }
           /// <summary>
           /// Записать в z_td7 и outbnsi
           /// </summary>
           //+
           void WriteZ_td7AndOutBnsi()//Запись в Out_bnsi & Z_td7
           {
               try
               {
                   string query = "";
                   var tst = Model.db.z_td7.FirstOrDefault(p => p.zakaz == Model.pTbOrder && p.nom == Model.pTbNumber);
                   var tsto = Model.db.out_bnsi.FirstOrDefault(p => p.zakaz == Model.pTbOrder && p.nom == Model.pTbNumber);
                   if (tst == null && tsto == null)
                   {
                       query = "INSERT INTO FOX.dbo.z_td7 (zakaz, nom, dtt, dtz, dt_i, dt_f, st_po, st_sv, tip) " +
                          "VALUES({0}, {1}, NULL, {2}, NULL, NULL, 0, 0, {3})";
                       var resm = Model.db.ExecuteStoreCommand(query, Model.pTbOrder, Model.pTbNumber, DateTime.Now, Model.tip);
                       AddTextToRtbInfo("Вставлено в z_td7 строк: " + resm.ToString(), false);

                       //
                       query = "INSERT INTO FOX.dbo.out_bnsi " +
                               "([zakaz], [nom], [posit], [draft], [quant], [across], [knk], [ksi], [spec], [rung],[summ], [path], [km], [norm], [kz], " +
                               "[p_nm], [p_obm], [p_tr], [mg_pl], [p_pec], [mg_vd],[mg_sp], [imcom], [nom_nar], [p_ved], [p_neo], [g_nar], [p_cex], " +
                               "[ro], [d_opl], [d_dok], [blok], [cop], [normold], [norm_ob], [vari], [cid], [pid]) " +
                               "SELECT [zakaz], [nom], [posit], [draft], [quant], [across], [knk], [ksi], [spec], [rung], " +
                               "[summ], [path], [km], [norm], [kz], [p_nm], [p_obm], [p_tr], [mg_pl], [p_pec], [mg_vd],[mg_sp], [imcom], [nom_nar], " +
                               "[p_ved], [p_neo], [g_nar], [p_cex], [ro], [d_opl], [d_dok], [blok], [cop], [normold], [norm_ob], [vari], [cid], [pid] FROM #outpro";
                       resm = Model.db.ExecuteStoreCommand(query);
                       AddTextToRtbInfo("Вставлено в outbnsi строк: " + resm.ToString(), false);
                       MailToClient.mailTO("Заказ " + Model.pTbOrder.ToString() + "№ " + Model.pTbNumber.ToString(), @"Примите решение по использованию п/ф в заказе", "", ReadId());

                       //var mail = new MailMessage(Environment.UserName + "@elsib.ru", ReadId(), @"Примите решение по использованию п/ф в заказе", "Заказ " + Model.order.ToString() + "№ " + Model.number.ToString());

                       //var smtpClient = new SmtpClient("192.168.5.2");
                       //smtpClient.Send(mail);
                   }
                   else
                       MessageBox.Show("Заказ уже есть в OUT_BNSI И Z_td7");

               }
               
               catch (Exception ex)
               {
                   CheckStartUp.WriteErr(ex, @"writeztd7");
                   System.Windows.MessageBox.Show("Произошла ошибка,\n информация передана разработчику");

               } 

           }
        /// <summary>
        /// Список адресов Email
        /// </summary>
        /// <returns></returns>
        //+
           public string ReadId()
           {
               var xDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"\eMail_adress.frx");
               return xDoc.Root.Element("ListEml").Value; ;
           }
           /// <summary>
           /// Записать в outpro и pl_god
           /// </summary>
           //+
           void WriteIntoOutproAndPlgod()//Запись в Outpro  & Pl_god
           {
               try
               {
                   var qe = "select top (1) * from FOX.dbo.outpro  where zakaz={0} and nom={1}";
                   var re = Model.db.ExecuteStoreQuery<outpro>(qe, Model.pTbOrder, Model.pTbNumber).ToList().Count();
                   if (re == 0)
                   {
                       var query = "INSERT INTO FOX.dbo.outpro SELECT [zakaz], [nom], [posit], [draft], [quant], [across], [knk], [ksi], [spec], [rung], " +
                                   "[summ], [path], [km], [norm], [kz], [p_nm], [p_obm], [p_tr], [mg_pl], [p_pec], [mg_vd],[mg_sp], [imcom], [nom_nar], " +
                                   "[p_ved], [p_neo], [g_nar], [p_cex], [ro], [d_opl], [d_dok], [blok], [cop], [normold], [norm_ob], [vari], [cid], [pid] FROM #outpro";
                       var resm = Model.db.ExecuteStoreCommand(query);
                       AddTextToRtbInfo("Вставлено в outpro строк: " + resm.ToString(), false);
                       //
                       query = "UPDATE FOX.dbo.pl_god SET mt ={0}, nv = {1}, zp = {2} WHERE zakaz = {3} AND nom = {4}";
                       resm = Model.db.ExecuteStoreCommand(query, Model.mt, Model.nv, Model.zp, Model.pTbOrder, Model.pTbNumber);
                       AddTextToRtbInfo("Обновлено в pl_god: " + resm.ToString(), false);
                       //********************
                       string Us = mParceFio(CheckStartUp.UserFIO);
                       string Bm = Model.pTbOrder.ToString() + " " + Model.pTbNumber.ToString()+" MT"+(Model.mt>0?"1":"0")+" NV"+(Model.nv>0?"1":"0")+" ZP"+(Model.zp>0?"1":"0");
                       DateTime Mom = DateTime.Now;


                       string mcsql = " INSERT INTO App_Log (App_name,User_name,Button_name,Moment) values({0},{1},{2},{3})";

                       Model.db.ExecuteStoreCommand(mcsql, "FormationZakaz", Us, Bm, Mom);
                       //*****************
                   }
                   else
                       MessageBox.Show("Заказ уже есть в OUTPRO");
               }
               
               catch (Exception ex)
               {
                   CheckStartUp.WriteErr(ex, @"WriteOutpro");
                   System.Windows.MessageBox.Show("Произошла ошибка,\n информация передана разработчику");

               } 

           }
           public string mParceFio(string pfio)
           {
               string tfio = "";

               string[] words = pfio.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
               if (words.Count() > 2)
               {
                   string im = words[1].Substring(0, 1) + ".";
                   string ot = words[2].Substring(0, 1) + ".";
                   tfio = words[0] + " " + im + ot;
               }
               else
                   tfio = pfio;

               return tfio;
           }

        private void _mLoadOrders()
        {
            if (Model.pIsAllSelected)
            {
                Model.pIsAllSelected = false;
            }
            //DateTime firstDate = DateTime.Now.Date;
            //DateTime secondDate = DateTime.Now.Date;
            DateTime firstDate = new DateTime(2024, 12, 1); 
            DateTime secondDate = new DateTime(2024, 12, 14); 

            List<Order> orders = Model.mGetOrders(firstDate, secondDate);
            Model.mUpdateOrders(orders);
        }

        // Метод для старта вычислений
        private void _mStartCalculation()
        {
            _mChangeProgressBarVisibility();
            _mChangeAccessCommandElementsEnabled();

            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }

        // Метод для выполнения работы в фоновом потоке
        private void _mWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CommandStatus status;
            bool showMessageFlag = false;
            var selectedOrders = Model.mGetSelectedOrders();
            int totalOrders = selectedOrders.Count;
            double completedOrders = 0;

            foreach (var order in selectedOrders)
            {  
                // Рассчитываем прогресс
                int progressPercentage = (int)((completedOrders / totalOrders) * 100);

                // Обновляем прогресс в UI потоке
                _worker.ReportProgress(progressPercentage);

                Model.pSelectedOrder = order;
                Model.pTbOrder = order.OrderID;
                Model.pTbNumber = order.Number;

                // Долгая операция
                status = mBtnCalc(showMessageFlag);

                order.Log = Model.pTextBlock;
                order.Result = Model.pListOutPro;
                order.CountResult = order.Result.Count;

                if (status == CommandStatus.EXECUTED)
                {
                    order.pStatus = OrderStatus.COMPLECTED;
                }
                else
                {
                    order.pStatus = OrderStatus.NOT_COMPLECTED;
                }
                completedOrders++;
            }
        }

        // Обработчик для обновления прогресс-бара
        private void _mWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Model.pProgress = e.ProgressPercentage;
        }


        // Код, выполняемый после завершения задачи
        private void _mWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _mChangeProgressBarVisibility();
            _mChangeAccessCommandElementsEnabled();
        }

        private void _mChangeProgressBarVisibility()
        {
            Model.pIsProgressVisible = !Model.pIsProgressVisible;
        }

        private void _mChangeAccessCommandElementsEnabled()
        {
            Model.pAccessCommandElementsForOneOrder = !Model.pAccessCommandElementsForOneOrder;
            Model.pAccessCommandElementsForOrders = !Model.pAccessCommandElementsForOrders;
        }

        private void _mSetAllOrdersSelection(bool isSelected)
        {
            foreach (var order in Model.pOrders)
            {
                order.IsSelected = isSelected;
            }
        }

        private void _mBtnCalcWithFlag()
        {
            mBtnCalc(true);
        }

        #endregion

        #region new methods for CalcOutPro

        private void _mFindOrderInOutpro()
        {
            
        }

        private bool _mCheckOrderInOutpro()
        {
            return false;
        }


        private void _mShowMsgBox()
        {

        }

        private void _mFindOrderInPl_god()
        {

        }

        private void _mCheckOrderInPl_god()
        {

        }

        private void _mCheckOrderInProduct()
        {

        }

        private void _mIsMainProduct()
        {

        }

        private void _mHasApplication()
        {

        }

        #region new methods for replace GetPrilzAndKudaList

        private List<izvv> _mGetApplicationData(string applicationNumber)
        {
            return (from p in Model.db.izvv where p.nom == applicationNumber select p).ToList();
        }

        private List<pril_zM> _mGenerateApplicationData(izvv record, List<decimal> nodesList)
        {
            List<pril_zM> applicationData = new List<pril_zM>();

            string nodeStr = LetterToDigit(record.kuda.ToString().Trim());
            string nodeIsStr = record.isk_ot.ToString().Trim().PadLeft(2, '0');
            decimal node = Convert.ToDecimal(nodeStr + "," + nodeIsStr);
            _mAddToNodesList(node, nodesList);

            var targetStr = LetterToDigit(record.what.ToString().Trim());
            var targetIsStr = record.is_ot.ToString().Trim().PadLeft(2, '0');
            var target = Convert.ToDecimal(targetStr + "," + targetIsStr);

            applicationData.Add(new pril_zM
            {
                zak = (decimal)Model.pTbOrder,
                nom = (decimal)Model.pTbNumber,
                ko = (decimal)record.k_ob,
                poz = (decimal)record.posit,
                what = target,
                kol = (decimal)record.quant,
                kuda = node,
                spec = (decimal)record.spec,
                path = record.path,
                km = (decimal)record.km,
                norm = (decimal)record.norm,
                dd = 0,
                ksi = (decimal)record.ksi,
                dat = Convert.ToDateTime(record.data),
                norm_p = record.nom,
                r_zag = record.r_zag,
                k_det = (decimal)record.k_det
            });

            return applicationData;
        }

        private void _mProcessApplication(string applicationNumber)
        {
            var nodesList = new List<decimal>();
            var applicationsList = new List<pril_zM>();

            var applicationData = _mGetApplicationData(applicationNumber);

            foreach (var record in applicationData)
            {
                // Формируем значение узла и добавляем его в nodesList
                var nodeStr = LetterToDigit(record.kuda.ToString().Trim());
                var nodeIsStr = record.isk_ot.ToString().Trim().PadLeft(2, '0');
                var node = Convert.ToDecimal(nodeStr + "," + nodeIsStr);
                
                // Генерируем данные приложения и добавляем их в общий список
                var generatedData = _mGenerateApplicationData(record, nodesList);
                applicationsList.AddRange(generatedData);
            }

            var errors = _mValidateApplicationData(applicationsList, nodesList);

            if (errors.Any())
            {
                _mLogApplicationErrors(errors);
            }
        }

        private void _mAddToNodesList(decimal node, List<decimal> nodesList)
        {
            if (!nodesList.Contains(node))
            {
                nodesList.Add(node);
            }    
        }

        private List<pril_zM> _mValidateApplicationData(List<pril_zM> applicationsList, List<decimal> nodesList)
        {
            return (from p in applicationsList
                    join o in nodesList on p.what equals o
                    where p.ko == 2
                    select p).ToList();
        }

        private void _mLogApplicationErrors(List<pril_zM> errorsList)
        {
            foreach (var error in errorsList)
            {
                AddTextToRtbInfo($"\nОшибка в ДД - есть изменения в удаляемом узле  :" +
                    $"\nПозиция: {error.poz}" +
                    $"\tЧертёж: {error.what}" +
                    $"\tУзел: {error.kuda}" +
                    "\nПроверьте в ДД записи, входящие в вышеуказанный узел \n", true);
            }
        }

        #endregion new methods for replace GetPrilzAndKudaList

        #region new methods for replace CheckOut

        /// <summary>
        /// Генерирует заголовок для сверки позиций приложения с OUT.
        /// </summary>
        /// <returns>Строка, содержащая заголовок для сверки.</returns>
        private string _mGenerateComparisonHeader()
        {
            var header = DateTime.Now + "\n ";
            header += $"\n{'*' * 100}";
            header += "\n* Сверка позиций приложения с OUT, если найдены позиции которых нет в общем виде                  *";
            header += "\n* - проверьте не касаются ли позиции приложения с КО=2 или КО=3 ДСЕ вводимых в данном приложении  *";
            header += "\n*  -  если это так, то можно формировать заказ далее                                                                                               *";
            header += $"\n{'*' * 100}";
            header += "   ";
            return header;
        }

        /// <summary>
        /// Получает список позиций, которые не найдены в общем виде (OUT), для сверки.
        /// </summary>
        /// <param name="list">Список позиций приложения для проверки.</param>
        /// <param name="draft">Чертёж для сверки.</param>
        /// <returns>Строка с описанием позиций, которых нет в общем виде.</returns>
        private string _mGetPositionsNotInOut(List<pril_zM> applicationsList, decimal draft)
        {
            var result = "";

            // Проверка каждой позиции, не относящейся к КО=1
            foreach (var l in applicationsList.Where(z => z.ko != 1))
            {
                // Проверка наличия позиции в общем виде по чертежу и узлу
                var _out = (from p in Model.db.@out where p.to == draft && p.across == l.kuda select p).ToList();
                if (_out.Count == 0)
                {
                    result += "\nПозиция: " + l.poz + "\tЧертёж: " + (decimal)l.what + "\t Узел: " + (decimal)l.kuda + " - нет в общем виде.";
                }
            }

            return result;
        }

        /// <summary>
        /// Логирует результаты сверки позиций в информационное окно.
        /// </summary>
        /// <param name="str">Строка с результатами сверки позиций.</param>
        /// <param name="draft">Чертёж для логирования.</param>
        private void _mLogComparisonResult(string str, decimal draft)
        {
            if (!string.IsNullOrEmpty(str))
            {
                // Логирование информации о сверке
                AddTextToRtbInfo($"{'-' * 100}", false);
                AddTextToRtbInfo(DateTime.Now + "\t Заказ/Номер: " + Model.pTbOrder.ToString() + "/" + Model.pTbNumber.ToString() + "\t Чертёж: " + (decimal)draft, false);
                AddTextToRtbInfo(str, false);
                AddTextToRtbInfo($"{'-' * 100}", false);
            }
            else
            {
                // Логирование сообщения об отсутствии ошибок
                AddTextToRtbInfo(DateTime.Now + " Сверка прошла - О Ш И Б О К   Н Е Т!", false);
                AddTextToRtbInfo("*****", false);
            }
        }

        /// <summary>
        /// Выполняет полную сверку позиций приложения с OUT, включая генерацию заголовка, проверку позиций и логирование результата.
        /// </summary>
        /// <param name="list">Список позиций приложения для сверки.</param>
        /// <param name="draft">Чертёж для сверки.</param>
        /// <returns>Результат сверки в виде строки.</returns>
        public string _mPerformComparisonCheck(List<pril_zM> list, decimal draft)
        {
            var str = _mGenerateComparisonHeader();
            AddTextToRtbInfo(str, false); // Логируем заголовок 

            // Проверка позиций
            str = _mGetPositionsNotInOut(list, draft);

            // Логирование результатов
            _mLogComparisonResult(str, draft);

            return str;
        }

        #endregion new methods for replace CheckOut

        #region new methods for replace GetComplList

        /// <summary>
        /// Метод для получения списка переменных частей (группа == 2).
        /// </summary>
        /// <param name="kitsList">Список комплектующих.</param>
        /// <returns>Список уникальных значений kuda (node/узел) для переменных частей.</returns>
        private List<decimal>_mGetVariableParts(List<complect> kitsList)
        {
            return (from p in kitsList where p.@group == 2 select p.kuda).Distinct().ToList();
        }

        /// <summary>
        /// Метод для получения постоянных частей для заданного узла.
        /// </summary>
        /// <param name="intNode">Узел, для которого ищем постоянные части.</param>
        /// <returns>Список постоянных частей для указанного узла.</returns>
        private List<complect> _mGetConstantParts(decimal intNode)
        {
            return (from p in Model.db.complect where p.kuda == intNode && p.@group == 1 select p).ToList();
        }

        /// <summary>
        /// Метод для добавления постоянной части в список, если такая еще не существует.
        /// </summary>
        /// <param name="kitsList">Список комплектующих, в который будет добавлена постоянная часть.</param>
        /// <param name="item">Постоянная часть, которая может быть добавлена в список.</param>
        private void _mAddConstantPartIfNotExist(List<complect> kitsList, complect item)
        {
            var existing = kitsList.Count(p =>
                p.format == item.format && p.posit == item.posit && p.what == item.what &&
                p.kuda == item.kuda && p.quant == item.quant && p.ed == item.ed &&
                p.group == item.group && p.spec == item.spec && p.ksi == item.ksi &&
                p.path == item.path && p.izv == item.izv && p.dti == item.dti && p.tfl == item.tfl);

            if (existing == 0)
            {
                kitsList.Add(new complect
                {
                    format = item.format,
                    posit = item.posit,
                    what = item.what,
                    kuda = item.kuda,
                    quant = item.quant,
                    ed = item.ed,
                    group = item.group,
                    spec = item.spec,
                    ksi = item.ksi,
                    path = item.path,
                    izv = item.izv,
                    dti = item.dti,
                    tfl = item.tfl
                });
            }
        }

        /// <summary>
        /// Метод для добавления постоянных частей, если они соответствуют переменным частям в списке комплектующих.
        /// </summary>
        /// <param name="kitsList">Список комплектующих, в который могут быть добавлены постоянные части.</param>
        /// <returns>Обновленный список комплектующих с добавленными постоянными частями.</returns>
        private List<complect> _mAddConstantPartIfNeeded(List<complect> kitsList)
        {
            var updatedKitsList = new List<complect>(kitsList);

            // 1. Поиск переменных частей
            var variableParts = _mGetVariableParts(updatedKitsList);

            // 2. Для каждой переменной части, проверяем и добавляем постоянную часть
            foreach (var node in variableParts)
            {
                var intNode = decimal.Truncate(node);
                if (intNode != node)
                {
                    var constantParts = _mGetConstantParts(intNode);
                    foreach (var item in constantParts)
                    {
                        // 3. Проверяем, не существует ли такая постоянная часть в списке, если нет, добавляем
                        _mAddConstantPartIfNotExist(updatedKitsList, item);
                    }
                }
            }

            return updatedKitsList;
        }

        #endregion new methods for replace GetComplList

        #region new methods for replace ModifyComplect

        void ModifyComplectNew(List<pril_zM> applicationsList, List<complect> kitsList, string draft)
        {
            int delCount = 0;
            int addCount = 0;
            int changeCount = 0;
            List<string> queryQueue = new List<string>();

            _mLogHeader(draft);

            foreach (var application in applicationsList)
            {
                ProcessApplicationRecord(application, kitsList, ref delCount, ref addCount, ref changeCount, queryQueue);
            }

            _mExecuteQueriesToInsertKits(queryQueue);
            LogFooter(kitsList.Count, delCount, addCount, changeCount);
        }

        void _mLogHeader(string draft)
        {
            AddTextToRtbInfo("________________", false);
            AddTextToRtbInfo($"{DateTime.Now}\t Заказ/Номер: {Model.pTbOrder}/{Model.pTbNumber}\t Чертёж: {draft.Replace(',', '.')}", false);
        }

        void LogFooter(int kitsListCount, int delCount, int addCount, int changeCount)
        {
            AddTextToRtbInfo("________________", false);
            AddTextToRtbInfo($"ko = 3 изменено: {changeCount}\tko = 2 удалено: {delCount}\tko = 1 добавлено: {addCount}", false);
            AddTextToRtbInfo($"{DateTime.Now} CompList после модификации: {kitsListCount}", false);
        }

        void ProcessApplicationRecord(pril_zM application, List<complect> kitsList, ref int delCount, ref int addCount, ref int changeCount, List<string> queryQueue)
        {
            _mValidateApplicationRecord(application, kitsList);
            _mCheckAndQueueInsertQueries(application, queryQueue);

            var existingKit = kitsList
                .OrderBy(kit => kit.spec)
                .SingleOrDefault(
                    kit =>
                    kit.what == application.what &&
                    kit.posit == application.poz &&
                    kit.kuda == application.kuda &&
                    kit.spec == application.spec &&
                    kit.ksi == application.ksi
                );

            if (existingKit != null)
            {
                _mHandleExistingkit(application, kitsList, existingKit, ref delCount, ref changeCount);
            }
            else
            {
                ProcessNewOrGroupedEntry(application, kitsList, ref addCount, ref delCount, ref changeCount);
            }
        }

        void _mValidateApplicationRecord(pril_zM pl, List<complect> compList)
        {
            var compl = compList.Find(z => z.kuda == pl.kuda);
            if (compl != null)
                pl.dd = compl.group > 0 ? 2 : 0;
            else
                AddTextToRtbInfo($"Узла: {pl.kuda} нет в compl.", true);
        }

        void _mCheckAndQueueInsertQueries(pril_zM pl, List<string> placeholderQueries)
        {
            var existingInDb = Model.db.complect.FirstOrDefault(p => p.what == pl.what);
            if (pl.ko != 2 && existingInDb == null)
            {
                _mAddInsertKitQueryToQueue(pl, placeholderQueries);
            }
        }

        void _mAddInsertKitQueryToQueue(pril_zM pl, List<string> placeholderQueries)
        {
            var query = "INSERT INTO complect values ('',{0},{1},9999,{2},0,0,{3},{4},{5},{6},{7},'')";
            placeholderQueries.Add(string.Format(query, pl.poz, pl.what, pl.kol, pl.spec, pl.ksi, pl.path, "ДД" + pl.zak, "/" + pl.nom));

            if (pl.spec == 2)
            {
                query = "INSERT INTO complect values ('',{0},{1},{1},1,0,0,1,{2},{3},{4},{5},'')";
                placeholderQueries.Add(string.Format(query, pl.poz, pl.what, pl.what, pl.ksi, pl.path, "ДД" + pl.zak, "/" + pl.nom));
            }
        }

        void _mExecuteQueriesToInsertKits(List<string> placeholderQueries)
        {
            foreach (var query in placeholderQueries)
            {
                Model.db.ExecuteStoreCommand(query);
            }
        }

        void _mHandleExistingkit(pril_zM pl, List<complect> compList, complect res, ref int del, ref int change)
        {
            if (pl.ko == 2)
            {
                compList.Remove(res);
                del++;
            }
            else if (pl.ko == 3)
            {
                res.quant = pl.kol;
                res.spec = pl.spec;
                res.path = pl.path;
                change++;
            }
            else
            {
                AddTextToRtbInfo($"ДД\tПозиция:{pl.poz}\tЧертёж:{pl.what}\tУзел: {pl.kuda}\tРСП: {pl.spec}\tКСИ: {pl.ksi}\t ko = 1 уже есть в сomplect", true);
            }
        }

        void ProcessNewOrGroupedEntry(pril_zM pl, List<complect> compList, ref int add, ref int del, ref int change)
        {
            if (pl.ko == 1)
            {
                _mAddNewKit(pl, compList, ref add);
            }
            else
            {
                _mProcessKitRecordBySpecifications(pl, compList, ref del, ref change);
            }
        }

        void _mAddNewKit(pril_zM pl, List<complect> compList, ref int add)
        {
            var kudaStr = pl.kuda.ToString();
            var ind = kudaStr.IndexOf(",");
            var kudaEnd = ind >= 0 ? Convert.ToDecimal(kudaStr.Substring(ind + 1)) : 0;

            compList.Add(new complect
            {
                posit = pl.poz,
                kuda = pl.kuda,
                what = pl.what,
                quant = pl.kol,
                spec = pl.spec,
                path = pl.path,
                ksi = pl.ksi,
                group = pl.dd == 0 ? 0 : (kudaEnd > 0 ? 2 : 1)
            });
            add++;
        }

        void _mProcessKitRecordBySpecifications(pril_zM pl, List<complect> compList, ref int del, ref int change)
        {
            var intkuda = decimal.Truncate(pl.kuda);
            var groupedEntries = compList.Where(p => p.kuda == pl.kuda && p.group == 2).ToList();

            if (groupedEntries.Any())
            {
                _mHandleKitRecord(pl, compList, intkuda, ref del, ref change);
            }
            else
            {
                LogMissingEntry(pl);
            }
        }

        void _mHandleKitRecord(pril_zM pl, List<complect> compList, decimal intkuda, ref int del, ref int change)
        {
            var fixedEntry = compList.OrderBy(z => z.spec)
                                     .SingleOrDefault(z => z.what == pl.what && z.posit == pl.poz && z.kuda == intkuda && z.spec == pl.spec && z.ksi == pl.ksi && z.group == 1);

            if (fixedEntry != null)
            {
                if (pl.ko == 2)
                {
                    compList.Remove(fixedEntry);
                    del++;
                }
                else
                {
                    fixedEntry.quant = pl.kol;
                    fixedEntry.spec = pl.spec;
                    fixedEntry.path = pl.path;
                    change++;
                }
            }
            else
            {
                LogMissingEntry(pl);
            }
        }

        void LogMissingEntry(pril_zM pl)
        {
            AddTextToRtbInfo($"ДД\tПозиция:{pl.poz}\tЧертёж: {pl.what}\tУзел: {pl.kuda}\tРСП: {pl.spec}\tКСИ: {pl.ksi}\t ko = {pl.ko} нет в сomplect", true);
        }

        #endregion new methods for replace ModifyComplect

        #endregion new methods for CalcOutPro

    }

}