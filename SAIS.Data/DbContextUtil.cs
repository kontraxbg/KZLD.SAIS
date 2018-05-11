using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SAIS.Data
{
    public static class DbContextUtil
    {
        public static async Task<int> SaveChangesWithValidationExplainedAsync(this DbContext db)
        {
            ThrowValidationErrors(db);
            return await db.SaveChangesAsync();
        }

        /// <summary>
        /// Тази (не-async) версия на метода е нужна, за да се извиква от атрибут или в lock секция - все места, които не поддържат async pattern.
        /// </summary>
        public static void SaveChangesWithValidationExplained(this DbContext db)
        {
            ThrowValidationErrors(db);
            db.SaveChanges();
        }

        public static void ThrowValidationErrors(DbContext db)
        {
            System.Text.StringBuilder errors = new System.Text.StringBuilder();

            IEnumerable<EntityEntry> entries = from e in db.ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e;
            foreach (var entry in entries)
            {
                var validationContext = new ValidationContext(entry);
                try
                {
                    Validator.ValidateObject(entry, validationContext);
                }
                catch(ValidationException ex)
                {
                    errors.AppendFormat("{0} {1}: {2}", entry.State,
                        entry.Entity.GetType().Name, ex.ValidationResult.ErrorMessage);
                    errors.AppendLine();
                }
            }

            string result = errors.ToString();
            if (!string.IsNullOrEmpty(result))
            {
                throw new Exception(result);
            }
        }

        public static Task<object> FindAsync(this DbContext db, string entityName, params object[] keyValues)
        {
            Type contextType = db.GetType();
            Type entityType = contextType.Assembly.GetType(string.Format("{0}.{1}", contextType.Namespace, entityName));
            if (entityType != null)
            {
                //var dbSetMethodInfo = typeof(DbContext).GetMethod("Set");
                //var dbSet = dbSetMethodInfo.MakeGenericMethod(entityType).Invoke(db, null);
                //var findMethodInfo = dbSet.GetType().GetMethod("Find");
                //object entity = findMethodInfo.Invoke(dbSet, keyValues);

                return db.FindAsync(entityType, keyValues);
            }
            return Task.FromResult<object>(null);
        }
    }
}
