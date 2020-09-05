# Задачи

- [ ] Добавить поддержку отображения **Unity Cube** в перспективной/ортографической проекции 
     с возможностью выбора режима отображения в меню настроек 
     (отдельная вкладка для Unity Cube: **Show Unity Cube**; **Projection: Perspective/Orthographic**
     (отображается, если Show Unity Cube = true (или нет, мб не надо такой возни, подумай еще))).
- [ ] Добавить возможность настраивать порядок рендера итогового изображения от двух камер 
     (**#1 - Perspective - Main Player Camera**; **#2 - Orthographic; Depth of #1 > Depth of #2**) 
     с помощью 4-х настроек (**Clearing Flags, Active** от обоих камер). 
     
     Возможные варианты:
     
     1. **#1 - Active = true, Clearing Flags = Solid Color | #2 - Active = true / false, Clearing Flags = Solid Color / Depth Only**
            - изображение от **мастер** камеры полностью перекрывает изображение от **слейв**
                (потому что у Depth of #1 > Depth of #2 и #1 Clearing Flags = Solid Color)
     
     2. **#1 - Active = false, Clearing Flags = Solid Color / Depth Only | #2 - Active = true, Clearing Flags = Solid Color**
            - итоговое изображение формируется исключительно **слейв** камерой без смазывания
     
     3. **#1 - Active = true, Clearing Flags = Depth Only | #2 - Active = true, Clearing Flags = Solid Color**
            - изображение от **мастер** камеры накладывается поверх изображения от **слейв** без смазывания
     
     4. **#1 - Active = true, Clearing Flags = Depth Only | #2 - Active = false, Clearing Flags = Solid Color / Depth Only**
            - итоговое изображение формируется исключительно **мастер** камерой со смазыванием
               
     5. **#1 - Active = false, Clearing Flags = Solid Color / Depth Only | #2 - Active = true, Clearing Flags = Depth Only**
            - итоговое изображение формируется исключительно **слейв** камерой со смазыванием
     
     6. **#1 - Active = true, Clearing Flags = Depth Only | #2 - Active = true, Clearing Flags = Depth Only**
            - изображение от **мастер** камеры накладывается поверх изображения от **слейв** и оба смазываются
          
    Примечания:
    - Мастер камера - это камера, которой управляет игрок. Слейв - вторичная камера;
    - Как минимум одна камера должна быть Active для рендера итогового изображения;
    - Необходимо вводить дополнительные блокирующие условия (например, если выключена одна камера, то недоступна возможность выключить другую; когда #1 Clearing Flags = Solid Color изменение #2 Active, Clearing Flags не дает видимых результатов для пользователя, поэтому эти настройки должны быть недоступны);
    - В панели настроек настройки камеры должны быть упакованы в сворачивающуюся панель и должны гибко встраиваться в текущий интерфейс (как в инспекторе Unity);
    - Настройки должны дезейблиться, но не сбрасываться;
    - Clearing Flags и Active заменить наверное стоит на более подходящие термины для пользователя;
    - Мб с битовыми масками повозиться стоит;
    - Добавить Clipping Planes в настройки.