```C#
CalcOutPro(showMessageFlag)
╰── if (_Outp!=null):
    ├── if (showMessageFlag):
    │   ├── if (!notErr):
    │   │   ╰── CommandStatus.FAILED
    │   else
    │   ╰── CommandStatus.FAILED;
    if (notErr):
    ├── if (_plg != null):
    │   ├── if (m_otgr == 0 && g_otgr == 0):
    │   │    ├── if (tip == 1 || tip == 2 || tip == 3 || tip == 5):
    │   │    │   ├── if (!string.IsNullOrEmpty(npril)):
    │   │    │   │   ├── getPrilzAndKudaList
    │   │    │   │   ├── if (CheckOut(prilzList, draft) != "")
    │   │    │   │   │   ├── if (showMessageFlag):
    │   │    │   │   │   │   ├── if (!notErr):
    │   │    │   │   │   │   │   ╰── CommandStatus.FAILED;
    │   │    │   │   │   │   else:
    │   │    │   │   │   │   ╰── CommandStatus.FAILED;
    │   │    │   │   │   if (notErr):
    │   │    │   │   │   ├── getCompList()
    │   │    │   │   │   ╰── modifyComplect()
    │   │    │   ├── if (notErr):
    │   │    │   │   ├── createAndWriteComplTable()
    │   │    │   │   │   ├── createAndWritePrilzTmpTable()
    │   │    │   │   │   ├── formOutPro() --> subStatus
    │   │    │   │   │   if (subStatus == CommandStatus.FAILED)
    │   │    │   │   │   ╰── CommandStatus.FAILED
    │   │    ├── else:   
    │   │    │   ├── if (tip == 4):
    │   │    │   │   ├── if (Model.pTbNumber >= 900):
    │   │    │   │   │   ├── getPartsCompList()
    │   │    │   │   │   ├── createAndWriteComplTable()
    │   │    │   │   │   ├── formOutPro() --> subStatus
    │   │    │   │   │   │   if (subStatus == CommandStatus.FAILED)
    │   │    │   │   │   │   ╰── CommandStatus.FAILED
    │   │    │   │   │   else:
    │   │    │   │   │   ╰── CommandStatus.FAILED    
    │   │    │   ╰── else:
    │   │    │       ╰── if (tip == 6):
    │   │    │           ├── getProdList() --> dr
    │   │    │           ├── if (draft == dr):
    │   │    │           │   ├── formOutPro() --> subStatus
    │   │    │           │   │    if (subStatus == CommandStatus.FAILED)
    │   │    │           │   │    ╰── CommandStatus.FAILED
    │   │    │           │   else:
    │   │    │           │   ╰── CommandStatus.FAILED
    │   │    │           else:
    │   │    │           ╰── CommandStatus.FAILED         
    │   │    else:       
    │   │    ╰── CommandStatus.FAILED   
    │   else:
    │   ╰── CommandStatus.FAILED                    
    else:
    ╰── CommandStatus.FAILED
```

## Где

### showMessageFlag (`bool`):
**Описание**: *Параметр метода*.<br>
При `True` - включает отображение диалоговых окон.<br>
При `False` - отключает отображение диалоговых окон.

---

### subStatus (`CommandStatus`):
**Описание**: Статус выполнения подкоманды.
```C#
subStatus = FormOutPro(draft, 0, null, showMessageFlag)
```

### _Outp (`outpro`):
**Описание**: Запись из таблицы `outpro`.
```C#
var res = MessageBox.Show("Заказ уже есть в OutPro. \nПродолжить расчёт?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question);
notErr = (res.ToString() == "Yes");
```

### notErr (`bool`):
**Описание**: Статус диалогового окна, сообщающего об имеющихся ошибка с выбором пропустить заказ либо продолжить с ошибками.<br>
```C#
var notErr = true;
...
var res = MessageBox.Show("Заказ уже есть в OutPro. \nПродолжить расчёт?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question);
notErr = (res.ToString() == "Yes");
```

### _plg (`pl_god`):
**Описание**: Запись из таблицы `pl_god`.
```C#
var _plg = Model.db.pl_god.FirstOrDefault(p => p.zakaz == Model.pTbOrder && p.nom == Model.pTbNumber);
```

### m_otgr (`decimal`):
**Описание**: Месяц отгрузки заказа.
```C#

```

### g_otgr (`decimal`):
**Описание**: Год отгрузки товара.
```C#

```

### tip (`decimal`):
**Описание**: Тип заказа.
```C#

```

### draft (`decimal`):
**Описание**: Чертеж общего вида из план года.
```C#

```

### prilzList (`List<pril_zM>`):
**Описание**: 
```C#

```

### npril (`string`):
**Описание**: Чертеж приложения.
```C#

```

### dr (`decimal`):
**Описание**:
```C#

```