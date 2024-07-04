using System.Reflection;
using System.Text;

namespace HomeworkGB6
{
    internal class Program
    {
        static void Main()
        {
            var test = MakeTestClass(10, "строка", 1.578M, ['a', 'b', 'c']);
            string? str = ObjectToString(test!);
            Console.WriteLine("Объект test преобразован в строку: \n" + str + "\n");

            var testEmpty = MakeTestClass();
            Console.WriteLine("Пустой объект testEmpty: \n" + ObjectToString(testEmpty!) + "\n");

            testEmpty = StringToObject(str) as TestClass;
            Console.WriteLine("Пустому объекту testEmpty присвоен объект test: \n" + ObjectToString(testEmpty!) + "\n");

            Console.ReadKey(true);
        }
        static TestClass? MakeTestClass()
        {
            return Activator.CreateInstance(typeof(TestClass)) as TestClass;
        }
        static TestClass? MakeTestClass(int i)
        {
            return Activator.CreateInstance(typeof(TestClass), i) as TestClass;
        }
        static TestClass? MakeTestClass(int i, string s, decimal d, char[] c)
        {
            //параметры для конструктора переданы через запятую, тк это ключевая особенность params
            //можно не собирать их в массив типа object
            return Activator.CreateInstance(typeof(TestClass), i, s, d, c) as TestClass;
        }
        static string ObjectToString(object obj)
        {
            //создаем стрингбилдер для вывода
            StringBuilder result = new();

            //сохраняем в переменную тип переданного объекта
            Type type = obj.GetType();

            //имя сборки и название типа
            result.Append($"{type.AssemblyQualifiedName}:{type.Name}|");

            //перебираем свойства, получив их массив через метод GetProperties
            foreach (PropertyInfo? item in type.GetProperties())
            {
                //извлекаем значение свойства в temp 
                object? temp = item.GetValue(obj);

                //ищем у этого свойства атрибут
                CustomNameAttribute? attribute = Attribute.GetCustomAttribute(item, typeof(CustomNameAttribute)) as CustomNameAttribute;

                //если атрибут найден, то вместо имени свойства пишем имя атрибута
                //значение записывается с использованием тернарного оператора: это позволяет записать char[] слитно, как string
                result.Append($"{attribute?.PropertyName ?? item.Name}:{(temp is char[]? new string(temp as char[]) : temp)}|"); 
            }

            //результат в виде строки
            return result.ToString();
        }
        static object? StringToObject(string str)
        {
            //разделяем строку на части (класс, параметры)
            //убираем символ разделения с конца, чтобы массив не сохранялся с одним пустым элементом
            string[] pairs = str.TrimEnd('|').Split('|'); 

            //вытаскиваем из первого элемента название пространства имен и полное имя класса
            string[] names = pairs[0].Split(',');

            //создаем объект по названию пространства имен и имени класса
            object? result = Activator.CreateInstance(names[1], names[0])?.Unwrap(); //Unwrap распакует из ObjectHandle

            //прерывание метода, если result пуст или не содержит полей
            if (result == null || pairs.Length < 2) return result;

            //сохраняем в переменную тип объекта result
            Type type = result.GetType();

            //перебираем с единицы, тк первый элемент это имя сборки
            for (int i = 1; i < pairs.Length; i++)
            {
                //сохраняем пару "имя свойства" - "значение" в массив pair
                string[] pair = pairs[i].Split(':');

                //ищем свойство с данным именем
                PropertyInfo? prop = type.GetProperty(pair[0]);

                //если имя было сохранено не свойства, а кастомное, то prop равен нулю и мы ищем по атрибуту:
                if (prop == null) 
                {
                    foreach (PropertyInfo item in type.GetProperties())
                    {
                        var attribute = Attribute.GetCustomAttribute(item, typeof(CustomNameAttribute)) as CustomNameAttribute;
                        if (attribute?.PropertyName == pair[0])
                            prop = item;
                    }
                }

                //проверки типа свойства, чтобы правильно запарсить значение и присвоить переменной класса
                if (prop?.PropertyType == typeof(int)) { prop.SetValue(result, int.Parse(pair[1])); }
                else if (prop?.PropertyType == typeof(string)) { prop.SetValue(result, pair[1]); }
                else if (prop?.PropertyType == typeof(decimal)) { prop.SetValue(result, decimal.Parse(pair[1])); }
                else if (prop?.PropertyType == typeof(char[])) { prop.SetValue(result, pair[1].ToCharArray()); }
                else
                    Console.WriteLine($"Свойство не было найдено (цикл: {i})"); //на всякий случай :)
            }

            //результат в виде объекта (в типе object)
            return result;
        }
    }
}
