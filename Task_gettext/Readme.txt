Выбор файла "*.po" осуществляется при нажатии кнопки "Открыть" главного меню формы.
После открытия файл распарсивается и переводится в формат xml. 

Из исходного файла:

msgid "OK"
msgstr "OK"

msgid "Cancel"
msgstr "Отмена"

msgctxt "Context1.Subcontext1"
msgid "Yes"
msgstr "Да"

msgctxt "Context1.Subcontext1.Subcontext2"
msgid "No"
msgstr "Нет"

msgctxt "Context2.Subcontext3"
msgid "Test"
msgstr "Тест"

будет получен файл следующей структуры:

<root>
  <msg id="OK" str="OK" />
  <msg id="Cancel" str="Отмена" />
  <Context1>
    <Subcontext1>
      <msg id="Yes" str="Да" />
      <Subcontext2>
        <msg id="No" str="Нет" />
      </Subcontext2>
    </Subcontext1>
  </Context1>
  <Context2>
    <Subcontext3>
      <msg id="Test" str="Тест" />
    </Subcontext3>
  </Context2>
</root>

Заполнение TreeView выполняется из xml. Все узлы xml-файла, кроме <msg> превращаются в узлы TreeView при помощи рекурсивного метода 
private void AddTreeViewChildNodes(MsgTreeNode parentNodeTrv, XElement nodeXml). 
А узлы <msg> добавляются к родительскому узлу как связанная информация. 
Чтобы иметь возможность добавлять связанную информацию к узлу, был создан класс-наследник от TreeNode
public class MsgTreeNode : TreeNode
    {
        public List<Msg> msgs = new List<Msg>();
    }
При выборе мышью узла в TreeView связанная информация выводится в TextBox.