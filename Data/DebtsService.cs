using System.Text.Json;

namespace Todo.Data;

public static class DebtsService
{
    private static readonly string DebtsFilePath = "debts.json";
    private static readonly string ClearedDebtsFilePath = "deleted_debts.json";


    private static void SaveAll(Guid userId, List<Debt> debts)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string userDebtsFilePath = GetUserDebtsFilePath(userId);

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(debts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(userDebtsFilePath, json);
    }

    private static string GetUserDebtsFilePath(Guid userId)
    {
        return Path.Combine(Utils.GetAppDirectoryPath(), $"{userId}_debts.json");
    }

    public static List<Debt> GetAll(Guid userId)
    {
        string userDebtsFilePath = GetUserDebtsFilePath(userId);
        if (!File.Exists(userDebtsFilePath))
        {
            return new List<Debt>();
        }

        var json = File.ReadAllText(userDebtsFilePath);
        return JsonSerializer.Deserialize<List<Debt>>(json) ?? new List<Debt>();
    }

    public static List<Debt> Delete(Guid userId, Guid id)
    {
        List<Debt> debts = GetAll(userId);
        Debt debt = debts.FirstOrDefault(x => x.Id == id);

        if (debts == null)
        {
            throw new Exception("Todo not found.");
        }

        debts.Remove(debt);
        SaveAll(userId, debts);
        return debts;
    }

    public static void DeleteByUserId(Guid userId)
    {
        string userDebtsFilePath = Utils.GetUsersFilePath(userId);
        if (File.Exists(userDebtsFilePath))
        {
            File.Delete(userDebtsFilePath);
        }
    }

    public static List<Debt> Create(Guid userId, string taskName, string SourceofDebt, string debtAmount, DateTime dueDate)
    {
        if (dueDate < DateTime.Today)
        {
            throw new Exception("Due date must be in the future.");
        }

        List<Debt> debts = GetAll(userId);
        debts.Add(new Debt
        {
            Id = Guid.NewGuid(),
            TaskName = taskName,
            SourceofDebt = SourceofDebt,
            DebtAmount = debtAmount,
            DueDate = dueDate,
            CreatedBy = userId,
            CreatedAt = DateTime.Now,
            IsDone = false
        });

        SaveAll(userId, debts);
        return debts;
    }

    public static List<Debt> Clear(Guid userId, Guid id)
    {
        List<Debt> debts = GetAll(userId);
        Debt debt = debts.FirstOrDefault(x => x.Id == id);

        if (debt == null)
        {
            throw new Exception("Debt not found.");
        }

        // Remove from the current debts list
        debts.Remove(debt);
        SaveAll(userId, debts);

        //debts.Add(debt);
        

        // Save to deleted debts
        debt.IsDone=true;
        List<Debt> clearedDebts = GetClearedDebts();
        clearedDebts.Add(debt);
        SaveDeletedDebts(clearedDebts);

        return debts;
    }

    public static void ClearByUserId(Guid userId)
    {
        string userDebtsFilePath = GetUserDebtsFilePath(userId);
        if (File.Exists(userDebtsFilePath))
        {
            File.Delete(userDebtsFilePath);
        }
    }

    public static List<Debt> Update(Guid userId, Guid id, string taskName, string SourceofDebt, string debtAmount, DateTime dueDate, bool isDone)
    {
        List<Debt> debts = GetAll(userId);
        Debt debtToUpdate = debts.FirstOrDefault(x => x.Id == id);

        if (debtToUpdate == null)
        {
            throw new Exception("Debt not found.");
        }

        debtToUpdate.TaskName = taskName;
        debtToUpdate.SourceofDebt = SourceofDebt;
        debtToUpdate.DebtAmount = debtAmount;
        debtToUpdate.DueDate = dueDate;
        debtToUpdate.IsDone = isDone;

        SaveAll(userId, debts);
        return debts;
    }
    private static void SaveDeletedDebts(List<Debt> clearedDebts)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string fullPath = Path.Combine(appDataDirectoryPath, ClearedDebtsFilePath);

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(clearedDebts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fullPath, json);
    }

    // Get deleted debts from JSON
    public static List<Debt> GetClearedDebts()
    {
        string fullPath = Path.Combine(Utils.GetAppDirectoryPath(), ClearedDebtsFilePath);

        if (!File.Exists(fullPath))
        {
            return new List<Debt>();
        }

        var json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<List<Debt>>(json) ?? new List<Debt>();
        }



          public class DebtStateService
    {
        private List<Debt> _clearedDebts = new List<Debt>();

        public List<Debt> GetDeletedDebts() => _clearedDebts;

        public void ClearDeletedDebts() => _clearedDebts.Clear();
    }

}

