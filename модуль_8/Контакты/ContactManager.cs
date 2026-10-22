using Microsoft.Data.Sqlite; 
using System;                 
using System.Collections.Generic; 
using System.IO;            
using System.Linq;            
namespace ContactsWPF 
{
    // Класс ContactManager содержит логику работы с контактами.
    // Он отвечает за добавление, изменение, удаление, поиск и загрузку данных.
    public class ContactManager
    {
        // Создаём список, в котором программа хранит загруженные контакты.
        // List<Contact> означает, что каждый элемент списка — объект класса Contact.
        // readonly запрещает заменить сам список другим объектом после инициализации, но позволяет добавлять в него элементы, удалять их и изменять их свойства.
        private readonly List<Contact> contacts = new List<Contact>();
        // Поле хранит полный путь к файлу базы данных.
        // private означает, что поле доступно только внутри ContactManager.
        // readonly означает, что путь задаётся при создании объекта и затем не меняется.
        private readonly string databasePath;
        // Свойство ConnectionString возвращает строку подключения к SQLite.
        // Строка подключения сообщает программе, с каким файлом базы данных работать.
        // get означает, что значение свойства можно получить, но нельзя напрямую изменить.
        private string ConnectionString
        {
            get
            {
                // Data Source указывает путь к файлу базы данных.
                // Знак + соединяет текст "Data Source=" с путём к файлу.
                return "Data Source=" + databasePath;
            }
        }
        // Конструктор ContactManager выполняется автоматически, когда в программе создаётся объект с помощью new ContactManager().
        // Здесь мы подготавливаем папку, создаём таблицу и загружаем контакты.
        public ContactManager()
        {
            // Получаем путь к локальной папке данных текущего пользователя Windows.
            // Environment.SpecialFolder.LocalApplicationData позволяет не прописывать имя пользователя и путь вручную.
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "ContactsWPF"); // Добавляем имя папки нашего приложения.
            // Создаём папку для хранения файла базы данных.
            // Если папка уже существует, повторный вызов не вызывает ошибку.
            // Это позволяет запускать приложение много раз.
            Directory.CreateDirectory(folder);
            // Объединяем путь к папке с именем файла базы данных.
            // В результате получаем полный путь к contacts.db.
            databasePath = Path.Combine(folder, "contacts.db");
            // Перед чтением или записью проверяем, что таблица существует.
            // Если базы данных ещё нет, SQLite создаст файл при подключении, а команда ниже создаст в нём таблицу Contacts.
            InitializeDatabase();
            // После подготовки таблицы загружаем уже сохранённые контакты.
            // Благодаря этому данные отображаются после повторного запуска программы.
            LoadContacts();
        }
        // Метод InitializeDatabase отвечает за первоначальную подготовку базы данных.
        // private означает, что этот служебный метод вызывается внутри класса.
        // void означает, что метод выполняет действия, но не возвращает значение.
        private void InitializeDatabase()
        {
            // Создаём объект подключения к базе данных.
            // В конструктор передаём строку подключения с путём к файлу.
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                // Открываем подключение к базе данных.
                // До Open() выполнять SQL-запросы через это подключение нельзя.
                connection.Open();
                // Создаём объект команды для выполнения SQL-запроса.
                // using автоматически освобождает ресурсы команды после завершения блока.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // CommandText содержит SQL-команду, которую должна выполнить SQLite.
                    // CREATE TABLE создаёт новую таблицу.
                    // IF NOT EXISTS запрещает создавать таблицу повторно, если таблица Contacts уже существует.
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Contacts
                        (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Phone TEXT NOT NULL,
                            Email TEXT NOT NULL
                        );";
                    // Id INTEGER — целочисленный идентификатор записи.
                    // PRIMARY KEY — первичный ключ, уникально определяющий контакт.
                    // AUTOINCREMENT — SQLite автоматически выдаёт новый ID.
                    // Name TEXT — имя контакта хранится как текст.
                    // Phone TEXT — телефон хранится как текст, поскольку он может содержать знак +, пробелы, скобки и ведущие нули.
                    // Email TEXT — адрес электронной почты хранится как текст.
                    // NOT NULL — значение столбца не может быть SQL-значением NULL.
                    // Выполняем команду создания таблицы.
                    // ExecuteNonQuery используется для команд, которые не возвращают таблицу результатов, например CREATE, INSERT, UPDATE и DELETE.
                    command.ExecuteNonQuery();
                }
                // При выходе из блока using подключение будет освобождено автоматически.
                // Это помогает избежать незакрытых подключений к базе данных.
            }
        }
        // Метод LoadContacts читает контакты из файла базы данных.
        // Он вызывается при запуске приложения после создания таблицы.
        private void LoadContacts()
        {
            // Удаляем элементы из списка перед повторной загрузкой.
            // Это защищает от дублирования контактов, если метод вызовут ещё раз.
            contacts.Clear();
            // Создаём подключение к той же базе данных.
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                // Открываем базу данных для выполнения запроса.
                connection.Open();
                // Создаём команду для чтения данных.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // SELECT указывает, какие столбцы нужно прочитать.
                    // FROM Contacts указывает таблицу, из которой берутся данные.
                    // ORDER BY Id сортирует контакты по идентификатору по возрастанию.
                    command.CommandText = @"
                        SELECT Id, Name, Phone, Email
                        FROM Contacts
                        ORDER BY Id;";
                    // ExecuteReader выполняет SELECT и возвращает объект чтения.
                    // SqliteDataReader позволяет последовательно читать строки результата.
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        // Read() переходит к следующей строке результата.
                        // Пока строки существуют, цикл выполняется.
                        while (reader.Read())
                        {
                            // Создаём объект Contact из данных текущей строки базы.
                            // Индексы столбцов начинаются с нуля:
                            Contact contact = new Contact(
                                reader.GetInt32(0),  // Читаем ID как целое число.
                                reader.GetString(1), // Читаем имя.
                                reader.GetString(2), // Читаем телефон.
                                reader.GetString(3)  // Читаем электронную почту.
                            );
                            // Добавляем созданный объект в список программы.
                            // Теперь с ним можно работать в интерфейсе.
                            contacts.Add(contact);
                        }
                    }
                }
            }
            // После завершения метода список contacts содержит все прочитанные записи.
            // Сам файл базы данных при этом остаётся на диске.
        }
        // Метод возвращает все контакты в виде списка.
        // public означает, что его могут вызывать другие классы, например MainWindow.
        public List<Contact> GetAllContacts()
        {
            // Создаём копию списка, а не возвращаем исходную коллекцию.
            // Это позволяет просматривать контакты, не заменяя внутренний список.
            // Объекты Contact внутри копии при этом остаются теми же объектами.
            return new List<Contact>(contacts);
        }
        // Метод добавляет новый контакт в базу данных.
        // В параметры передаются имя, телефон и адрес электронной почты.
        // ID отдельно не передаётся: его автоматически назначает SQLite.
        public void AddContact(string name, string phone, string email)
        {
            // Проверяем имя до выполнения SQL-запроса.
            // Имя не должно быть null, пустой строкой или состоять только из пробелов.
            if (string.IsNullOrWhiteSpace(name))
            {
                // ArgumentException сообщает вызывающему коду, что переданные данные не соответствуют требованиям метода.
                throw new ArgumentException("Введите имя контакта.");
            }
            // Trim удаляет пробелы в начале и конце имени.
            name = name.Trim();
            // Оператор ?? возвращает пустую строку, если значение равно null.
            // Затем Trim убирает лишние пробелы у телефона и электронной почты.
            phone = (phone ?? "").Trim();
            email = (email ?? "").Trim();
            // Открываем подключение для записи нового контакта.
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                // Открываем файл базы данных.
                connection.Open();
                // Создаём команду INSERT для добавления новой строки.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // INSERT INTO указывает таблицу и столбцы для заполнения.
                    // VALUES задаёт значения через параметры.
                    // Символ @ перед названием параметра обозначает параметр запроса.
                    // Параметры предпочтительнее склеивания SQL-строки вручную: они корректно передают текст и помогают предотвратить SQL-инъекции.
                    command.CommandText = @"
                        INSERT INTO Contacts (Name, Phone, Email)
                        VALUES (@Name, @Phone, @Email);";
                    // Передаём имя в параметр @Name.
                    command.Parameters.AddWithValue("@Name", name);
                    // Передаём телефон в параметр @Phone.
                    command.Parameters.AddWithValue("@Phone", phone);
                    // Передаём электронную почту в параметр @Email.
                    command.Parameters.AddWithValue("@Email", email);
                    // Выполняем INSERT.
                    // После выполнения запись сохраняется в базе данных.
                    command.ExecuteNonQuery();
                    // Создаём отдельную команду, чтобы получить ID добавленной записи.
                    using (SqliteCommand idCommand = connection.CreateCommand())
                    {
                        // last_insert_rowid() возвращает ID последней вставленной записи в текущем подключении SQLite.
                        idCommand.CommandText = "SELECT last_insert_rowid();";
                        // ExecuteScalar возвращает одно значение — в данном случае ID.
                        // Convert.ToInt32 преобразует результат в тип int.
                        int newId = Convert.ToInt32(idCommand.ExecuteScalar());
                        // Создаём объект контакта с ID, который присвоила база.
                        Contact contact = new Contact(
                            newId,
                            name,
                            phone,
                            email);
                        // Добавляем объект в список в оперативной памяти.
                        // Благодаря этому интерфейс может сразу показать новую запись, не закрывая приложение и не выполняя повторную загрузку.
                        contacts.Add(contact);
                    }
                }
            }
            // После выхода из using соединение закрывается автоматически.
            // Запись уже находится в файле базы данных и останется там после закрытия программы, если файл базы данных не будет удалён.
        }
        // Метод изменяет данные контакта, который уже существует.
        // ID используется для определения конкретной строки таблицы.
        public void UpdateContact(
            int id, string name, string phone, string email)
        {
            // Сначала проверяем обязательное поле имени.
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Введите имя контакта.");
            }
            // Ищем контакт в списке по уникальному ID.
            // c — переменная, которая по очереди обозначает каждый контакт.
            // FirstOrDefault возвращает первый совпавший объект или null, если контакт с таким ID отсутствует.
            Contact contact = contacts.FirstOrDefault(c => c.Id == id);
            // Проверяем результат поиска до обращения к свойствам объекта.
            if (contact == null)
            {
                // Не пытаемся изменить запись, которой нет в текущем списке.
                throw new ArgumentException("Контакт не найден.");
            }
            // Убираем пробелы по краям введённых данных.
            name = name.Trim();
            phone = (phone ?? "").Trim();
            email = (email ?? "").Trim();
            // Открываем подключение для изменения записи в базе.
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();
                // Создаём SQL-команду UPDATE.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // UPDATE изменяет существующую запись.
                    // SET задаёт новые значения столбцов.
                    // WHERE Id = @Id ограничивает изменение одной записью.
                    // Без WHERE запрос мог бы изменить данные всех контактов.
                    command.CommandText = @"
                        UPDATE Contacts
                        SET Name = @Name,
                            Phone = @Phone,
                            Email = @Email
                        WHERE Id = @Id;";
                    // Передаём новые значения полей.
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Email", email);
                    // Передаём ID изменяемого контакта.
                    command.Parameters.AddWithValue("@Id", id);
                    // Выполняем обновление записи в базе данных.
                    command.ExecuteNonQuery();
                }
            }
            // После успешного выполнения запроса обновляем объект в списке.
            // Благодаря этому таблица сможет показать новые данные без перезапуска.
            contact.Name = name;
            contact.Phone = phone;
            contact.Email = email;
            // Изменения сохранены в двух местах:
            // 1. В файле SQLite — для последующих запусков программы.
            // 2. В списке contacts — для текущей работы приложения.
        }
        // Метод удаляет контакт с указанным ID.
        public void DeleteContact(int id)
        {
            // Сначала ищем объект в текущем списке.
            Contact contact = contacts.FirstOrDefault(c => c.Id == id);
            // Если объект не найден, прекращаем выполнение метода.
            // return завершает метод, поэтому SQL-запрос не выполняется.
            if (contact == null)
            {
                return;
            }
            // Открываем подключение для удаления записи.
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();
                // Создаём команду удаления.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // DELETE FROM удаляет строки из таблицы.
                    // WHERE оставляет удаление только для указанного ID.
                    command.CommandText =
                        "DELETE FROM Contacts WHERE Id = @Id;";
                    // Передаём ID в параметр SQL-запроса.
                    command.Parameters.AddWithValue("@Id", id);
                    // Выполняем удаление в базе данных.
                    command.ExecuteNonQuery();
                }
            }
            // Удаляем контакт из списка только после выполнения команды SQLite.
            // Это позволяет обновить текущий интерфейс без повторного чтения базы.
            contacts.Remove(contact);
            // Удаление касается и базы данных, и списка в памяти.
            // Поэтому после перезапуска удалённый контакт не загрузится снова.
        }
        // Метод выполняет поиск контактов по имени.
        // Можно указать полное имя или только его часть.
        public List<Contact> SearchContacts(string name)
        {
            // Если пользователь ничего не ввёл, возвращаем весь список.
            // Так после очистки строки поиска снова показываются все контакты.
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetAllContacts();
            }
            // Where отбирает только элементы, удовлетворяющие условию.
            // Для каждого контакта проверяется его свойство Name.
            return contacts
                .Where(c =>
                    // IndexOf ищет в имени переданную строку.
                    // StringComparison.OrdinalIgnoreCase отключает различие между заглавными и строчными буквами.
                    // Значение -1 означает, что искомый текст не найден.
                    // Поэтому условие >= 0 означает, что совпадение существует.
                    c.Name.IndexOf(
                        name.Trim(),
                        StringComparison.OrdinalIgnoreCase) >= 0)
                // ToList создаёт список из найденных совпадений.
                // Исходный список contacts при этом не удаляется и не изменяется.
                .ToList();
        }
    }
}
