using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items.Remove(id);
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            _items[id].Quantity = newQuantity;
        }
    }

    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Freepods pro 12", 10, "Oraimo", 24));
            _electronics.AddItem(new ElectronicItem(2, "Galaxy A30", 8, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Blue pin Adapter 23A", 25, "HP", 18));

            _groceries.AddItem(new GroceryItem(1, "Cowbell Coffee", 20, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(2, "Oba Spaghetti", 15, DateTime.Now.AddDays(3)));
            _groceries.AddItem(new GroceryItem(3, "Gino Curry powder", 35, DateTime.Now.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock updated for {item.Name}. New quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    public class Program
    {
        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("Grocery Items:");
            manager.PrintAllItems(manager.GroceriesRepo);

            Console.WriteLine("\nElectronic Items:");
            manager.PrintAllItems(manager.ElectronicsRepo);

            Console.WriteLine("\n--- Exception Handling Tests ---");

            try
            {
                var duplicateItem = new ElectronicItem(1, "MacBook Pro", 5, "Apple", 12);
                Console.WriteLine($"Attempting to add {duplicateItem.Name} with ID {duplicateItem.Id}...");
                manager.ElectronicsRepo.AddItem(duplicateItem);
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Failed: Attempted to add MacBook Pro with ID 1. {ex.Message}");
            }

            Console.WriteLine("\nAttempting to remove Grocery item with ID 99...");
            manager.RemoveItemById(manager.GroceriesRepo, 99);

            try
            {
                Console.WriteLine("\nAttempting to update quantity of Electronic item ID 2 to -5...");
                manager.ElectronicsRepo.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Failed: Attempted to update item ID 2 with negative quantity. {ex.Message}");
            }
        }
    }
}
