using System;
class TemperatureSensor // Класс датчика температуры
{
    // Создаём событие TemperatureChanged, оно будет передавать новую температуру
    public event Action<double> TemperatureChanged;
    // Поле для хранения текущей температуры
    private double temperature;
    public void SetTemperature(double newTemperature) // Метод для установки новой температуры
    {
        temperature = newTemperature; // Сохраняем новую температуру
        // Вызываем событие изменения температуры, если на событие кто-то подписан, он получит новую температуру
        TemperatureChanged?.Invoke(temperature);
    }
}
class Thermostat // Класс термостата
{
    public void Subscribe(TemperatureSensor sensor) // Метод для подписки термостата на датчик
    {
        // Подписываем метод CheckTemperature на событие TemperatureChanged
        sensor.TemperatureChanged += CheckTemperature;
    }
    // Этот метод будет автоматически вызываться, когда изменится температура
    private void CheckTemperature(double temperature)
    {
        // Проверяем температуру
        if (temperature < 20)
        {
            // Если температура меньше 20 градусов, включаем отопление
            Console.WriteLine("Температура: " + temperature);
            Console.WriteLine("Отопление включено.");
        }
        else
        {
            // Если температура 20 градусов или выше, выключаем отопление
            Console.WriteLine("Температура: " + temperature);
            Console.WriteLine("Отопление выключено.");
        }
    }
}
class Program
{
    static void Main()
    {
        TemperatureSensor sensor = new TemperatureSensor(); // Создаём датчик температуры
        Thermostat thermostat = new Thermostat();// Создаём термостат
        thermostat.Subscribe(sensor);// Подключаем термостат к датчику
        // Передаём датчику температуру 
        sensor.SetTemperature(34);
        Console.WriteLine();
        // Передаём датчику температуру
        sensor.SetTemperature(-2);
    }
}
