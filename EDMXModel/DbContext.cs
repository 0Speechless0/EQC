using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace EQC.EDMXModel
{
    public class DbContext : System.Data.Entity.DbContext
    {
        HashSet<string> myPrimaryCol = new HashSet<string>()
        {
            "Seq"
        };
        public DbContext(bool lazyLoading) : base("name=EQC_NEW_Entities")
        {
            this.Configuration.LazyLoadingEnabled = lazyLoading;
        }

        public static Action<string, string> changeTextSetter;
        public DbContext(string connectionStrring)
        : base(connectionStrring)
        {

        }

        public override int SaveChanges()
        {
            // 紀錄
            RecordEntity();
            var change = base.SaveChanges();
            //RecordEntityAfterInsert();
            if(changeText.Length > 0)
                changeTextSetter?.Invoke(changeText.ToString(), changeDbTable.ToString() );
            return change;
        }

        private Dictionary<DbEntityEntry, EntityState> lastEntryState = new Dictionary<DbEntityEntry, EntityState>();
        private string GetUpdatedColText(DbPropertyValues oldProps, DbPropertyValues newProps)
        {
            StringBuilder entityColUpdated= new StringBuilder();
            newProps.PropertyNames.ToList()
                .ForEach(propName => {

                    object oldValue = oldProps.GetValue<object>(propName) ?? "";
                    object newValue = newProps.GetValue<object>(propName) ?? "";
                    if(myPrimaryCol.Contains(propName) ) entityColUpdated.AppendLine($"{propName}[{newValue}]:");
                    else if (!oldValue.Equals(newValue))
                        entityColUpdated.AppendLine($",{propName}[{oldValue} => {newValue}]");
                });
            return entityColUpdated.ToString();
        }
        private string GetInsertOrDelText(DbEntityEntry dbEntityEntry)
        {
            StringBuilder entityColUpdated = new StringBuilder();
            var dbPropertyValues = dbEntityEntry.State == EntityState.Deleted ?
                dbEntityEntry.OriginalValues :
                dbEntityEntry.CurrentValues;
            dbPropertyValues.PropertyNames.ToList()
                .ForEach(propName => {

                    object value = dbPropertyValues.GetValue<object>(propName);
                    entityColUpdated.Append($",{propName}[{value}]");
                });
            return entityColUpdated.ToString();
        }

        private string GetUsedDbTableName(DbEntityEntry entry)
        {
            if (entry == null) return "";
            var databaseValues = entry.GetValidationResult();
            var internalDatabaseValues = databaseValues.GetType().GetProperties(BindingFlags.Instance |
                            BindingFlags.NonPublic |
                            BindingFlags.Public)[0].GetValue(databaseValues)
                            ;
            string valuesPropertyName = "";

            if(entry.State == EntityState.Deleted)
            {
                valuesPropertyName = "OriginalValues";
            }
            else
            {
                valuesPropertyName = "CurrentValues";
            }
            var values = internalDatabaseValues.GetType().GetProperty(valuesPropertyName).GetValue(internalDatabaseValues);
            var internalValues = values.GetType().GetProperties(BindingFlags.Instance |
                            BindingFlags.NonPublic |
                            BindingFlags.Public)[2].GetValue(values);
            var objectTypeValue = internalValues.GetType().GetProperty("ObjectType").GetValue(internalValues);
            string dbName = objectTypeValue.GetType().GetProperty("Name").GetValue(objectTypeValue).ToString();
            return dbName;
        }

        private void RecordEntityAfterInsert()
        {
            Regex rx = new Regex(@"\[0\]",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchIndex = rx.Matches(changeText.ToString())
                .Cast<Match>()
                .Select(match => {
                    return match.Groups[0].Index;
                })
                .ToArray();

            int i = 0;
            if(matchIndex.Length > 0)
            {
                lastEntryState.Where(r => r.Value == EntityState.Added)
                .ToList()
                .ForEach(entry =>
                {
                    string primaryKeyName =
                    entry.Key.OriginalValues.PropertyNames.Where(r => myPrimaryCol.Contains(r)).FirstOrDefault();
                    if(primaryKeyName != null)
                    {
                        string replacement = $"{primaryKeyName}[{entry.Key.CurrentValues[primaryKeyName]}]";
                        changeText = changeText.Replace($"{primaryKeyName}[0]", replacement, matchIndex[i++] - primaryKeyName.Length, $"{primaryKeyName}[0]".Length);
                    }
                    
                });
            }

  
        }
        StringBuilder changeText = new StringBuilder();
        StringBuilder changeDbTable = new StringBuilder();
        private void RecordEntity()
        {
            var entries = base.ChangeTracker.Entries().Where(r => r.State != EntityState.Unchanged);
            DbPropertyValues originalValues;
            DbPropertyValues currentValues;
            
            var entitiesActionTypeDiv = entries.GroupBy(r => r.State)
            .ToDictionary(r => r.Key, r => r);
            foreach(var entry in entries)
            {

                lastEntryState.Add(entry, entry.State);
                var tableName = GetUsedDbTableName(entry);
                if (!changeDbTable.ToString().Contains(tableName))
                {
                    changeDbTable.Append(tableName);
                    changeDbTable.Append(",");
                }
            }

            //var a = databaseValues.GetValue<object>("InternalPropertyValues");
            if (entitiesActionTypeDiv.ContainsKey(EntityState.Modified))
                entitiesActionTypeDiv[EntityState.Modified].Take(3) 
                .ToList()
                .ForEach(entity => {
                 
                    changeText.AppendLine($"\t=== 更新 {GetUsedDbTableName(entity)}資料 ======");
                    changeText.AppendLine(GetUpdatedColText(entity.OriginalValues, entity.CurrentValues));
                });
            if (entitiesActionTypeDiv.ContainsKey(EntityState.Added))
                entitiesActionTypeDiv[EntityState.Added].Take(3)
                .ToList()
                .ForEach(entity => {
                    changeText.AppendLine($"\t=== 新增 {GetUsedDbTableName(entity)}資料 ======");
                    changeText.AppendLine(GetInsertOrDelText(entity));
                });
            if(entitiesActionTypeDiv.ContainsKey(EntityState.Deleted)) 
                
                entitiesActionTypeDiv[EntityState.Deleted].Take(3)
                .ToList()
                .ForEach(entity => {
                    changeText.AppendLine($"\t=== 刪除 {GetUsedDbTableName(entity)}資料 ======");
                    changeText.AppendLine(GetInsertOrDelText(entity));
                });


        }
    }
}