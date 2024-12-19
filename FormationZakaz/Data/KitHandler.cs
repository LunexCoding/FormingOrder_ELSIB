using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormationZakaz.Data
{

    public interface IModel
    {
        IQueryable<complect> complect { get; }
        void AddObject(complect complect);
    }


    public class KitHandler
    {

        private int delCount;
        private int addCount;
        private int changeCount;
        private List<complect> kitsList;
        private List<pril_zM> applicationsList;
        private Action<string, bool> logAction;
        private readonly IModel db; // Используем интерфейс для доступа к базе данных

        KitHandler(List<complect> kitsList, List<pril_zM> applicationsList, Action<string, bool> logAction)
        {
            delCount = 0;
            addCount = 0;
            changeCount = 0;
            this.kitsList = new List<complect>(kitsList);
            this.applicationsList = applicationsList;
            this.logAction = logAction;
        }

        public void ResetData()
        {
            delCount = 0;
            addCount = 0;
            changeCount = 0;
            kitsList.Clear();
            applicationsList.Clear();
        }

        public void mModifyComplect(string draft)
        {

            _mLogHeader(draft);

            foreach (var application in applicationsList)
            {
                var kit = kitsList.Find(z => z.kuda == application.kuda);
                _mHandleKitByOperationCode(kit, application);
            }

            _mLogFooter();
        }

        private void _mLogHeader(string draft)
        {
            logAction("____", false);
            logAction($"{DateTime.Now}\t Заказ/Номер: {draft.Replace(',', '.')}", false);
        }

        private void _mLogFooter()
        {
            logAction("____", false);
            logAction($"ko = 3 изменено: {changeCount}\tko = 2 удалено: {delCount}\tko = 1 добавлено: {addCount}", false);
            logAction($"{DateTime.Now} CompList после модификации: {kitsList.Count}", false);
        }

        private void _mValidateApplicationRecord(pril_zM application)
        {
            var requiredKit = kitsList.Find(kit => kit.kuda == application.kuda);
            if (requiredKit != null)
            {
                application.dd = requiredKit.group > 0 ? 2 : 0;
            }
            else
            {
                logAction($"Узла: {application.kuda} нет в compl.", true);
            }
        }

        private void _mHandleKitByOperationCode(complect kit, pril_zM application)
        {
            if (application.ko == 1)
            {
                _mAddNewKit(application);
            }
            else if (application.ko == 2)
            {
                _mDeleteKit(kit);
            }
            else if (application.ko == 3)
            {
                _mModifyKit(kit, application);
            }
        }

        private void _mAddNewKit(pril_zM application)
        {
            var nodeStr = application.kuda.ToString();
            var ind = nodeStr.IndexOf(",");
            var nodeEnd = ind >= 0 ? Convert.ToDecimal(nodeStr.Substring(ind + 1)) : 0;

            var requiredKit = kitsList.Find(kit => kit.kuda == application.kuda);
            if (requiredKit != null)
            {
                logAction($"ДД\tПозиция:{application.poz}\tЧертёж:{application.what}\tУзел:" +
                          $" {application.kuda}\tРСП: {application.spec}\tКСИ: {application.ksi}\t ko = 1 уже есть в сomplect", true);
                return;
            }

            kitsList.Add(
                new complect
                {
                    posit = application.poz,
                    kuda = application.kuda,
                    what = application.what,
                    quant = application.kol,
                    spec = application.spec,
                    path = application.path,
                    ksi = application.ksi,
                    group = application.dd == 0 ? 0 : (nodeEnd > 0 ? 2 : 1)
                }
            );
            addCount++;
        }

        private void _mDeleteKit(complect kit)
        {
            if (kit != null)
            {
                kitsList.Remove(kit);
                delCount++;
            }
        }

        private void _mModifyKit(complect kit, pril_zM application)
        {
            if (kit != null)
            {
                kit.quant = application.kol;
                kit.spec = application.spec;
                kit.path = application.path;
                changeCount++;
            }
        }

        // Метод для проверки и вставки нового комплекта в базу данных
        void _mCheckAndInsertKitToDB(pril_zM application)
        {
            var requiredKit = db.complect.FirstOrDefault(p => p.what == application.what);
            if (application.ko != 2 && requiredKit == null)
            {
                _mInsertKitToDB(application);
            }
        }

        // Метод для вставки нового комплекта в базу данных
        void _mInsertKitToDB(pril_zM application)
        {
            db.AddObject(new complect
            {
                format = string.Empty,
                posit = application.poz,
                what = application.what,
                kuda = application.kuda,
                quant = application.kol,
                ed = 0,
                group = 0,
                spec = application.spec,
                ksi = application.ksi,
                path = application.path,
                izv = application.zak.ToString(),
                dti = application.nom.ToString(),
                tfl = string.Empty
            });

            if (application.spec == 2)
            {
                // Если узел, то вставляем заголовок
                db.AddObject(new complect
                {
                    format = string.Empty,
                    posit = application.poz,
                    what = application.what,
                    kuda = application.what,
                    quant = 1,
                    ed = 0,
                    group = 0,
                    spec = 1,
                    ksi = application.ksi,
                    path = application.path,
                    izv = application.zak.ToString(),
                    dti = application.nom.ToString(),
                    tfl = string.Empty
                });
            }
        }

        List<complect> _mGetKitRecordsWithSecondGroup(pril_zM application)
        {
            var filtredKitsByNodeAndGroup = kitsList
                .Where(
                    kit =>
                    kit.kuda == application.kuda &&
                    kit.group == 2
                )
                .ToList();
            return filtredKitsByNodeAndGroup;

            //if (filtredKitsByNodeAndGroup.Any())
            //{
            //    _mHandleKitRecord(application, kitsList, intNode);
            //}
            //else
            //{
            //    _mLogMissingApplication(application);
            //}
        }

        complect _mGetKitRecordWithFirstGroup(pril_zM application)
        {
            decimal intNode = decimal.Truncate(application.kuda);

            var kitWithFirstGroup = kitsList
                .OrderBy(kit => kit.spec)
                .SingleOrDefault(
                    kit =>
                    kit.what == application.what &&
                    kit.posit == application.poz &&
                    kit.kuda == intNode &&
                    kit.spec == application.spec &&
                    kit.ksi == application.ksi &&
                    kit.group == 1
                );

            return kitWithFirstGroup;


            //if (kit != null)
            //{
            //    _mHandleKitByOperationCode(kit, application);
            //}
            //else
            //{
            //    _mLogMissingApplication(application);
            //}
        }
        
        void _mLogMissingApplication(pril_zM application)
        {
            logAction($"ДД\tПозиция:{application.poz}\tЧертёж: {application.what}\tУзел: {application.kuda}\tРСП:" +
                $" {application.spec}\tКСИ: {application.ksi}\t ko = {application.ko} нет в сomplect", true);
        }

    }

}