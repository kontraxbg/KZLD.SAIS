using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SAIS.Data;
using System.Linq;
using SAIS.Model.Audit;
using SAIS.Serice.Audit;

namespace SAIS.Service.Audit
{
    public static class ChangeLogger
    {
        // Ако е нужно един ChangeLogger да се ползва многократно, методите за инициализация и описание може да се подадат еднократно.

        //private readonly Func<Log> _logInitializer;
        //private readonly Func<PropertyDescriptorParams, Task<string>> _propertyDescriptorAsync;

        //public ChangeLogger(Func<Log> logInitializer, Func<PropertyDescriptorParams, Task<string>> propertyDescriptorAsync)
        //{
        //    _logInitializer = logInitializer;
        //    _propertyDescriptorAsync = propertyDescriptorAsync;
        //}

        //public Task<int> SaveChangesAsync(DbContext db, Func<Task<int>> saveChagesAsyncMethod)
        //{
        //    Log log = (_logInitializer != null ? _logInitializer() : null) ?? new Log();
        //    return SaveChangesAsync(db, saveChagesAsyncMethod, log, _propertyDescriptorAsync);
        //}

        public async static Task<int> SaveChangesAsync(
            DbContext db,
            AuditContext auditDb,
            Func<Task<int>> saveChagesAsyncMethod,
            AuditModel audit,
            Type auditMasterEntityType,
            string auditMasterEntityId)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (saveChagesAsyncMethod == null)
            {
                throw new ArgumentNullException(nameof(saveChagesAsyncMethod));
            }
            if (audit == null)
            {
                throw new ArgumentNullException(nameof(audit));
            }
            if (auditMasterEntityType == null)
            {
                throw new ArgumentNullException(nameof(auditMasterEntityType));
            }

            // Ако е подаден генериран клас, наследник на entity класа (т.нар. dynamic proxy), се взима entity класът.
            if (auditMasterEntityType.BaseType != null && auditMasterEntityType.Namespace == "System.Data.Entity.DynamicProxies")
            {
                auditMasterEntityType = auditMasterEntityType.BaseType;
            }

            audit.EntityName = auditMasterEntityType.Name;
            audit.EntityRecordId = auditMasterEntityId;
            audit.AuditTypeCode = AuditTypeCode.Write;
            audit.Sanitize();
            if (audit.AuditDetails == null)
            {
                audit.AuditDetails = new List<AuditDetailModel>();
            }

            int result;
            IEnumerable<EntityEntry> addedEntries = GetAddedEntries(db);

            try
            {
                var logConfigDictionary = await auditDb.AuditConfigs.ToDictionaryAsync(l => l.EntityName + "." + l.PropertyName);
                await GetLogDetailsForUpdatedAndDeletedAsync(db, audit, logConfigDictionary);

                result = await saveChagesAsyncMethod();

                await GetLogDetailsForAddedAsync(db, audit, logConfigDictionary, addedEntries);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            audit.GetMissingEntityRecordIdFromDetails();

            // да не се записват записи за промяна без реални промени
            if (audit.AuditDetails.Count > 0)
            {
                AuditUtil.Add(auditDb, audit);  // Извиква SaveChanges().
            }

            await UpdateUserHostFromIpAsync(audit);

            return result;
        }

        private static void Sanitize(this AuditModel audit)
        {
            audit.IpAddress = Sanitize(audit.IpAddress, 100);
            audit.UrlAccessed = Sanitize(audit.UrlAccessed, 1000);
            audit.UserName = Sanitize(audit.UserName, 256);
            audit.Controller = Sanitize(audit.Controller, 1000);
            audit.Action = Sanitize(audit.Action, 1000);
            audit.SessionId = Sanitize(audit.SessionId, 1000);
            audit.RequestMethod = Sanitize(audit.RequestMethod, 100);
            //audit.AuditTypeString = Sanitize(audit.AuditTypeString, 100);
            audit.EntityName = Sanitize(audit.EntityName, 100);
            audit.EntityRecordId = Sanitize(audit.EntityRecordId, 100);
        }

        private static string Sanitize(string text, int maxLength)
        {
            return string.IsNullOrEmpty(text) ? null : text.Truncate(maxLength);
        }

        private static async Task<string> GetPropertyDescriptionAsync(PropertyDescriptorParams e)
        {
            object propertyValue = e.PropertyValue;
            if (propertyValue == null)
            {
                return null;
            }

            string result = null;
            string masterEntityName = e.GuessMasterEntityName;
            if (!string.IsNullOrEmpty(masterEntityName))
            {
                //// Първо се търси номенклатурна стойност.
                //ObjectTypeCode objectTypeCode;
                //if (Enum.TryParse(entityName, out objectTypeCode))
                //{
                //    using (LangRepo lang = new LangRepo())
                //    {
                //        result = await lang.GetTextAsync(objectTypeCode, propertyValue.ToString());
                //    }
                //}
                //
                //// Обектът се търси в таблицата, към която сочи navigation property-то.
                //// Пример: propertyName = "PermitId" предизвиква търсене db.Permits.Find().
                //if (result == null)
                //{

                // Не се поддържа търсене на master entity по 2+ колони (не се поддържат съставни foreign keys).
                // Tрябва да се извика FindAsync(key1, key2, ...), но в този момент работим с 1 конкретно property
                // и няма лесен начин да се открият останалите foreign key property-та и техните стойности.
                string[] masterKeyNames = GetKeyNames(e.DbContext, masterEntityName);
                if (masterKeyNames != null && masterKeyNames.Length == 1)
                {
                    result = DescribeForLogging(await e.DbContext.FindAsync(masterEntityName, propertyValue));
                }

                //}
            }

            // Ако стойността не сочи към друго entity, се прави опит самата стойност да бъде форматирана.
            // Пример: propertyName = "IsPrimary" има описание "Да" или "Не". 
            if (result == null)
            {
                result = DescribeForLogging(propertyValue);
            }
            return result;
        }

        public static async Task<string> GetEntityDescriptionAsync(DbContext db, string entityName, string entityId)
        {
            return DescribeForLogging(await db.FindAsync(entityName, GetEntityIdFromString(db, entityName, entityId)));
        }

        private static string DescribeForLogging(object obj)
        {
            if (obj is bool value)
            {
                return value ? "Да" : "Не";
            }
            return null;
        }

        private static IEnumerable<EntityEntry> GetAddedEntries(DbContext db)
        {
            return db.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToArray();
        }

        private static async Task GetLogDetailsForUpdatedAndDeletedAsync(
            DbContext db,
            AuditModel audit,
            Dictionary<string, AuditConfig> logConfigDictionary)
        {
            foreach (var ent in db.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified || p.State == EntityState.Deleted))
            {
                // For each changed record, get the audit record entries and add them
                List<AuditDetailModel> auditDetails = await GetAuditRecordsForChangeAsync(db, logConfigDictionary, ent, ent.State);
                foreach (AuditDetailModel logDetail in auditDetails)
                {
                    audit.AuditDetails.Add(logDetail);
                }
            }
        }

        private static async Task GetLogDetailsForAddedAsync(
            DbContext db,
            AuditModel audit,
            Dictionary<string, AuditConfig> auditConfigDictionary,
            IEnumerable<EntityEntry> addedEntries)
        {
            foreach (var ent in addedEntries)
            {
                List<AuditDetailModel> logDetails = await GetAuditRecordsForChangeAsync(db, auditConfigDictionary, ent, EntityState.Added);
                foreach (AuditDetailModel logDetail in logDetails)
                {
                    audit.AuditDetails.Add(logDetail);
                }
            }
        }

        private static async Task<List<AuditDetailModel>> GetAuditRecordsForChangeAsync(
            DbContext db,
            Dictionary<string, AuditConfig> logConfigDictionary,
            EntityEntry dbEntry,
            EntityState entityState)
        {
            List<AuditDetailModel> result = new List<AuditDetailModel>();

            PropertyValues dbProperties = (entityState != EntityState.Added) ? dbEntry.OriginalValues : dbEntry.CurrentValues;
            string entityName = GetEntityName(dbEntry);
            string[] keyNames = GetKeyNames(db, entityName);
            string keyValues = String.Join("; ", keyNames.Select(k => dbProperties[k]).ToArray());

            AuditDetailTypeCode logDetailType;
            if (entityState == EntityState.Added)
                logDetailType = AuditDetailTypeCode.C;
            else if (entityState == EntityState.Modified)
                logDetailType = AuditDetailTypeCode.U;
            else if (entityState == EntityState.Deleted)
                logDetailType = AuditDetailTypeCode.D;
            else
                throw new NotImplementedException("GetAuditRecordsForChangeAsync: Invalid entity state - " + entityState);

            foreach (string propertyName in dbProperties.Properties.Select(p => p.Name).ToArray())
            {
                object originalValueObj = (entityState != EntityState.Added) ? dbEntry.OriginalValues[propertyName] : null;
                object currentValueObj = (entityState != EntityState.Deleted) ? dbEntry.CurrentValues[propertyName] : null;

                string originalValue = originalValueObj?.ToString();
                string currentValue = currentValueObj?.ToString();

                if (!keyNames.Contains(propertyName) && originalValue != currentValue)
                {
                    logConfigDictionary.TryGetValue(entityName + "." + propertyName, out AuditConfig auditConfig);
                    string navigationPropertyEntityName = auditConfig?.Mapping;

                    string originalValueDescription = null, currentValueDescription = null;
                    originalValueDescription = await GetPropertyDescriptionAsync(new PropertyDescriptorParams(
                        db, /*entityName, */propertyName, navigationPropertyEntityName, originalValueObj));
                    currentValueDescription = await GetPropertyDescriptionAsync(new PropertyDescriptorParams(
                        db, /*entityName, */propertyName, navigationPropertyEntityName, currentValueObj));

                    result.Add(
                        new AuditDetailModel()
                        {
                            AuditDetailTypeCode = logDetailType,
                            EntityName = entityName,
                            RecordId = keyValues.ToString(),
                            PropertyName = propertyName,
                            OriginalValue = originalValue,
                            NewValue = currentValue,
                            OriginalValueDescription = originalValueDescription,
                            NewValueDescription = currentValueDescription,
                        }
                    );
                }
            }

            return result;
        }

        private static string GetEntityName(EntityEntry entry)
        {
            Type entityType = entry.Entity.GetType();
            if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
                entityType = entityType.BaseType;
            return entityType.Name;
        }

        private static string[] GetKeyNames(DbContext db, string entityTypeName)
        {
            string entityFullName = string.Format("{0}.{1}", db.GetType().Namespace, entityTypeName);
            return db.Model.FindEntityType(entityFullName).FindPrimaryKey().Properties.Select(p => p.PropertyInfo.Name).ToArray();

            //ObjectContext objectContext = ((IObjectContextAdapter)|db).ObjectContext;
            //EntityContainer container = objectContext.MetadataWorkspace.GetEntityContainer(
            //    objectContext.DefaultContainerName, DataSpace.CSpace);
            //string entitySetName = (
            //    from meta in container.BaseEntitySets
            //    where meta.ElementType.Name == entityTypeName
            //    select meta.Name).FirstOrDefault();
            //return entitySetName != null ? container.BaseEntitySets[entitySetName].ElementType.KeyMembers.Select(k => k.Name).ToArray() : null;
        }

        private static object GetEntityIdFromString(DbContext db, string entityTypeName, string entityId)
        {
            string entityFullName = string.Format("{0}.{1}", db.GetType().Namespace, entityTypeName);
            var primaryKeyProperties = db.Model.FindEntityType(entityFullName).FindPrimaryKey().Properties;

            //ObjectContext objectContext = ((IObjectContextAdapter)db).ObjectContext;
            //EntityContainer container = objectContext.MetadataWorkspace.GetEntityContainer(
            //    objectContext.DefaultContainerName, DataSpace.CSpace);
            //string entitySetName = (
            //    from meta in container.BaseEntitySets
            //    where meta.ElementType.Name == entityTypeName
            //    select meta.Name).FirstOrDefault();

            //ReadOnlyMetadataCollection<EdmMember> keymembers = container.BaseEntitySets[entitySetName].ElementType.KeyMembers;
            //if (keymembers.Count == 1)
            //{
            //    string type = ((EdmProperty)keymembers[0]).TypeName;

            //    switch (type)
            //    {
            //        case "Int32": return Int32.Parse(entityId);
            //        default: return entityId;
            //    }
            //}

            return null;
        }


        private static Task UpdateUserHostFromIpAsync(AuditModel audit)
        {
            return Task.CompletedTask;
        }

        //        ///// TODO: трябва ли ни името?

        //        // Внимание: пускане на background задачи в ASP.NET се води ненадеждно:
        //        // http://stackoverflow.com/questions/29577652/calling-an-async-method-without-awaiting
        //        private static Task UpdateUserHostFromIpAsync(Audit audit)
        //        {
        //            if (audit.UserHostName == null)
        //            {
        //                // Вариант 1:
        //                // Незнайно защо води до грешка "The transaction manager has disabled its support for remote/network transactions.
        //                // (Exception from HRESULT: 0x8004D024); Network access for Distributed Transaction Manager (MSDTC) has been
        //                // disabled. Please enable DTC for network access in the security configuration for MSDTC using the Component
        //                // Services Administrative tool.; The underlying provider failed on Open."
        //                //return Task.Run(() => ResolveUserHost(args.Log.Id));

        //                // Вариант 2:
        //                // Ако се ползва async вариантът, контекстът се създава в същата транзакция, която записва данните и това
        //                // води до грешка "The transaction operation cannot be performed because there are pending requests working on this transaction.".
        //                // или "The operation is not valid for the state of the transaction.; The underlying provider failed on Open."
        //                // Вариант 2а:
        //                // Целият метод за логване се подава на HostingEnvironment.QueueBackgroundWorkItem, а самото resolve-ане
        //                // се изчаква синхронно в рамките на така създадения WorkItem.
        //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //                return ResolveUserHostAsync(audit.Id);
        //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        //                // Вариант 3:
        //                // Инжектира се функция, която извиква ResolveUserHostAsync през HostingEnvironment.QueueBackgroundWorkItem.
        //                // Понеже обаче методът за логване не се изчаква от контролера, обикновено изобщо не се достига до
        //                // извикване на този метод (UpdateUserHostFromIp), може би защото request-ът приключва преди това.
        //                //if (args.UserHostResolver != null)
        //                //{
        //                //    args.UserHostResolver(args.Log.Id);
        //                //}
        //            }
        //            return null;
        //        }

        //        private static async Task ResolveUserHostAsync(int logId)
        //        {
        //            using (ReguxEntities auditDb = new ReguxEntities())
        //            {
        //                Audit audit = await auditDb.Audits.FindAsync(logId);
        //                if (audit != null && audit.UserHostName == null)
        //                {
        //                    try
        //                    {
        //                        IPHostEntry entry = await Dns.GetHostEntryAsync(audit.IpAddress);
        //                        if (entry != null)
        //                        {
        //                            audit.UserHostName = entry.HostName;
        //                            await auditDb.SaveChangesAsync();
        //                        }
        //                    }
        //                    catch (Exception)
        //                    {
        //                    }
        //                }
        //            }
        //        }

    }
}
