using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shell;

namespace FormationZakaz.Data
{

    public interface IModel
    {
        IQueryable<complect> complect { get; }
        void AddObject(complect complect);
    }


    public class ComplectHandler
    {

        private int delCount;
        private int addCount;
        private int changeCount;
        private List<complect> complectsList;
        private List<pril_zM> applicationsList;
        private Action<string, bool> logAction;
        private readonly IModel db; // Используем интерфейс для доступа к базе данных

        ComplectHandler(List<complect> complectsList, List<pril_zM> applicationsList, Action<string, bool> logAction)
        {
            delCount = 0;
            addCount = 0;
            changeCount = 0;
            this.complectsList = new List<complect>(complectsList);
            this.applicationsList = applicationsList;
            this.logAction = logAction;
        }

        public void ResetData()
        {
            delCount = 0;
            addCount = 0;
            changeCount = 0;
            complectsList.Clear();
            applicationsList.Clear();
        }

        public void mModifyComplect(string draft)
        {

            _mLogHeader(draft);

            foreach (var application in applicationsList)
            {
                complect complectByNode = complectsList.Find(z => z.kuda == application.kuda);
                _mValidateApplicationRecord(complectByNode, application);

                _mFindAndInsertComplectToDB(application);

                complect requiredComplect = complectsList
                    .OrderBy(complect => complect.spec)
                    .SingleOrDefault(
                        complect =>
                        complect.what == application.what &&
                        complect.posit == application.poz &&
                        complect.kuda == application.kuda &&
                        complect.spec == application.spec &&
                        complect.ksi == application.ksi
                    );
                //var nodeStr = application.kuda.ToString();
                //int commaIndex = nodeStr.IndexOf(",");
                //var nodeEnd = Convert.ToDecimal(nodeStr.Substring(commaIndex + 1));
                //var what = (decimal)application.what;
                //var node = (decimal)application.kuda;


                _mHandleComplectByOperationCode(requiredComplect, application);


                
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
            logAction($"{DateTime.Now} CompList после модификации: {complectsList.Count}", false);
        }

        private void _mValidateApplicationRecord(complect complect, pril_zM application)
        {
            if (complect != null)
            {
                application.dd = complect.group > 0 ? 2 : 0;
            }
            else
            {
                logAction($"Узла: {application.kuda} нет в compl.", true);
            }
        }

        private void _mHandleComplectByOperationCode(complect complect, pril_zM application)
        {
            if (application.ko == 1)
            {
                _mAddNewComplect(application);
            }
            else if (application.ko == 2)
            {
                _mDeleteComplect(complect);
            }
            else if (application.ko == 3)
            {
                _mModifyComplect(complect, application);
            }
        }

        private void _mAddNewComplect(pril_zM application)
        {
            var nodeStr = application.kuda.ToString();
            var ind = nodeStr.IndexOf(",");
            var nodeEnd = ind >= 0 ? Convert.ToDecimal(nodeStr.Substring(ind + 1)) : 0;

            var requiredComplect = complectsList.Find(complect => complect.kuda == application.kuda);
            if (requiredComplect != null)
            {
                logAction($"ДД\tПозиция:{application.poz}\tЧертёж:{application.what}\tУзел:" +
                          $" {application.kuda}\tРСП: {application.spec}\tКСИ: {application.ksi}\t ko = 1 уже есть в сomplect", true);
                return;
            }

            complectsList.Add(
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

        private void _mDeleteComplect(complect complect)
        {
            if (complect != null)
            {
                complectsList.Remove(complect);
                delCount++;
            }
        }

        private void _mModifyComplect(complect complect, pril_zM application)
        {
            if (complect != null)
            {
                complect.quant = application.kol;
                complect.spec = application.spec;
                complect.path = application.path;
                changeCount++;
            }
        }

        // Метод для проверки и вставки нового комплекта в базу данных
        void _mFindAndInsertComplectToDB(pril_zM application)
        {
            var requiredComplect = db.complect.FirstOrDefault(p => p.what == application.what);
            if (application.ko != 2 && requiredComplect == null)
            {
                _mInsertComplectToDB(application);
            }
        }

        // Метод для вставки нового комплекта в базу данных
        void _mInsertComplectToDB(pril_zM application)
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

        List<complect> _mGetComplectRecordsWithSecondGroup(pril_zM application)
        {
            var filtredComplectsByNodeAndGroup = complectsList
                .Where(
                    complect =>
                    complect.kuda == application.kuda &&
                    complect.group == 2
                )
                .ToList();
            return filtredComplectsByNodeAndGroup;

            //if (filtredComplectsByNodeAndGroup.Any())
            //{
            //    _mHandleComplectRecord(application, complectsList, intNode);
            //}
            //else
            //{
            //    _mLogMissingApplication(application);
            //}
        }

        complect _mGetComplectRecordWithFirstGroup(pril_zM application)
        {
            decimal intNode = decimal.Truncate(application.kuda);

            var complectWithFirstGroup = complectsList
                .OrderBy(complect => complect.spec)
                .SingleOrDefault(
                    complect =>
                    complect.what == application.what &&
                    complect.posit == application.poz &&
                    complect.kuda == intNode &&
                    complect.spec == application.spec &&
                    complect.ksi == application.ksi &&
                    complect.group == 1
                );

            return complectWithFirstGroup;


            //if (complect != null)
            //{
            //    _mHandleComplectByOperationCode(complect, application);
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