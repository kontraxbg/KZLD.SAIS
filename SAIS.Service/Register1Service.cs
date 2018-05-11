using System;
using System.Threading.Tasks;
using SAIS.Data;
using SAIS.Model;
using System.Linq;
using SAIS.Model.Audit;
using Microsoft.EntityFrameworkCore;

namespace SAIS.Service
{
    public class Register1Service : BaseService
    {
        public Register1Service(ContextService contextService)
            : base(contextService)
        {
        }

        public async Task<PersonModel> Get(int id)
        {
            PersonModel model = new PersonModel();

            if (id > 0)
            {
                Person entity = await _db.People.Where(p => p.Id == id).FirstOrDefaultAsync();
                LoadFrom(entity, model);
            }
            return model;
        }

        public string Test()
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateEdit(PersonModel model)
        {
            Person entity = model.Id.HasValue ? await _db.People.FirstOrDefaultAsync(p => p.Id == model.Id.Value) : null;

            if (entity == null)
            {
                entity = new Person();
                _db.People.Add(entity);
            }

            SaveTo(entity, model);
            await SaveAndLogAsync(entity);

            return entity.Id;
        }

        //private void SaveTo(Register1 entity, Register1Model model)
        //{

        //}
        //private void LoadFrom(Register1 entity, Register1Model model)
        //{

        //}

        private void SaveTo(Person entity, PersonModel model)
        {
            entity.FirstName = model.FirstName;
            entity.MiddleName = model.MiddleName;
            entity.LastName = model.LastName;
        }
        private void LoadFrom(Person entity, PersonModel model)
        {
            model.FirstName = entity.FirstName;
            model.MiddleName = entity.MiddleName;
            model.LastName = entity.LastName;
        }
    }
}
