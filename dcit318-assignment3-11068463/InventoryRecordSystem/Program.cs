using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public interface IInventoryEntity
{
    int Id { get; }
}

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("Inventory file not found.");

            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            Console.WriteLine("Data loaded successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Alienware Aurora 16X", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Logitech 15 pro wireless", 20, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Oraimo Freepods 12", 30, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

class Program
{
    static void Main()
    {
        string filePath = "../../../inventory.json";

        InventoryApp app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        Console.WriteLine($"\nNew session. Reading contents from {filePath}");
        app = new InventoryApp(filePath);
        app.LoadData();
        app.PrintAllItems();
    }
}
